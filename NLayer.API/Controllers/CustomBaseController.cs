using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nlayer.Core.DTOs;

namespace NLayer.API.Controllers
{
    [Route("api/[controller]")] // [action] belirtmedik, bizim HttpGet ve HttpPost işlemleirmize göre kendi seçer metodu, metodun adına göre değil direk tipine göre seçer
    [ApiController]
    public class CustomBaseController : ControllerBase
    {
        [NonAction] // bu bir endpoint değil, swagger bunu anlasın diye bunu ekledik, yoksa get veya post yok diye hata verir
        public IActionResult CreateActionResult<T>(CustomResponseDto<T> response)
        {
            if (response.StatusCode == 204)
                return new ObjectResult(null)
                {
                    StatusCode = response.StatusCode
                };

            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }
    }
}
