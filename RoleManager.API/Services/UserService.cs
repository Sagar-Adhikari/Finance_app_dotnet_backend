using IdentityServer.Data;
using IdentityServer.Entities;
using RoleManager.API.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoleManager.API.Services
{
    public interface IUserService
    {
        Task<UserDetailView> CreateUserDetail(UserDetailView userDetailData, string userId);
        Task<UserDetailView> UpdateUserDetail(UserDetailView userDetailData, string userId);
        Task<UserDetailView> GetUserDetail(string userId);
    }

    public class UserService : IUserService
    {
        private readonly AppDbContext appDbContext;

        public UserService(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<UserDetailView> CreateUserDetail(UserDetailView userDetailData, string userId)
        {

            var userDetail = new UserDetail()
            {
                UserId = userId,
                Post = userDetailData.Post,
                Permission = userDetailData.Permission,
                Status = userDetailData.Status,
                Locked = userDetailData.Locked,
                LockMsg = userDetailData.LockMsg,
                LockMsgDev = userDetailData.LockMsgDev,
                FullNameDev = userDetailData.FullNameDev,
                DrLimit = userDetailData.DrLimit,
                CrLimit = userDetailData.CrLimit,
                Stackverifier = userDetailData.Stackverifier,
                Comp1 = userDetailData.Comp1,
                Comp2 = userDetailData.Comp2,
                Hidden = userDetailData.Hidden,
                ShakhaId = userDetailData.ShakhaId,
            };

            this.appDbContext.UserDetails.Add(userDetail);

            var res = await this.appDbContext.SaveChangesAsync();

            return new UserDetailView(userDetail);
        }

        public async Task<UserDetailView> UpdateUserDetail(UserDetailView userDetailData, string userId)
        {
            var userDetail = appDbContext.UserDetails.FirstOrDefault(u => u.UserId == userId);
            if(userDetail == null)
            {
                return null;
            }

            userDetail.Post = userDetailData.Post;
            userDetail.Permission = userDetailData.Permission;
            userDetail.Status = userDetailData.Status;
            userDetail.Locked = userDetailData.Locked;
            userDetail.LockMsg = userDetailData.LockMsg;
            userDetail.LockMsgDev = userDetailData.LockMsgDev;
            userDetail.FullNameDev = userDetailData.FullNameDev;
            userDetail.DrLimit = userDetailData.DrLimit;
            userDetail.CrLimit = userDetailData.CrLimit;
            userDetail.Stackverifier = userDetailData.Stackverifier;
            userDetail.Comp1 = userDetailData.Comp1;
            userDetail.Comp2 = userDetailData.Comp2;
            userDetail.Hidden = userDetailData.Hidden;
            userDetail.ShakhaId = userDetailData.ShakhaId;

            appDbContext.UserDetails.Update(userDetail);
            await appDbContext.SaveChangesAsync();

            return new UserDetailView(userDetail);
        }

        public async Task<UserDetailView> GetUserDetail(string userId)
        {
            var userDetail = await Task.Run(() => appDbContext.UserDetails.FirstOrDefault(u => u.UserId == userId));
            if(userDetail == null)
            {
                return null;
            }
            return new UserDetailView(userDetail);
        }
    }
}
