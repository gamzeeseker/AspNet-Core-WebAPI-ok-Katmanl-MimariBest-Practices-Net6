using Microsoft.EntityFrameworkCore;
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

namespace NLayer.Service.Services
{
    public class Service<T> : IService<T> where T : class
    {
        private readonly IGenericRepository<T> _repository; // Bununla birlikte db deki işlemlere ulaştık
        private readonly IUnitOfWork _unitOfWork; // Db ye kaydetmek için bunu kulladnık, Repository tarafında SaveChanges yok çünkü
        public Service(IGenericRepository<T> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
        public async Task<T> AddAsync(T entity)
        {
            await _repository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await _repository.AddRangeAsync(entities);
            await _unitOfWork.CommitAsync();
            return entities;
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            return await _repository.AnyAsync(expression);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repository.GetAll().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            // id olup olmadığını yani try cactc kontrollerini service katmanında yapmak en best practice olandır

            var hasProduct = await _repository.GetByIdAsync(id);
            if (hasProduct == null) // Ortak olarak service katmanında bu id ye ait bir ürün var mı diye kontrol ettik, bu sayede Controller içinde tek tek yapmaya gerek kalmadı
            {
                // Buraya dinamik olarak T geliyor, bu yüzden tipini alıyoruz önce
                throw new NotFoundExcepiton($"{typeof(T).Name} ({id}) not found"); // Özelleştirilmiş client a özel biizm hazırladığımız bir exception du bu, 404 yani bulunamadı döneriz
            }
            return hasProduct;
        }

        public async Task RemoveAsync(T entity) // Normalde Generic Repository de bu Remove alanı asenkron değildi ama burada SaveChanges() i CommitAsync ile çağırdık, bu tüzden RemoveRange yaptık metodu da
        {
            _repository.Remove(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoveRangeAsync(IEnumerable<T> entities)
        {
            _repository.RemoveRange(entities);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _repository.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return _repository.Where(expression);
        }
    }
}
