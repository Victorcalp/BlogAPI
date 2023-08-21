using System.ComponentModel.DataAnnotations;

namespace BlogAPI.ViewModels.Accounts
{
    public class UploadImageViewModel
    {
        [Required(ErrorMessage = "Imagem Invalida")]
        public string Base64Image { get; set; }
    }
}
