using Microsoft.EntityFrameworkCore;
using Nlayer.Core;
using Nlayer.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Repository.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }

        public Task<Category> GetSingleCategoryByIdWithProductsAsync(int categoryId)
        {
            return _context.Categories.Include(x => x.Products).Where(x => x.Id == categoryId).SingleOrDefaultAsync();
        }
    }
}
// SingleOrDefault ve FirstOrDefault Farkı;

// FirstOrDefault dersek ilgili id ye ait 4-5 ürün varsa ilkini bulur

// SingleOrDefault dersek bu id ye ait yani Where(x => x.Id == categoryId) bu artı sağlayan birden fazla satır bulursa hata döner

// Burada id uniq o yüzden SingleOrDefault kullanmak daha mantıklı, çünkü birden fazla ise hata döner ve zaten bu ciddi bir sıkıntıdır.
// Veriden bir tutarsızlık var demektir, geriye bir exception dönsün
