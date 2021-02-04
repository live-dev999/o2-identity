using System.ComponentModel.DataAnnotations;

namespace O2.Identity.Web.Models.AccountViewModels
{
    public class VerifyViewModel
    {
        [Required]
        [Display(Name = "Verification Code")]
        public string Code { get; set; }

        public string PhoneNumber { get; set; }
    }
}