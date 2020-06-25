using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MySocNet.Enums;

namespace MySocNet.Models
{
    public class Policies
    {
        public const string Admin = "Admin";
        public const string User = "User";

        public static AuthorizationPolicy AdminPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(Admin).Build();
        }

        public static AuthorizationPolicy UserPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(User).Build();
        }
    }
}
