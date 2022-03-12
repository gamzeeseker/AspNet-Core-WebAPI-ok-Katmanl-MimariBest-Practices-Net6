using Nlayer.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nlayer.Core.Services
{
    public interface IProductService : IService<Product>
    {
        Task<CustomResponseDto<List<ProductWithCategoryDto>>> GetProductsWithCategory(); // Burada artık özelleştirilmiş bir Dto dönmemiz gerek, çünkü özel db den direk çekilen bir kısım değil
    }
}


// Repository ler geriye entity dönerken servisler geriye direk olarak API ın isteyeceği Dto yu döner