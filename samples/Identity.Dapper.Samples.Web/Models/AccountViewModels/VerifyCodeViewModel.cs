using System.ComponentModel.DataAnnotations;

namespace Identity.Dapper.Samples.Web.Models.AccountViewModels
{
    public class VerifyCodeViewModel
    {
        [Required]
        public string? Provider { get; set; }

        [Required]
        public string? Code { get; set; }

#pragma warning disable CA1056 // Uri properties should not be strings
        public string? ReturnUrl { get; set; }
#pragma warning restore CA1056 // Uri properties should not be strings

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
