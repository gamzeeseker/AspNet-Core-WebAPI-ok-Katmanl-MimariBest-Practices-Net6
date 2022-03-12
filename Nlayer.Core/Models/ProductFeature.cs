using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nlayer.Core
{
    public class ProductFeature // Buna BaseEntity vermeme gerek yok, zaten Product a bağlı
    {
        public int Id { get; set; }
        public string Color { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int ProductId { get; set; } // İlişki için onun id  sini tutarız
        public Product Product { get; set; }
    }

    // Product ile birebir bir ilişkisi vardır, Product entitysine ait sütun sayısı artarsa bu entity yi daha fazla büyütmemek için ayrı bir tabloda devamındaki değerleri birebir ilişki ile tutabiliriz
    // Bunun avantajı hem Product Entity büyümemiş olur, hemde db den data çekmek isteyince tüm sütunları Product a yığarsak data çekerken 40 sütun birden gelir, ama bölersek daha az gelir 
    // Her bir Product ın ProductFeature ı olmak zorunda, bu yüzden her bir Product ın oluşturulma tarihi aynı zamanda ProductFeature ın oluşturulma tarihidir
    // BaseEntity kullanmadığımız için ProductFeature a kendi Id sini vermeliyiz
}
