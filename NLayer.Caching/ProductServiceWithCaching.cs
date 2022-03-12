using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Nlayer.Core;
using Nlayer.Core.DTOs;
using Nlayer.Core.Repositories;
using Nlayer.Core.Services;
using Nlayer.Core.UnitOfWorks;
using NLayer.Service.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Caching
{
    // Önceden IProductServis i implemente eden bir ProductServis vardı. Ama artık ProductServiceWithCaching var. 
    // DI Container a sen herhangi bir class ın ctor unda IProductServis görürsen artık ProductServis i değil ProductServiceWithCaching class ından bir nesne örneği al dememiz lazım.
    // Çünkü artık ilk cache i kontrol etmeli
    public class ProductServiceWithCaching : IProductService  // Var olan yapıyı bozmamak için Product ta çalışcaz diye IProductService i implemente edicez.
                                                              // Burada decorator ya da proxy design pattern yapısını kullanmış olduk. Bu 2 design pattern birbirine çok benziyor 
    {
        // Biz Product ları cache liycez. InMemory Caching kullanıcaz yani, dataları uygulamamız nerede host ediliyorsa o host un memory sini kullanacak
        // Burada Redis falan kullanmıycaz direk InMemory cache kullancaz
        // Clienttan bize bir istek geldiğinde önce cache te var mı yok mu onu kontrol edicez, eğer cache te varsa cache ten dönücez
        // Eğer cache te yoksa, datayı repodan çekip cache leyip öyle dönücez

        // Bu kodlamayı yaparken var olan kodlaqrımızda hiç bir değişiklik yapmayacağız, ama cımız zaten var olan kodları bozmamak ve sürdürülebilir kod yazmak

        // Servis katmanı ile aynı seviyede ve servis katmanının özellikleirni kullanabilmesi için onu referans alması gerek

        // Var olan yapıyı bozmuyoruz ama yeni özellikte ekleyebiliyoruz, SOLID in O su bu
        // In Memory cache te key value şeklindecache lerimizi tutuyoruz 
        // Program.cs de de MemoryCache i ekle

        private const string CacheProductKey = "productsCache"; // key bu 
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly IProductRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductServiceWithCaching(IUnitOfWork unitOfWork, IProductRepository repository, IMemoryCache memoryCache, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _memoryCache = memoryCache;
            _mapper = mapper;

            if (!_memoryCache.TryGetValue(CacheProductKey, out _)) // TryGetValue bool döner, eğer bizim belirttiğimiz CacheProductKey e ait bir cache varsa geriye true yoksa false döner.
                                                                   // Out ile de cache te tuttuğu datayı döner, _ ile memory de boş yere datayı tutmasını engelledik, biz sadece true false olmasına bakıcaz
            {
                _memoryCache.Set(CacheProductKey, _repository.GetProductsWithCategory().Result); // Eğer cache boşsa tüm datayı al ve doldur cache i dedik
                                                                                                 // Product larle birlikte Category lerini de cache ledim
                                                                                                 // ctor da asenkron metod kullanamayız, Result() yapıp senkron hale getirdik
            }
        }

        // En iyi cache adayı olan data; sık değiştirmeyeceğimiz ama sık olarak da erişmemiz gereken bir data olmalıdır
        public async Task<Product> AddAsync(Product entity)
        {
            await _repository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
            return entity;
        }

        public async Task<IEnumerable<Product>> AddRangeAsync(IEnumerable<Product> entities)
        {
            await _repository.AddRangeAsync(entities);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
            return entities;
        }

        public Task<bool> AnyAsync(Expression<Func<Product, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {

            var products = _memoryCache.Get<IEnumerable<Product>>(CacheProductKey);
            return Task.FromResult(products);
        }

        public Task<Product> GetByIdAsync(int id) // await olmadığı için async de kullanmaya gerek yok
        {
            var product = _memoryCache.Get<List<Product>>(CacheProductKey).FirstOrDefault(x => x.Id == id);

            if (product == null)
            {
                throw new NotFoundExcepiton($"{typeof(Product).Name}({id}) not found");
            }

            return Task.FromResult(product); // await kullanmadığım yerlerde geriye Task bekliyorsa Task.FromResult kullanırız
        }

        public Task<CustomResponseDto<List<ProductWithCategoryDto>>> GetProductsWithCategory()
        {
            var products = _memoryCache.Get<IEnumerable<Product>>(CacheProductKey);

            var productsWithCategoryDto = _mapper.Map<List<ProductWithCategoryDto>>(products);

            return Task.FromResult(CustomResponseDto<List<ProductWithCategoryDto>>.Success(200, productsWithCategoryDto));
        }

        public async Task RemoveAsync(Product entity)
        {
            _repository.Remove(entity);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync(); // entity yi silince yendien cache i çağırdık, çünkü yenilesin ve cache deki o datayı temizlesin diye
        }

        public async Task RemoveRangeAsync(IEnumerable<Product> entities)
        {
            _repository.RemoveRange(entities);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
        }

        public async Task UpdateAsync(Product entity)
        {
            _repository.Update(entity);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
        }

        public IQueryable<Product> Where(Expression<Func<Product, bool>> expression)
        {
            return _memoryCache.Get<List<Product>>(CacheProductKey).Where(expression.Compile()).AsQueryable(); // expression ı Compile() ile Func a çeviriyoruz
        }

        public async Task CacheAllProductsAsync()  // Otomatik olarak cache leme işlemi yapmamızı sağlar
        {
            _memoryCache.Set(CacheProductKey, await _repository.GetAll().ToListAsync()); // tüm datayı tekrar çek ve cache yap

        }

    }
}
