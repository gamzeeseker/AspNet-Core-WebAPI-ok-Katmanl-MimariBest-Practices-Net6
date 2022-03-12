using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Nlayer.Core.Services
{
    public interface IService<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        //IQueryable<T> GetAll(Expression<Func<T, bool>> expression); sırf fark olsun görelim diye burada GetAll u değiştirdik
        Task<IEnumerable<T>> GetAllAsync(); // Dönüş tipini IEnumerable yaptık ve tüm datayı çektik. Üstelik Async de yapmış olduk
        IQueryable<T> Where(Expression<Func<T, bool>> expression); //bunu asenkron yapmadık çünkü bir IQueryble dönücez ve bunun veritabanına yansımasını bu servisi çağıran kodda ToList veya ToListAsync çağırarak yapıcaz
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
        Task<T> AddAsync(T entity); // AddAsync ve AddRangeAsync metodları Generic Repository tarafında geriye bir şey dönmüyordu, ama servis katmanında SaveChanges yaparız UnitOfWork ile
                                    // bu sayede geriye dönülebilecek bir id ye sahip yani db de olan ürün vardır ve geri dönebiliriz onu
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

        // Ef Core un Update ve Remove için asenkron metodları yoktu ama IService de biz bu değişiklikleri veritabanına yansıtacakğız
        // Orada SaveChanges metodunu kullanacağımız için Void leri Task yaparız
        Task UpdateAsync(T entity);
        Task RemoveAsync(T entity);
        Task RemoveRangeAsync(IEnumerable<T> entities);
    }
}

/*
 
    Burada da IGenericRepository ile aynı metodlar olacak ancak burada dönüş tiplerimizde değişiklikler olacak
    Buradaki amaç şu, Repository katmanından aldığımız datayı işleyerek biz mapping yapabiliriz, başka bir şey yapabiliriz
    Business kod burada dönüyor çünkü, motedların aynı olması ya da farklı olması önemli değil.
    Best Practice açısından aynı şeyleri ekledik buraya, ilerde IProductRepository yaptığımzda onun dönüş tipleri ile IProductService nin dönüş tipleri aynı olmayacak
    IService de generic olduğu için dönüş tipi IGenericRepository ile aynı olması normal, burada kodu duplicate ediyor gibi düşünme

 */

