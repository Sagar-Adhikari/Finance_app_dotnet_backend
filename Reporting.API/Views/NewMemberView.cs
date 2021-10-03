
using System.ComponentModel.DataAnnotations;

namespace Reporting.API.Views
{
    public class PersonalInfo
    {
        [Required]
        public string firstName { get; set; }
        public string middleName { get; set; }
        [Required]
        public string lastName { get; set; }
        public string nameInDevnagari { get; set; }
        [Required]
        [Range(0,5)]
        public int gendId { get; set; }
        public string dobAD { get; set; }
        public string dobBS { get; set; }
        public string phoneLandline { get; set; }
        public string phoneMobile { get; set; }
        public string email { get; set; }
        public string citizenshipNo { get; set; }
        public string citizenshipIssDt { get; set; }
        public string citizenshipIssfrom { get; set; }
        public bool isMarried { get; set; }
        public string nationality { get; set; }
        public string memberPhotoBase64 { get; set; }
    }
    public class OccupationInfo
    {
        public OccupationEnum yourOccupation { get; set; }
        public int panNo { get; set; }
        public OccupationEnum spouseOccupation { get; set; }
        public string otherRelationship { get; set; }
        public OccupationEnum otherOccupation { get; set; }
        public bool isHoldingOtherPost { get; set; }
        public string name { get; set; }
        public string relationship { get; set; }
        public string positionTitle { get; set; }
    }
    public class IncomeInfo
    {
    }
    public class FamilyInfo
    {
        [Required]
        public string fatherFullName { get; set; }
        public string grandFatherFullName { get; set; }
        public string motherFullName { get; set; }
        public string spouseFullName { get; set; }
        public string sonFullName { get; set; }
        public string daughterInLawFullName { get; set; }
        public string fatherInLawFullName { get; set; }
        public string motherInLawFullName { get; set; }
    }
    public class NomineeInfo
    {
        public bool isNominee { get; set; }
        [Required]
        public string fullName { get; set; }
        [Required]
        public string relation { get; set; }
        public string citizenshipNo { get; set; }
        public string citizenshipIssuedDate { get; set; }
        public string citizenshipIssuedFrom { get; set; }
        public string address { get; set; }
        public string nomineePhotoBase64 { get; set; }
    }
    public class AddressInfo
    {
        public AddressModel CurrentAddress { get; set; }
        public AddressModel PermanentAddress { get; set; }
        public bool CurSameAsPermAdd { get; set; }
    }
    public class AddressModel 
    {
        public string country { get; set; }
        public string province { get; set; }
        public string district { get; set; }
        public string vdcMunci { get; set; }
        public string wardNum { get; set; }
        public string tole { get; set; }

        public string ToStr()
        {
            return $"{this.tole} - {this.wardNum}, {this.vdcMunci}, {this.district}, {this.province}, {this.country}";
        }
    }

    public class CoOperativeInfo
    {
    }

    public enum OccupationEnum
    {
        service,
        govt,
        publicOrPrivateSector,
        agriculture,
        retired,
        foreignEmployment,
        others
    }

    public class NewMemberView
    {
        [Required]
        public PersonalInfo PersonalInfo { get; set; }
        public OccupationInfo OccupationInfo { get; set; }
        public IncomeInfo IncomeInfo { get; set; }
        [Required]
        public FamilyInfo FamilyInfo { get; set; }
        [Required]
        public NomineeInfo NomineeInfo { get; set; }
        [Required]
        public AddressInfo AddressInfo { get; set; }
        public CoOperativeInfo CoOperativeInfo { get; set; }
    }


}
