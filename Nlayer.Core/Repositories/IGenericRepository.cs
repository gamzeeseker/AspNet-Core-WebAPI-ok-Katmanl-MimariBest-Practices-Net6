using System.Linq.Expressions;

namespace Nlayer.Core.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        IQueryable<T> GetAll();
        IQueryable<T> Where(Expression<Func<T, bool>> expression); // Entity alacak ve bool döner, yani x=>x.Id>5 mi der true ise onu da ekler döneceği dataya
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities); // Dikkat et, bir List almıyoruz bir interface aldık
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}

/*

IQueryable<T> Where(Expression<Func<T, bool>> expression);

IQueryable döneriz geriye yani bir List dönmeyiz bunun sebebi 
IQueryable kullandığımızda yazdığımız sorgular direk veritabanına gitmez
ToList, ToListAsync gibi metodları kullanırsak o zaman veritabanına gider
Buradaki amaç üzerine OrderBy yapalım başka sorgular yazalım üzerine

O sorguları bu where i yazdığımız yer ToList dediğimizde veri tabanına gidecek ve daha performanslı bir şekilde çalışabilmek için
Async yapmadık çünkü burada veritabanına bir sorgu yapmıyoruz sadece veritabanına yapılabilecek sorguyu oluşturuyoruz
 
İçine de bir expression tanımlayabiliriz
Amacımız şu;
productRepository.Where(x=>x.Id>5) deriz buradan bize bir IQueryable döner, çünkü veritabanına sorgu yapmadık
 
productRepository.Where(x=>x.Id>5).OrderBy() gibi bir sürü işlem yaparız buna
Ama bunun sonuna ToList ya da ToListAsync dersek o zaman bu db ye sorgu yapar

O yüzden biz bu Where den sonra bir IQueryable alırız ki belki burada OrderBy kullanıcaz

Eğer burada direk List çekseydik gidip bu productRepository.Where(x=>x.Id>5) buradan gelen datayı çekerdi direk sonra bunu memory ye alır ve sonrasında orderby yapar

Ama IQueryable yaparsak Where den sonra gelen OrderBy ı da alır, öyle sorgu atar db ye



Task<bool> AnyAsync(Expression<Func<T, bool>> expression); 
productRepository.Any(x=>x.Id>5) dersek bize ya true ya da false döner


void Update(T entity);
Update ve Remove async değildir. Bunların ef core da async metodları yok.
Ef Core memory ye alıp takip etmiş olduğu bir Product ın sadece state ini değiştiriyor.
O yüzden bu uzun süren bir işlem olmadığı için asenkron bir yapısı yok

void Update(T entity); Yani biz bir şey update etmek için bir entity vermeliyiz ve bu entity memory de ef core tarafından takip ediliyor
Update dediğimizde sadece memory de takip etmiş olduğu entity nin state ini modify olarak değiştiriyor. Yani uzun süren bir işlem değil

Ama Add memory ye bir data ekliyor. Orada bir süreç var, uzun süren bir işlem olduğu için asenkron u var


Task AddRange(IEnumerable<T> entities); Yazılımda mümkün oldukça soyut nesnelerle çalışmak önemli, new ile nesne örneği alınmaz bu yüzden interface kullandık
Biz bunu IEnumerable interfaceini implemente eden herhangi bir class a çevirebiliriz yani cast yapabiliriz

IQueryable<T> GetAll(Expression<Func<T, bool>> expression); geriye IEnumerable dönmeyiz çünkü biz datayı aldıktan sonra
productRepository.GetAll(x=>x.Id>5).OrderBy() Id si 5 ten büyük olanları getir dedik belki sonrasında burada bir OrderBy yaparız diye IQueryable kullandık
Sıralamak istemezsekte ne zaman productRepository.GetAll(x=>x.Id>5).ToList() ya da ToListAsync() dersek ToList() metodu çağırdığımız anda veritabanına sorgu atar

IQueryable dönen bir şeylerde veritabanına sorgu atılmaz. Bunlar memory de bizim yazmış olduğumuz cümlecikler x=>x.Id>5 veya x.Enable(true) olanlar gibi, bunlar memory de birleştirilir ve
tek bir seferde veritabanına gönderilir ne zaman ToList() ya da ToListAsync() gibi metodları çağırırsak



 */