using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nlayer.Core.DTOs;

namespace NLayer.API.Filters
{
    public class ValidateFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                // SelectMany ile dictionary yi tek tek ModelStateEntry leri getir dedik, sonra bunun içinde Select ile ErrorMessage ları getirdik

                context.Result = new BadRequestObjectResult(CustomResponseDto<NoContentDto>.Fail(400, errors)); // FluentValidation kullansakta kullanmasakta oluşan hatalar context.ModelState e map leniyor
                                                                                                                // Result ın body kısmında data olarak errors ları da göndereceğiz, o yüzden BadRequestObjectResult seçtik

            }
        }
    }
}
