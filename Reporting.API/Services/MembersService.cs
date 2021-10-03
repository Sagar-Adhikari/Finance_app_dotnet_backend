using Dapper;
using pw.Commons.Services;
using Reporting.API.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static pw.Commons.Models.EnumCollection;

namespace Reporting.API.Services
{
    public interface IMembersService
    {
        Task<List<MemberView>> SearchMembers(MemberFilterData queryData);
        Task<List<TinpusteView>> SearchTinPuste(MemberFilterData queryData);
        Task<List<MemberTransactionView>> GetMembersForTransactionView();
        Task<int> CreateNewMember(NewMemberView model);

        Task<int> GetNewMemberId( );
    }
    public class MembersService: IMembersService
    {
        private string conn;
        private int shakhaId;


        public MembersService(string dbConn, int shakhaId)
        {
            this.conn = dbConn;
            this.shakhaId = shakhaId;
        }

        private bool IsValidString(string text)
        {
            if (String.IsNullOrEmpty(text) || String.IsNullOrWhiteSpace(text)) return false;
            return true;
        }

        private DynamicParameters GenerateParameters(MemberFilterData memberQueryData)
        {
            var dictionary = new Dictionary<string, object>()
            {
                { "@AntGrpID", null },
                { "@Amount", null },
                { "@MemIsOFF", null},
                { "@FromDate", null},
                { "@ToDate", null},
                { "@MemLName", null },
                { "@ShareMem", null },
                { "@AccSN", null },
                { "@Jati", null },
                { "@Sex", null },
                { "@AddDist", null },
                { "@AddVDC", null },
                { "@AddWard", null },
                { "@AddTole", null },
                { "ShakhaID",  shakhaId },
            };

            if (memberQueryData == null)
            {
                return new DynamicParameters(dictionary);
            }

            if (memberQueryData.antGrpID > 0)
            {
                dictionary["@AntGrpID"] = memberQueryData.antGrpID;
            }

            dictionary["@Amount"] = memberQueryData.amount;

            if (memberQueryData.balSelected)
            {
                //todo:
            }
            if (IsValidString(memberQueryData.caste))
            {
                dictionary["@MemLName"] = memberQueryData.caste;
            }
            if (IsValidString(memberQueryData.district))
            {
                dictionary["@AddDist"] = memberQueryData.district;
            }
            if (memberQueryData.displayBalanceSelected)
            {
                //todo:
            }
            if (memberQueryData.membersCreatedSelected)
            {
                if (IsValidString(memberQueryData.fromDate))
                {
                    dictionary["@FromDate"] = memberQueryData.fromDate;
                }
                if (IsValidString(memberQueryData.toDate))
                {
                    dictionary["@ToDate"] = memberQueryData.toDate;
                }
            }
            if (NeedShareMemberSelection(memberQueryData.gairShareMembersSelected, memberQueryData.shareMembersSelected))
            {
                dictionary["@ShareMem"] = memberQueryData.shareMembersSelected ? 1 : 0;
            }
            if (memberQueryData.genderId >= 0)
            {
                //todo: check if default value was 0. default value must be set as -1
                dictionary["@Sex"] = memberQueryData.genderId;
            }
            if (memberQueryData.jatiId >= 0)
            {
                //todo: check if default value was 0. default value must be set as -1
                dictionary["@Jati"] = memberQueryData.jatiId;
            }
            dictionary["@MemIsOFF"] = memberQueryData.offMembersSelected ? 1 : 0;
            if (memberQueryData.subAccountId >= 0)
            {
                dictionary["@AccSN"] = memberQueryData.subAccountId;
            }
            if (IsValidString(memberQueryData.tole))
            {
                dictionary["@AddTole"] = memberQueryData.tole;
            }
            if (IsValidString(memberQueryData.vdc))
            {
                dictionary["@AddVDC"] = memberQueryData.vdc;
            }
            if (IsValidString(memberQueryData.ward))
            {
                dictionary["@AddWard"] = memberQueryData.ward;
            }

            return new DynamicParameters(dictionary);
        }

       
        private bool NeedShareMemberSelection(bool gairSharememberSelected, bool sharememberSelected)
        {
            if (gairSharememberSelected && sharememberSelected)
                return false;
            if (!gairSharememberSelected && !sharememberSelected)
                return false;
            return true;
        }

        private string queryMembersNepaliSelector = @"Select MemID as SN, MemID, NameInDev As MemName, AddDev as Address, 
            Phone, BlockNo,d.AntGrpID,MemType,MemCNameDev,GrpNameDev, ";
        private string queryMembersEnglishSelector = @"Select MemID as SN, MemID, (MemFName + ' ' + MemLName) As MemName, 
            (AddVDC + '-' + AddWard + ',' + AddDist) as Address, 
            Phone, BlockNo,d.AntGrpID,MemType,MemCName,GrpName, ";

        //todo: implement upID
        //todo: implement shakha id

        private string queryTinPusteBuilder = @"
            select 
                d.MemID as SN, d.MemID, d.MemType,
                d.MemName, d.NameInDev,
                d.Address, d.AddDev,
                tm.DOBE, tm.DOBN,
                tm.CitzNo,
                tm.FathName, tm.FathNameDev, 
                tm.GPSName as GranFathName, tm.GranFathNameDev  as GranFathNameDev, 
                NULL as ShareAmt, NULL as SavAmt, NULL as RinAmt 
            from viewLedgdets d 
            inner join tblMembers tm on d.MemSN=tm.SN
            Where SN <> 0 
            and (tm.MemIsOFF = @MemIsOFF or @MemIsOFF is null)
            and (MemDate >= @FromDate or @FromDate is null)
            and (MemDate <= @ToDate or @ToDate is null)
            and (ShareMem = @ShareMem  or @ShareMem is null)
            and (d.ShakhaID = @ShakhaID or @ShakhaID is null)
            and (MemLName = @MemLName or @MemLName is null)
            and (Jati = @Jati or @Jati is null)
            and (AddDist = @AddDist or @AddDist is null)
            and (AddVDC = @AddVDC or @AddVDC is null)
            and (AddWard = @AddWard or @AddWard is null)
            and (AddTole = @AddTole or @AddTole is null)
            and (d.AntGrpID = @AntGrpID or @AntGrpID is null)
            and (tm.Sex = @Sex or @Sex is null)
            and (AccSN  = @AccSN or @AccSN is null)
            and (DatedN >= @FromDate or @FromDate is null)
            and (DatedN <= @ToDate or @ToDate is null)
            order by d.MemID 
        ";

        private string queryTinPusteWithBalanceBuilder = @"
            select 
                d.MemID as SN, d.MemID, d.MemType,
                d.MemName, d.NameInDev,
                d.Address, d.AddDev,
                tm.DOBE, tm.DOBN,
                tm.CitzNo,
                tm.FathName, tm.FathNameDev, 
                tm.GPSName as GranFathName, tm.GranFathNameDev  as GranFathNameDev, 
            isNull(Sum(d.CrAmt)-Sum(d.DrAmt),0) as ShareAmt, 
            isnull(
	            (
		            Select Sum(CrAmt)-Sum(DrAmt) 
		            From viewLedgDets 
		            Where MemSN = d.MemSN and AccID = 30 
		            and (AccSN  = @AccSN or @AccSN is null)
		            and (DatedN >= @FromDate or @FromDate is null)
		            and (DatedN <= @ToDate or @ToDate is null)
	            ),
	            0
            ) as SavAmt, 
            isnull(
	            (
		            Select Sum(DrAmt)-Sum(CrAmt) 
		            From viewLedgDets 
		            Where MemSN = d.MemSN and AccID = 110 
		            and (AccSN  = @AccSN or @AccSN is null)
		            and (DatedN >= @FromDate or @FromDate is null)
		            and (DatedN <= @ToDate or @ToDate is null)
	            ),
	            0
            ) as RinAmt 
            from viewLedgdets d 
            inner join tblMembers tm on (d.MemSN=tm.SN) 
            Where SN <> 0 
            and (tm.MemIsOFF = @MemIsOFF or @MemIsOFF is null)
            and (MemDate >= @FromDate or @FromDate is null)
            and (MemDate <= @ToDate or @ToDate is null)
            and (ShareMem = @ShareMem  or @ShareMem is null)
            and (d.ShakhaID = @ShakhaID or @ShakhaID is null)
            and (MemLName = @MemLName or @MemLName is null)
            and (Jati = @Jati or @Jati is null)
            and (AddDist = @AddDist or @AddDist is null)
            and (AddVDC = @AddVDC or @AddVDC is null)
            and (AddWard = @AddWard or @AddWard is null)
            and (AddTole = @AddTole or @AddTole is null)
            and (d.AntGrpID = @AntGrpID or @AntGrpID is null)
            and (tm.Sex = @Sex or @Sex is null)
            and AccID=10 
            and (AccSN  = @AccSN or @AccSN is null)
            and (DatedN >= @FromDate or @FromDate is null)
            and (DatedN <= @ToDate or @ToDate is null)
            group by d.MemID,d.NameInDev,d.AddDev,d.MemType,tm.DOBN,tm.DOBE,tm.CitzNo,tm.FathNameDev,
            tm.GranFathNameDev,d.MemSN,tm.Phone,d.MemName,d.Address,tm.FathName,tm.GPSName 
            order by d.MemID
         ";

        private string queryMembersBuilder = @" {0}
            MemDate as Remarks,
            (
	            Select Max(SavAccID) from tblsavingAccounts Where AccSN = 0 and MemSN= d.SN
            ) as SavAccID,             
            FMCntM+FmCntF as FMCnt,
            '' as ShareAmt
            From tblMembers d 
            Inner Join tblMicroGrp On tblMicroGrp.GrpID=d.Memtype  
            Inner Join 
            (
                select MemSN from tblLedgdets 
                Where (AccSN  = @AccSN or @AccSN is null)
                group by MemSN having (SUm(CrAmt)-Sum(DrAmt)) >= @Amount
            ) tl on tl.MemSN = d.SN
            Where SN <> 0 
            and (MemIsOFF = @MemIsOFF or @MemIsOFF is null)
            and (MemDate >= @FromDate or @FromDate is null)
            and (MemDate <= @ToDate or @ToDate is null)
            and (ShareMem = @ShareMem  or @ShareMem is null)
            and (d.ShakhaID = @ShakhaID or @ShakhaID is null)
            and (MemLName = @MemLName or @MemLName is null)
            and (Jati = @Jati or @Jati is null)
            and (AddDist = @AddDist or @AddDist is null)
            and (AddVDC = @AddVDC or @AddVDC is null)
            and (AddWard = @AddWard or @AddWard is null)
            and (AddTole = @AddTole or @AddTole is null)
            and (d.AntGrpID = @AntGrpID or @AntGrpID is null)
            and (Sex = @Sex or @Sex is null)
            Order By MemID {1}
        ";


        private string queryMembersWithBalanceBuilder = @" {0}
            MemDate as Remarks,
            (
	            Select Max(SavAccID) from tblsavingAccounts Where AccSN = 0 and MemSN= d.SN
            ) as SavAccID, 
            FMCntM+FmCntF as FMCnt,
            (
	            Select Sum(CrAmt)-Sum(DrAmt) 
	            from tblLedGdets 
	            Where AccSN = 76 and MemsN = d.SN
            ) as ShareAmt
            From tblMembers d 
            Inner Join tblMicroGrp On tblMicroGrp.GrpID=d.Memtype  
            Inner Join 
            (
                select MemSN from viewLedgdets 
                Where (AccSN  = @AccSN or @AccSN is null)
                and (DatedN >= @FromDate or @FromDate is null)
                and (DatedN <= @ToDate or @ToDate is null)
                group by MemSN having (SUm(CrAmt)-Sum(DrAmt)) >= @Amount
            ) tl on tl.MemSN = d.SN
            Inner Join 
            (
                Select MemSN 
	            from tblLedgdets 
	            Where (AccSN  = @AccSN or @AccSN is null)
	            Group By MemSN having (sum(CrAmt)-Sum(Dramt)) >= @Amount
            ) t2 on t2.MemSN = d.SN
            Where SN <> 0 
            and (MemIsOFF = @MemIsOFF or @MemIsOFF is null)
            and (MemDate >= @FromDate or @FromDate is null)
            and (MemDate <= @ToDate or @ToDate is null)
            and (ShareMem = @ShareMem  or @ShareMem is null)
            and (d.ShakhaID = @ShakhaID or @ShakhaID is null)
            and (MemLName = @MemLName or @MemLName is null)
            and (Jati = @Jati or @Jati is null)
            and (AddDist = @AddDist or @AddDist is null)
            and (AddVDC = @AddVDC or @AddVDC is null)
            and (AddWard = @AddWard or @AddWard is null)
            and (AddTole = @AddTole or @AddTole is null)
            and (d.AntGrpID = @AntGrpID or @AntGrpID is null)
            and (Sex = @Sex or @Sex is null)
            Order By MemID {1}
        ";

        private string GetMemberSearchQuery(MemberFilterData queryData)
        {
            var sortOrder = queryData.sortOrder == SortOrderEnum.Asc ? "asc" : "desc";
            if (queryData.selectedLanguage == LanguageEnum.English)
            {
                return queryData.balSelected 
                    ? String.Format(queryMembersWithBalanceBuilder, queryMembersEnglishSelector, sortOrder)
                    : String.Format(queryMembersBuilder, queryMembersEnglishSelector, sortOrder);
            }
            else
            {
                return queryData.balSelected
                    ? String.Format(queryMembersWithBalanceBuilder, queryMembersNepaliSelector, sortOrder)
                    : String.Format(queryMembersBuilder, queryMembersNepaliSelector, sortOrder);
            }
        }

        public async Task<List<TinpusteView>> SearchTinPuste(MemberFilterData queryData)
        {
            IEnumerable<TinpusteView> tbResults;
            var sql = queryData.balSelected ? queryTinPusteWithBalanceBuilder : queryTinPusteBuilder;

            using (IDbConnection sqlConn = new SqlConnection(conn))
            {
                sqlConn.Open();
                tbResults = await sqlConn.QueryAsync<TinpusteView>(sql, GenerateParameters(queryData), commandTimeout: 300);
            }

            return tbResults.ToList();
        }

        public async Task<List<MemberView>> SearchMembers(MemberFilterData queryData)
        {
            IEnumerable<MemberResult> tbResults;

            var sql = GetMemberSearchQuery(queryData);

            using (IDbConnection sqlConn = new SqlConnection(conn))
            {
                sqlConn.Open();
                tbResults = await sqlConn.QueryAsync<MemberResult>(sql, GenerateParameters(queryData), commandTimeout: 300);
            }

            var lsMembers = new List<MemberView>();

            foreach (var member in tbResults)
            {
                lsMembers.Add(new MemberView()
                {
                    Id = Convert.ToInt32(member.MemID),
                    FullName = member.MemName,
                    Address = member.Address,
                    GroupId = Convert.ToInt32(member.AntGrpID),
                    MemType = Convert.ToInt32(member.MemType),
                    BlockNo = member.BlockNo,
                    Phone = member.Phone,
                    Remarks = member.Remarks,
                    ShareAmt = member.ShareAmt
                });
            }

            return lsMembers;
        }

        public async Task<List<MemberTransactionView>> GetMembersForTransactionView() 
        {
            //var query = @"Select SN, MemFName, MemLName, RInChha, JamaniChha
            //                FROM tblMembers Where (ShareMem = 1
				        //                    OR ShareMem = 0)
				        //                    And
				        //                    (MemType = 3
				        //                    Or MemCatCode = 1)";
            var query = @$"Select top 30 SN, MemID, MemFName, MemMName, MemLName, Photo, Sig1, Sig2
                            FROM tblMembers Where (ShareMem = 1 OR ShareMem = 0)
                            and photo is not null";

            IEnumerable<MemberTransactionView> tbResults = await BaseDapper<MemberTransactionView>.QueryAsync(query, conn);
            return tbResults?.ToList();
        }

        public async Task<int> CreateNewMember(NewMemberView model)
        {
            var maxIdQuery = "select max(MemID) from tblMembers;";
            var maxId = await BaseDapper<int>.ExecuteScalarAsync(maxIdQuery, conn);
            var newMemId = maxId + 1;
            var base64StringFrmtMemPh = model.PersonalInfo.memberPhotoBase64;
            if (base64StringFrmtMemPh.Contains("base64"))
            {
                base64StringFrmtMemPh = base64StringFrmtMemPh.Substring(base64StringFrmtMemPh.IndexOf("base64,") + "base64,".Length);
            }                
            var memberPhoto = Convert.FromBase64String(base64StringFrmtMemPh);

            var insertQuery = @"INSERT INTO [dbo].[tblMembers]
	           (
		           [MemID]
                   ,[MemFName]
                   ,[MemLName]
                   ,[NameInDev]
                   ,[AddVDC]
                   ,[AddWard]
                   ,[AddTole]
                   ,[AddDist]
                   ,[AddZone]
                   ,[AddCountry]
                   ,[Phone]
                   ,[DOBE]
                   ,[DOBN]
                   ,[FathName]
                   ,[HakWala]
                   ,[Relation]
                   ,[Photo]
                   --,[Sig1]
                   --,[Sig2]
                   --,[HakWalaPhoto]
                   ,[ShareMem]
                   ,[ShareQty]
                   ,[ShareAmt]
                   ,[AEIL]
                   ,[JamaniChha]
                   ,[HakWalaAdd]
                   ,[HDOBN]
                   ,[MemDate]
                   ,[YeskoJamani]
                   ,[RinChha]
                   ,[MemType]
                   ,[Sex]
                   ,[Jati]
                   ,[PPP]
                   ,[SaveingAmt]
                   ,[MemCatCode]
                   ,[AddDev]
                   ,[AntGrpID]
                   ,[BlockNo]
                   ,[MemIsOFF]
                   ,[TapCatID]
                   ,[CurTapState_Bi]
                   ,[CurTapState_KN]
                   ,[SewaBanda]
                   ,[PreRdingID]
                   ,[RiniPrakar]
                   ,[RdingDayCode]
                   --,[Apanga]
                   --,[Pratinidhi]
                   ,[GrndPaSasura]
                   ,[GpSName]
                   ,[HPorP]
                   ,[HPPName]
                   ,[ShakhaID]
                   --,[PrevMemSN]
                   --,[Remarks]
                   --,[HeadMSN]
                   --,[HeadMem]
                   --,[CoMemID]
                   --,[RelMemSN]
                   ,[DCollAmt]
                   ,[DCollDay]
                   ,[MemTitle]
                   ,[MemMName]
                   ,[MemCName]
                   ,[CitzNo]
                   ,[PermAdd]
                   ,[FMCntM]
                   ,[MothName]
                   ,[Occupation]
                   ,[MaritalStatus]
                   ,[SpouseName]
                   --,[SetKista]
                   --,[ShramaDan]
                   --,[FlowSize]
                   ,[DharaJadanDate]
                   ,[MeterJadanDate]
                   ,[MeterNo]
                   ,[AddToleDev]
                   ,[Dharma]
                   ,[PrevLactoM]
                   ,[PrevFatM]
                   ,[PrevLactoE]
                   ,[PrevFatE]
                   ,[FathNameDev]
                   ,[GranFathNameDev]
                   ,[MemCNameDev]
                   ,[BankACNo]
                   ,[FMCntF]
                   ,[FMCntA]
                   ,[RollNo]
                   ,[RegNo]
                   ,[AcLevelID]
                   ,[GuardainsName]
                   ,[DrAmt]
                   ,[Sec]
                   ,[Grading]
                   ,[OldStud]
                   ,[FeeStyleCode]
                   ,[HostelFeeStyle]
                   ,[TranFeeStyle]
                   ,[BloodGrp]
                   ,[PhPerm]
                   ,[AddTemp]
                   ,[PhTemp]
                   ,[Religion]
                   ,[OrgGrd]
                   ,[PhOrgGrd]
                   ,[AdmCrded]
                   ,[DoNotDues]
                   ,[UpToDate]
                   ,[CertiNo]
                   ,[StudCatID]
                   ,[FamRelCode]
                   ,[ReaderID]
                   ,[PANNo]
                   ,[Remarks1]
                   ,[CitzDateN]
                   ,[CitzDist]
                   ,[FBID]
                   ,[Email]
                   ,[MothOccupation]
                   ,[TranOneWay]
	           )
             VALUES
	           (
			        @MemID
			        ,@MemFName
			        ,@MemLName
			        ,@NameInDev
			        ,@AddVDC
			        ,@AddWard
			        ,@AddTole
			        ,@AddDist
			        ,@AddZone
			        ,@AddCountry
			        ,@Phone
			        ,@DOBE
			        ,@DOBN
			        ,@FathName
			        ,@HakWala
			        ,@Relation
			        ,@Photo
			        --,@Sig1
			        --,@Sig2
			        --,@HakWalaPhoto
			        ,@ShareMem
			        ,@ShareQty
			        ,@ShareAmt
			        ,@AEIL
			        ,@JamaniChha
			        ,@HakWalaAdd
			        ,@HDOBN
			        ,@MemDate
			        ,@YeskoJamani
			        ,@RinChha
			        ,@MemType
			        ,@Sex
			        ,@Jati
			        ,@PPP
			        ,@SaveingAmt
			        ,@MemCatCode
			        ,@AddDev
			        ,@AntGrpID
			        ,@BlockNo
			        ,@MemIsOFF
			        ,@TapCatID
			        ,@CurTapState_Bi
			        ,@CurTapState_KN
			        ,@SewaBanda
			        ,@PreRdingID
			        ,@RiniPrakar
			        ,@RdingDayCode
			        --,@Apanga
			        --,@Pratinidhi
			        ,@GrndPaSasura
			        ,@GpSName
			        ,@HPorP
			        ,@HPPName
			        ,@ShakhaID
			        --,@PrevMemSN
			        --,@Remarks
			        --,@HeadMSN
			        --,@HeadMem
			        --,@CoMemID
			        --,@RelMemSN
			        ,@DCollAmt
			        ,@DCollDay
			        ,@MemTitle
			        ,@MemMName
			        ,@MemCName
			        ,@CitzNo
			        ,@PermAdd
			        ,@FMCntM
			        ,@MothName
			        ,@Occupation
			        ,@MaritalStatus
			        ,@SpouseName
			        --,@SetKista
			        --,@ShramaDan
			        --,@FlowSize
			        ,@DharaJadanDate
			        ,@MeterJadanDate
			        ,@MeterNo
			        ,@AddToleDev
			        ,@Dharma
			        ,@PrevLactoM
			        ,@PrevFatM
			        ,@PrevLactoE
			        ,@PrevFatE
			        ,@FathNameDev
			        ,@GranFathNameDev
			        ,@MemCNameDev
			        ,@BankACNo
			        ,@FMCntF
			        ,@FMCntA
			        ,@RollNo
			        ,@RegNo
			        ,@AcLevelID
			        ,@GuardainsName
			        ,@DrAmt
			        ,@Sec
			        ,@Grading
			        ,@OldStud
			        ,@FeeStyleCode
			        ,@HostelFeeStyle
			        ,@TranFeeStyle
			        ,@BloodGrp
			        ,@PhPerm
			        ,@AddTemp
			        ,@PhTemp
			        ,@Religion
			        ,@OrgGrd
			        ,@PhOrgGrd
			        ,@AdmCrded
			        ,@DoNotDues
			        ,@UpToDate
			        ,@CertiNo
			        ,@StudCatID
			        ,@FamRelCode
			        ,@ReaderID
			        ,@PANNo
			        ,@Remarks1
			        ,@CitzDateN
			        ,@CitzDist
			        ,@FBID
			        ,@Email
			        ,@MothOccupation
			        ,@TranOneWay
	           )";

            var dictionary = new Dictionary<string, object>()
            {
                { "@MemID", newMemId },
                { "@MemFName", model.PersonalInfo.firstName },
                { "@MemLName", model.PersonalInfo.lastName},
                { "@NameInDev", model.PersonalInfo.nameInDevnagari },
                { "@AddVDC", model.AddressInfo.CurrentAddress.vdcMunci },
                { "@AddWard", model.AddressInfo.CurrentAddress.wardNum },
                { "@AddTole", model.AddressInfo.CurrentAddress.tole },
                { "@AddDist", model.AddressInfo.CurrentAddress.district },
                { "@AddZone", model.AddressInfo.CurrentAddress.province },//todo:?
                { "@AddCountry", model.AddressInfo.CurrentAddress.country },
                { "@Phone", model.PersonalInfo.phoneMobile },
                { "@DOBE", model.PersonalInfo.dobAD },
                { "@DOBN", model.PersonalInfo.dobBS },
                { "@FathName", model.FamilyInfo.fatherFullName },
                { "@HakWala", model.NomineeInfo.fullName },
                { "@Relation", model.NomineeInfo.relation },
                { "@Photo",  Convert.FromBase64String(model.PersonalInfo.memberPhotoBase64 )},
                //{ "@Sig1", null },
                //{ "@Sig2", null },
                //{ "@HakWalaPhoto", null },//todo:
                { "@ShareMem", '0' },//todo:
                { "@ShareQty", null },
                { "@ShareAmt", null },
                { "@AEIL", null },
                { "@JamaniChha", null },
                { "@HakWalaAdd", model.NomineeInfo.address },
                { "@HDOBN", null },
                { "@MemDate", DateTime.Now },
                { "@YeskoJamani", null },
                { "@RinChha", null },
                { "@MemType", null },
                { "@Sex", model.PersonalInfo.gendId },
                { "@Jati", null },
                { "@PPP", null },
                { "@SaveingAmt", null },
                { "@MemCatCode", null },
                { "@AddDev", null },
                { "@AntGrpID", null },
                { "@BlockNo", null },
                { "@MemIsOFF", null },
                { "@TapCatID", null },
                { "@CurTapState_Bi", null },
                { "@CurTapState_KN", null },
                { "@SewaBanda", null },
                { "@PreRdingID", null },
                { "@RiniPrakar", null },
                { "@RdingDayCode", null },
                //{ "@Apanga", null },
                //{ "@Pratinidhi", null },
                { "@GrndPaSasura", null },
                { "@GpSName", model.FamilyInfo.grandFatherFullName },
                { "@HPorP", null },
                { "@HPPName", null },
                { "@ShakhaID", shakhaId },
                //{ "@PrevMemSN", null },
                //{ "@Remarks", null },
                //{ "@HeadMSN", null },
                //{ "@HeadMem", null },
                //{ "@CoMemID", null },
                //{ "@RelMemSN", null },
                { "@DCollAmt", null },
                { "@DCollDay", null },
                { "@MemTitle", null },
                { "@MemMName", model.PersonalInfo.middleName },
                { "@MemCName", null },
                { "@CitzNo", model.PersonalInfo.citizenshipNo },
                { "@PermAdd", model.AddressInfo.PermanentAddress.ToStr()},
                { "@FMCntM", null },
                { "@MothName", model.FamilyInfo.motherFullName },
                { "@Occupation", null },
                { "@MaritalStatus", model.PersonalInfo.isMarried },
                { "@SpouseName", model.FamilyInfo.spouseFullName },
                //{ "@SetKista", null },
                //{ "@ShramaDan", null },
                //{ "@FlowSize", null },
                { "@DharaJadanDate", null },
                { "@MeterJadanDate", null },
                { "@MeterNo", null },
                { "@AddToleDev", null },
                { "@Dharma", null },
                { "@PrevLactoM", null },
                { "@PrevFatM", null },
                { "@PrevLactoE", null },
                { "@PrevFatE", null },
                { "@FathNameDev", null },
                { "@GranFathNameDev", null },
                { "@MemCNameDev", null },
                { "@BankACNo", null },
                { "@FMCntF", null },
                { "@FMCntA", null },
                { "@RollNo", null },
                { "@RegNo", null },
                { "@AcLevelID", null },
                { "@GuardainsName", null },
                { "@DrAmt", null },
                { "@Sec", null },
                { "@Grading", null },
                { "@OldStud", null },
                { "@FeeStyleCode", null },
                { "@HostelFeeStyle", null },
                { "@TranFeeStyle", null },
                { "@BloodGrp", null },
                { "@PhPerm", null },
                { "@AddTemp", null },
                { "@PhTemp", null },
                { "@Religion", null },
                { "@OrgGrd", null },
                { "@PhOrgGrd", null },
                { "@AdmCrded", null },
                { "@DoNotDues", null },
                { "@UpToDate", null },
                { "@CertiNo", null },
                { "@StudCatID", null },
                { "@FamRelCode", null },
                { "@ReaderID", null },
                { "@PANNo", null },
                { "@Remarks1", null },
                { "@CitzDateN", model.PersonalInfo.citizenshipIssDt },
                { "@CitzDist", model.PersonalInfo.citizenshipIssfrom },
                { "@FBID", null },
                { "@Email", model.PersonalInfo.email },
                { "@MothOccupation", null },
                { "@TranOneWay", null }
            };

            var executed = await BaseDapper<int>.ExecuteAsync(insertQuery, conn, new DynamicParameters(dictionary));

            return executed > 0 ? newMemId : 0;
        }

        public async Task<int> GetNewMemberId()
        {
            var maxIdQuery = "select max(MemID) from tblMembers;";
            var maxId = await BaseDapper<int>.ExecuteScalarAsync(maxIdQuery, conn);
            var newMemId = maxId + 1;
            return newMemId;

        }
    }

    class MemberResult
    {
        public string MemID { get; set; }
        public string MemName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string BlockNo { get; set; }
        public string AntGrpID { get; set; }
        public string MemType { get; set; }
        public string Remarks { get; set; }
        public string ShareAmt { get; set; }
    }

}

