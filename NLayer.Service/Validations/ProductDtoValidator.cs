using FluentValidation;
using Nlayer.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Service.Validations
{
    public class ProductDtoValidator : AbstractValidator<ProductDto>  // ProductDto yu validate edicez
    {
        public ProductDtoValidator()
        {
            // Referans tipli değişkenler defaultta null gelir zaten, string te bir referans type tır
            RuleFor(x => x.Name).NotNull().WithMessage("{PropertyName} is required").NotEmpty().WithMessage("{PropertyName} is required"); // Name i Null olmayacak, eğer olursa yazdığımız mesajı göstericez {} placeholder ile bu property nin adını almamızı sağlar
                                                                                                                                           // "{PropertyName}" yazınca FluentValidation buraya direk Name i getirir

            // Value tipli değişkenler için aralık belirtmek gerek 
            RuleFor(x => x.Price).InclusiveBetween(1, int.MaxValue).WithMessage("{PropertyName} must be greater 0");
            RuleFor(x => x.Stock).InclusiveBetween(1, int.MaxValue).WithMessage("{PropertyName} must be greater 0");
            RuleFor(x => x.CategoryId).InclusiveBetween(1, int.MaxValue).WithMessage("{PropertyName} must be greater 0");
        }
    }
}


/* FluentValidation Library

Normalde biz herhangi bir kütüphane kullanamdan validation işlemleirni gerçekleştirebiliyoruz
.Net Framework ile birlikte bize hazır validation lar geliyor
Biz herhangi bir entity ya da dto class ımızda property lerinde [Required], [Range] gibi atribute ler kullanarak validation işlemleri yapabiliyoruz
Yalnız bu atribute lerimizi kullandığımız zaman bu entity ya da dto class larımızı kirletmiş oluyoruz
Mümkün oldukça buraları temiz tutmamız lazım, bu yüzden bu validation işlemini tamamen ayrı bir yerde yapmam gerek

Burada da .Net tarafında en çok kullanılan FluentValidation kütüphanesini kullanıcaz
Biz FluentValidation kullandığımızda API tarafında endpoint e istek yapılınca eğer validation a takılırsa, kendi result ını döner
Ama bizim zaten CustomResponseDto diye bir dönüş modelimiz var, biz FluentValidation nun dönüş modelini kullanmıycaz
CustomResponseDto yu kullanmamız gerek, bunu da bir kod ile sağlıycaz

Kendi modelimizi dönecek bir filter yazıcaz
*/


/*
FluentValidation response olarak kendi modelini dönüyordu, biz bir filter yazarak kendi belirlemiş olduğumuz custom response u döneceğiz

Filterları bizim controller içerisindeki metodlarımıza gelen request e müdahale etmek için kullanıyoruz
Yani bir metodumuza request gelmeden önce, geldikten sonra, Action metod çalışmadan önce ve çalıştıktan sonra , Action metodda sonuç üretilmeden önce ve üretildikten sonra yerlerinde müdahalelerde bulunabiliyoruz.

Öyle bir filter yazıcak ki, bu filter ın eğer bir validation hatası varsa o hatada bizim kendi custom modelimizi dönememize imkan tanıyacak
Bu sayede daha action metoda girmeden biz custom modelimizi dönmüş olacağız






 */