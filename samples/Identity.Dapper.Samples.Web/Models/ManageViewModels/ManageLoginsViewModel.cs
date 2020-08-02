using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Identity.Dapper.Samples.Web.Models.ManageViewModels
{
    public class ManageLoginsViewModel
    {
#pragma warning disable CA2227 // Collection properties should be read only
        public IList<UserLoginInfo>? CurrentLogins { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

#pragma warning disable CA2227 // Collection properties should be read only
        public IList<AuthenticationScheme>? OtherLogins { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
