using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nlayer.Core;
using Nlayer.Core.DTOs;
using Nlayer.Core.Services;

namespace NLayer.API.Filters
{
    public class NotFoundFilter<T> : IAsyncActionFilter where T : BaseEntity  // Sadece Product için olmasın tüm entity lerde id var, hepsi için ortak olsun
    {
        // Eğer bir filter ctor da bir class ya da interface yi DI olarak geçiyorsa, bunu program.cs de de eklemek lazım
        private readonly IService<T> _service;

        public NotFoundFilter(IService<T> service)
        {
            _service = service;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) // next in amacı şu; eğer herhangi bir filter a takılmazsa next diyip bu requesti yoluna devam ettircez
        {

            var idValue = context.ActionArguments.Values.FirstOrDefault(); // endpoint lerimize gidince id yi yakalamak için bunu kullandık, parametredeki ilk değeri al dedik

            if (idValue == null)
            {
                await next.Invoke(); // sen yoluna devam et, id null geldi demek
                return;
            }

            var id = (int)idValue;
            var anyEntity = await _service.AnyAsync(x => x.Id == id); // bu id ye ait ürün var mı yok mu diye Any ile kontrol ettik

            if (anyEntity)
            {
                await next.Invoke(); // eğer gerçekten böyle bir entity var ise yoluna devam etsin
                return;
            }


            // Buray kadar geldiyse artık bu id ye ait data yok demektir ve hata dönücez
            context.Result = new NotFoundObjectResult(CustomResponseDto<NoContentDto>.Fail(404, $"{typeof(T).Name}({id}) not found"));

        }
    }
}
