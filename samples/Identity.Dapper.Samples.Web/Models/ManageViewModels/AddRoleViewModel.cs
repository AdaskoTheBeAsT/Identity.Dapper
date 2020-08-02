using System.ComponentModel.DataAnnotations;

namespace Identity.Dapper.Samples.Web.Models.ManageViewModels
{
    public class AddRoleViewModel
    {
        [Display(Name = "Role Name")]
        [Required]
        public string? RoleName { get; set; }
    }
}
