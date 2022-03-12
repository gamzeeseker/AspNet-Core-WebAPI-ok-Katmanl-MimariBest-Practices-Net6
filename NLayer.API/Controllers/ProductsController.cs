using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nlayer.Core;
using Nlayer.Core.DTOs;
using Nlayer.Core.Services;
using NLayer.API.Filters;

namespace NLayer.API.Controllers
{
    public class ProductsController : CustomBaseController
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        public ProductsController(IMapper mapper,
            IProductService productService)
        {
            _mapper = mapper;
            _productService = productService;
        }

        // All metodu ile çakışmasın diye bunu çağırken metod ismi de ekleriz url e
        // GET api/products/GetProducsWithCategory
        // [HttpGet("[action]")] bunu da diyebiliriz, bu direk metod ismini alır zaten
        // [HttpGet("{GetProducsWithCategory}")]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetProducsWithCategory()
        {
            return CreateActionResult(await _productService.GetProductsWithCategory());
        }

        // GET api/products
        [HttpGet]
        public async Task<IActionResult> All()
        {
            var products = await _productService.GetAllAsync();  // Cache ten category leri ile birlikte product lar buraya gelecek, ama ProductDto ya çevrilirken bu categoryler ayrılacak, basılmayacak çünkü ProductDto da category ler yok
            var productsDtos = _mapper.Map<List<ProductDto>>(products).ToList(); //GetAllAsync() den IEnumerable dönüyor, o yüzden listeye çevirdik 

            //return Ok(CustomResponseDto<List<ProductDto>>.Success(200, productsDtos)); // Burada hem 200 hem ok dönüyoruz, bu çok çirkin oldu. Bunu CustomBaseControllerda yapalım en iyisi
            //return CreateActionResult<List<ProductDto>>(CustomResponseDto<List<ProductDto>>.Success(200, productsDtos)); içinde tipini verdik diye generic kısımda vermeye gerek kalamdı

            return CreateActionResult(CustomResponseDto<List<ProductDto>>.Success(200, productsDtos));
        }

        // NotFoundFilter ı [ValidateFilter] şeklinde yazamam
        // NotFoundFilter bir attribute class ını miras almıyor ama ValidateFilter miras alıyor
        // Eğer bir filter ın ctor unda bir parametre geçiyorsak dite [] içinde kullanamayız 
        // Bunun yerine ServiceFilter üzerinde kullanmamız gerek
        [ServiceFilter(typeof(NotFoundFilter<Product>))] // Action metoda girmeden çalışır ve id ye ait ürün yoksa direk hata döner 
        // GET /api/products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            var productsDto = _mapper.Map<ProductDto>(product);
            return CreateActionResult(CustomResponseDto<ProductDto>.Success(200, productsDto));
        }

        [HttpPost]
        public async Task<IActionResult> Save(ProductDto productDto)
        {
            var product = await _productService.AddAsync(_mapper.Map<Product>(productDto));
            var productsDto = _mapper.Map<ProductDto>(product);
            return CreateActionResult(CustomResponseDto<ProductDto>.Success(201, productsDto));
        }

        [HttpPut]
        public async Task<IActionResult> Update(ProductUpdateDto productDto)
        {
            await _productService.UpdateAsync(_mapper.Map<Product>(productDto));
            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204)); // geriye bir data dönmiycez diye NoContentDto kullandık
        }

        // GET /api/products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var product = await _productService.GetByIdAsync(id); // bu product var mı yok mu bunu exception yazarak kontrol edicez
            await _productService.RemoveAsync(product);
            var productsDto = _mapper.Map<ProductDto>(product);
            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204));
        }


    }
}
