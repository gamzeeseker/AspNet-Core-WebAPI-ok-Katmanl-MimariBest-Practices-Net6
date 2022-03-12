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
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Product>> GetProductsWithCategory()
        {
            return await _context.Products.Include(x => x.Category).ToListAsync(); //protected olduğu için geldi bu
                                                                                   // Include ile eager loading yaptık. Yani daha datayı çekerken categorylerinin de gelmesini istedik
                                                                                   // Product a bağla category yi de ihtiyaca bağlı sonradan çekersen bu da alzy loading olur
        }
    }
}
