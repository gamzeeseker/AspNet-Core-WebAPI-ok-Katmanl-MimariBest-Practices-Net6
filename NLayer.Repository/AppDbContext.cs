using Microsoft.EntityFrameworkCore;
using Nlayer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Repository
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) // ctor bir DbContextOptions alacak çünkü biz bu options ile beraber veritabanı yolunu startup ta vereceğiz
                                                                                    // bu kim için olacak AppDbContext için olacak bu yüzden <AppDbContext> yazdık
                                                                                    // ardından bunu base(options) ile base deki options a gönderiyoruz yani DbContext e gönderiyoruz
                                                                                    // çünkü :DbContext ten miras aldık ve ctor unda bu bir options yani veritabanı yolu istiyor
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductFeature> ProductFeatures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Her ClassLibrary bir Assembly dir, bu metod ile tüm configuration ları tüm Assembly lerden okur çünkü hepsi IEntityTypeConfiguration ı implemente ediyor
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); // Çalıştığın Assembly yi tara dedik

            // ProductFeature ı farklılık olsun diye buradan ekledik

            modelBuilder.Entity<ProductFeature>().HasData(new ProductFeature()
            {
                Id = 1,
                Color = "Kırmızı",
                Height = 100,
                Width = 200,
                ProductId = 1
            },
            new ProductFeature()
            {
                Id = 2,
                Color = "Mavi",
                Height = 300,
                Width = 500,
                ProductId = 2
            });

            base.OnModelCreating(modelBuilder);
        }

        /*
        ProductFeature Product ile ilgili, ProductFeature ı DbSet olarak eklersek bağımsız olarak ProductFeature satırlarını bağımsız olarak db ye ekleyebiliriz
        Bunu kapatırız ve eğer biri ProductFeature eklemek istiyorsa mutlaka Product nesnesi üzerinden eklesin diyebiliriz
        
        Eğer senaryomuz u şekilde ise, biz bağımsız olarak ProductFeature ekeleyemeyelim ve mutlaka Product üzerinden ekleyelim ise biz bunu DbSet e kapatırız ve 

        var product= new Product(){ ProductFeature =new ProductFeature(){}} diyerek ekleyebiliriz


        Gerçek dünyada ProductFeature ın Product üzerinden ilem görmesi daha best practice dir ama öğrenme aşamsında olduğumuz için burada açtık bunu

         */



    }
}
