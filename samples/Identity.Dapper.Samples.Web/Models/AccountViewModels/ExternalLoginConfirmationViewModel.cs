using System.ComponentModel.DataAnnotations;

namespace Identity.Dapper.Samples.Web.Models.AccountViewModels
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
