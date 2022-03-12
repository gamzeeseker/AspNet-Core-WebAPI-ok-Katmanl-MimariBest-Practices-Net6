using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nlayer.Core
{
    public class Product : BaseEntity
    {
        /*
        public Product(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));  // ?? eğer bu Name product üretilirken Null olursa NullException hatası fırlat 
        }
        public string Name { get; set; }
        */
        // public string? Name { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; } //Foreignkey dir
        public Category Category { get; set; }
        public ProductFeature ProductFeature { get; set; }
    }
}

// CategoryId yazarsak bunu foreignkey olarak algılar, Category_Id yaparsak ef core bunu anlamaz ve  [ForeignKey] yazmamız gerekir. Id ler içinde böyle 
// Best practice açısından bu standartlarda yap
// String ve class tipte değişkenler referans tiplidir ve onların altında yeşil bir tırtık var, bu nullable konusu ile ilgilidir

/*
 * 
    .Net6 Nullable
    
    C# 8 ile hayatımızı girdi nullable, .Net 5 te default olarak açık değildi ama .Net6 da default olarak açık
    Amaç null exception hatasını engellemektir. 
    Nullable özelliği daha kodlama sırasında bize null olabilecek alanları yeşil tırtık ile belirtir.
    Bu alanlar null olabilirse ya açık açık belirt ya da null değil ise belirt der
    Bunu referans tipli değişkenlerde yapar, Class ve stringler referans tiplidir 
    Value type ların zaten defauşt değeri var, int 0, bool false gibi 

    Sonuna soru işareti koyarak bu değer açık açık null alabilir diyoruz
    Tırtık kırmızı değil yeşil, yani bu olmadan da çalışır ama bize yardımcı olmak istiyor
 

    Eğer bu Name alanı kesin olarak null olmayacaksa ben bunu ctorda her instance üretirlirken veririm ve eğer bu alan boş gelirse bir NullException fırlat derim
 
    Bu Null hatalarını Projenin property kısmından Nullable kısmını Disable edebilriiz
 */

