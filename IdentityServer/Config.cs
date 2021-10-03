using IdentityModel;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiResource> GetApis() =>
            new List<ApiResource>
            {
                new ApiResource("RoleManager.API", "RoleManager.API")
                {
                    UserClaims = {"role", "userNo", "sub", "name", "shakhaId" }
                },
                new ApiResource("Reporting.API", "Reporting.API")
                {
                    UserClaims = {"role"}
                }
            };
    }
}