using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nlayer.Core.DTOs
{
    // Endpointlerden dönen işlem başarılı ya da başarısız olması için ortak bir Dto dönmek best practice açısından daha iyidir
    // Tek bir model üzerinden ilerlersek işlem başarılı da başarısız da olsa bunu tüketecek olan clientlar tek bir model döner
    // Bizim döndüğümüz bu model ile clientlar sonuçları kolaycaa kendi üzerine alırlar
    public class CustomResponseDto<T>
    {
        public T Data { get; set; }

        [JsonIgnore] // StatusCode u client lara açmak istemiyorum çünkü onlar zaten endpointlere bir istek yapınca StatusCode u otomatik olarak elde ediyorlar
                     // bu yüzden  [JsonIgnore] olarak işaretledik, yani bunu json yapıp client a verirken ignore et alma dedik
        public int StatusCode { get; set; }


        public List<String> Errors { get; set; }


        public static CustomResponseDto<T> Success(int statusCode, T data)
        {
            return new CustomResponseDto<T> { Data = data, StatusCode = statusCode };
        }

        public static CustomResponseDto<T> Success(int statusCode)
        {
            return new CustomResponseDto<T> { StatusCode = statusCode };
        }

        public static CustomResponseDto<T> Fail(int statusCode, List<string> errors)
        {
            return new CustomResponseDto<T> { StatusCode = statusCode, Errors = errors };
        }

        public static CustomResponseDto<T> Fail(int statusCode, string error)
        {
            return new CustomResponseDto<T> { StatusCode = statusCode, Errors = new List<string> { error } };
        }
    }
}

/*
    Herhangi bir class ın içinde static ve geriye new dönen metodlar varsa bunlara Static Factory Metod denir
    Factory D.P. den gelir, Factory D.P. da ayrı class lar veya interface ler oluşturmak yerine direk hangi sınıfı dönmek istiyorsak o sınıfın içerisinde static metodlar tanımlayarak geriye instance lar dönüyoruz    

    Bu sayede new demek yerine direk olarak bu metodları kullanarak nesne üretme olayını ilgili sınıf içeriisnde gerçekleştiriyoruz

    Bu Static Factory Metod D.P. dir. Bu sayede nesne oluşturma işlemini kontrol altına alırız


 */