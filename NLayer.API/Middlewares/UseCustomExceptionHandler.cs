using Microsoft.AspNetCore.Diagnostics;
using Nlayer.Core.DTOs;
using NLayer.Service.Services.Exceptions;
using System.Text.Json;

namespace NLayer.API.Middlewares
{
    // Middleware lar uygulamaya bir request geldiğinde aşama aşama middleware lardan geçer, response a dönüşürken de yine aynı şekilde middleware lara uğrayarak response a dönüşür 
    public static class UseCustomExceptionHandler // Extention Metod yazabilmek için metodum ve class ım mutlaka static olmak zorunda
    {
        public static void UseCustomException(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(config => // UseExceptionHandler normalde bir exception fırlatılırken çalışır, biz kendi modelimizi dönücez, bu yüzden bunu değiştircez
            {

                config.Run(async context => // Run sonlandırıcı bir middleware dir, artık bu koddan sonra akış geriye dönecek, request buraya girdiği anda daha ileriye gitmeyecek, controller lara falan gitmeyecek, hemen geri dönecek
                {
                    context.Response.ContentType = "application/json"; // geriye json dönücez

                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>(); // uygulamda fırlatılan hataları alırız

                    var statusCode = exceptionFeature.Error switch // burada uygulamdan kaynaklı bir hata da oluşabiliriz, bizde bir hata dönebiliriz 
                    {
                        ClientSideException => 400, // eğer bir ClientSideException ise geriye 400 dön
                        NotFoundExcepiton => 404,
                        _ => 500 // bu hatların dışında ise 500 ata dedik, 500 hataları serverdan kaynaklıdır, client a bunları ortak bir mesaj olarak dönebiliriz 
                    };
                    context.Response.StatusCode = statusCode;


                    var response = CustomResponseDto<NoContentDto>.Fail(statusCode, exceptionFeature.Error.Message);


                    await context.Response.WriteAsync(JsonSerializer.Serialize(response)); // response bir tiptir onu geriye dönmek için json a serialize atmemiz gerek

                    // Normal controllerlarda framework class larımızı otomatik olarak döndüğümüz tipten dolayı json a çevirir, ama burada kendimiz bir middleware yazdık, o yüzden bunu json a çevirmemiz gerek

                });
            });
        }
    }
}
