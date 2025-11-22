using System.ComponentModel.DataAnnotations;

namespace Khutootcompany.presention.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Display(Name = "اسم المستخدم")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        [Display(Name = "الاسم الكامل")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "الصلاحية مطلوبة")]
        [Display(Name = "الصلاحية")]
        public string Role { get; set; } = "User";


        // New Fields
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "كلمة المرور غير متطابقة")]
        public string? ConfirmPassword { get; set; }
    }
}
