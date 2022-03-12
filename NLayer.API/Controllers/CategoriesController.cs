using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nlayer.Core.Services;
using NLayer.API.Filters;

namespace NLayer.API.Controllers
{
    // [ValidateFilterAttribute] // buraya ekledik ki tüm action metodlar valide edilsin, bunu her controller için yapmamız gerekecek, ancak daha kolay bir yöntemi var ve global olarak ekleriz
    // Global şeyler Program.cs de yapılır
    public class CategoriesController : CustomBaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // api/categories/GetSingleCategoryByIdWithProducts/2
        [HttpGet("[action]/{categoryId}")] // {} kullandık çünkü bu bir parametredir
        public async Task<IActionResult> GetSingleCategoryByIdWithProducts(int categoryId)
        {
            return CreateActionResult(await _categoryService.GetSingleCategoryByIdWithProductsAsync(categoryId));
        }
    }
}
