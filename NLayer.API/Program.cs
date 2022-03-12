using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Nlayer.Core.Repositories;
using Nlayer.Core.Services;
using Nlayer.Core.UnitOfWorks;
using NLayer.API.Filters;
using NLayer.API.Middlewares;
using NLayer.API.Modules;
using NLayer.Repository;
using NLayer.Repository.Repositories;
using NLayer.Repository.UnitOfWork;
using NLayer.Service.Services;
using NLayer.Service.Services.Mapping;
using NLayer.Service.Validations;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// builder.Services.AddControllers(); bunun yan�na fluentvalidation ekledik
// builder.Services.AddControllers().AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<ProductDtoValidator>()); //yerini lokasyonunu vermek gerek Validatins lar�n
builder.Services.AddControllers(options => options.Filters.Add(new ValidateFilterAttribute())).AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<ProductDtoValidator>()); // art�k global oalrak t�m controllar�m�za bu filter uygulan�r
builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;  // Framework �n kendi d�nm�� oldu�u filter � invalid ettik, FluentValidation �n d�nd��� bir model vard�, onu invalid ettik bask�lad�k, kapatt�k gibi d���n
});   // MVC taraf�nda b�yle bir bask�lama yapmaya gerek yok, API taraf�nda defaultta validatefilter aktif
      // MVC de bu filter aktif de�il. MVC de geriye sayfa d�n�yor, hangi sayfay� d�ncecek bilmiyor ki, API da geriye bir response d�ncek bunu biliyor 


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMemoryCache();

builder.Services.AddScoped(typeof(NotFoundFilter<>)); // Generic olddu�u i�in typeof ile i�ine gireriz ve generic oldu�u i�inde <> yapt�k



/* 
AutoFac k�t�phanesi bir inversion of control yani IOC olarak ge�er. Ayn� zamanda DI container olarakta ge�er
Normalde biz Asp.Net Core projesi olu�turdu�umuzda bu framework �n i�erisinde Built-in olarak bir DI Container gelir, yani dahil olarak
Yani biz builder.Services. diyerek herhangi bir class �n ctor unda kullanaca��m�z interfaceyi ve bu interfaceye kar��l�k gelen class lar� ekliyoruz
Built-in DI Container �n ctor ve metod injection � var.
Ama AutoFac bu Built-in DI dan daha yetenekli ve ekstra olarak property injection � var.
Ayn� zamanda AutoFac te dinamik olarak servisleri ekleme �zelli�imiz var

Built-in DI Container � kullan�nca Program.cs de �ok fazla kod yaz�yoruz ve tan�mlama yap�yoruz
AutoFac i Program.cs te enjekte edicez

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)); //Generic oldu�u i�in typeof olarak ekledik, generic oldu�u i�in <> i�ini bo� b�rakt�k
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>)); //Generic 1 tane TEntity al�yor, birden �ok alsayd� <,> �eklinde yapard�k

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>(); 

 */


builder.Services.AddAutoMapper(typeof(MapProfile)); // istersen Assembly ver istersen typeof ile tip ver diyor, biz tip verdik, bu tipten hangi Assembly de oldu�unu bulur zaten


builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"), option =>
    {
        //option.MigrationsAssembly("NLayer.Repository"); // Migration dosyalar�m�z ve repository lerimiz NLayerRepository katman�nda
        // bu y�zden API taraf�nda bu AppDbContext in hangi assambly de oldu�unu s�ylemem laz�m, API da aramas�n diye
        // Ama bu tip g�vensiz bir y�ntem olursa Repository nin ismi de�i�irse o zaman hata ��kar, dinamik olarak al�caz bu ismi

        // Bu halde tip g�venli olmu� oldu
        option.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext)).GetName().Name); // AppContext in oldu�u Assembly nin ismini al dedik
    });
});


// Use ExceptionHandler middleware i sayesinde hatalar� global olarak ele al�r�z. Bu middleware uygulamada herhangi bir hata f�rlat�ld���nda o hatay� yakalar. 
// Biz bu middleware i customize ederek kendi CustomResponseDto nesnemizi d�nebiliriz 
// Bu bir MVC uygulamas� ise geriye bir sayfa d�neriz, Error sayfas� gibi bir sayfam�z olur, oraya y�nlendiririz 

// DI lar� tek bir yerden generic olarak yamay� sa�lar
builder.Host.UseServiceProviderFactory
    (new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => containerBuilder.RegisterModule(new RepoServiceModule()));


var app = builder.Build();

// app.Use ile ba�layanlar hep middleware dir. Burada yeni bir tane yazarsak kirlenir, d��arda yaz�p burda �a��rcaz, Extention metod yaz�caz
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCustomException(); // Exception middleware in yukarlarda olmas� daha mant�kl�, direk hata varsa geri d�ner

app.UseAuthorization();

app.MapControllers();

app.Run();
