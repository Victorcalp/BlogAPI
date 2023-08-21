using System.ComponentModel.DataAnnotations;

namespace BlogAPI.ViewModels.Accounts
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatorio")]
        public string Name { get; set; }

        [Required(ErrorMessage = "E-mail é obrigatorio")]
        [EmailAddress(ErrorMessage = "O email é invalido")]
        public string Email { get; set; }
    }
}
