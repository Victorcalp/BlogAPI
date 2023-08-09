using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BlogAPI.Extensions
{
    public static class ModelStateExtension
    {
        //Colocando o "this" ele adiciona o GetErrors dentro de ModelStateDictionary
        public static List<string> GetErrors(this ModelStateDictionary modelState)
        {
            var errors = new List<string>();

            foreach (var item in modelState.Values)
            {
                foreach (var error in item.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
            }
            return errors;
        }
    }
}
