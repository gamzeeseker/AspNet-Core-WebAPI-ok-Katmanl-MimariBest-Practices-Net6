using Microsoft.EntityFrameworkCore;
using Nlayer.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context; // Db ile ilgili işlem yapabilmek için AppDbContext nesnesi tanımladık
                                                  // Product gibi özel Repository lerde bizim bu AppDbContext nesensine erişmemiz gerekiyor
                                                  // Bunu da miras alarak yaparız, protected sadece miras alnınan yerde kullanılabilsin diye yaptık 

        private readonly DbSet<T> _dbSet; // Bizim entitylerimize yanı db deki tablolarımıza karşılık gelir 
                                          // readonly değerlere ya tanımlarken değer atıcaz ya da ctor da değer atıcaz demektir, biz ctor da atıcaz
                                          // ctor da set edilsin sonra başka bir yerde set edilmesin diye readonly yaptık
        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.AnyAsync(expression);
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsNoTracking().AsQueryable(); // AsNoTracking dememizin sebebi Ef Core çektiği dataları memory ye almasınki daha performasnlı çalışsın
                                                        // AsNoTracking kullanmadan 1000 tane data çekersek bu 1000 datayı memory ye alır ve anlık olarak durumlarını Track eder yani izler
                                                        // Ve bu da hem uygulama performansını düşürür, datalar dispose edilene kadar memory de bekler çünkü
                                                        // Burada Update Delete gibi bir işlem yapmıycaz o yüzden AsNoTracking diyerek daha performanslı hale getiririz 
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id); // Find bizden bir id params bekler yani birden fazla id yazabiliriz
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity); // Remove un asenkron metodu yok çünkü bu daha db den silmez, sadece Ef Core bu entity yi Track ediyor ve biz Remove edince bu entitynin state ini deleted olarak işaretliyoruz.
                                   // SaveChanges() metodunu çağırınca Ef Core bu deleted flag lerini bulup onu siliyor

            // _context.Entry(entity).State = EntityState.Deleted;   yukarıdaki Remove ile aynı şey bu, bu şekilde de yapabiliriz 

        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression);
        }
    }
}
