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

// builder.Services.AddControllers(); bunun yanýna fluentvalidation ekledik
// builder.Services.AddControllers().AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<ProductDtoValidator>()); //yerini lokasyonunu vermek gerek Validatins larýn
builder.Services.AddControllers(options => options.Filters.Add(new ValidateFilterAttribute())).AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<ProductDtoValidator>()); // artýk global oalrak tüm controllarýmýza bu filter uygulanýr
builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;  // Framework ün kendi dönmüþ olduðu filter ý invalid ettik, FluentValidation ýn döndüðü bir model vardý, onu invalid ettik baskýladýk, kapattýk gibi düþün
});   // MVC tarafýnda böyle bir baskýlama yapmaya gerek yok, API tarafýnda defaultta validatefilter aktif
      // MVC de bu filter aktif deðil. MVC de geriye sayfa dönüyor, hangi sayfayý döncecek bilmiyor ki, API da geriye bir response döncek bunu biliyor 


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMemoryCache();

builder.Services.AddScoped(typeof(NotFoundFilter<>)); // Generic oldduðu için typeof ile içine gireriz ve generic olduðu içinde <> yaptýk



/* 
AutoFac kütüphanesi bir inversion of control yani IOC olarak geçer. Ayný zamanda DI container olarakta geçer
Normalde biz Asp.Net Core projesi oluþturduðumuzda bu framework ün içerisinde Built-in olarak bir DI Container gelir, yani dahil olarak
Yani biz builder.Services. diyerek herhangi bir class ýn ctor unda kullanacaðýmýz interfaceyi ve bu interfaceye karþýlýk gelen class larý ekliyoruz
Built-in DI Container ýn ctor ve metod injection ý var.
Ama AutoFac bu Built-in DI dan daha yetenekli ve ekstra olarak property injection ý var.
Ayný zamanda AutoFac te dinamik olarak servisleri ekleme özelliðimiz var

Built-in DI Container ý kullanýnca Program.cs de çok fazla kod yazýyoruz ve tanýmlama yapýyoruz
AutoFac i Program.cs te enjekte edicez

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)); //Generic olduðu için typeof olarak ekledik, generic olduðu için <> içini boþ býraktýk
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>)); //Generic 1 tane TEntity alýyor, birden çok alsaydý <,> þeklinde yapardýk

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>(); 

 */


builder.Services.AddAutoMapper(typeof(MapProfile)); // istersen Assembly ver istersen typeof ile tip ver diyor, biz tip verdik, bu tipten hangi Assembly de olduðunu bulur zaten


builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"), option =>
    {
        //option.MigrationsAssembly("NLayer.Repository"); // Migration dosyalarýmýz ve repository lerimiz NLayerRepository katmanýnda
        // bu yüzden API tarafýnda bu AppDbContext in hangi assambly de olduðunu söylemem lazým, API da aramasýn diye
        // Ama bu tip güvensiz bir yöntem olursa Repository nin ismi deðiþirse o zaman hata çýkar, dinamik olarak alýcaz bu ismi

        // Bu halde tip güvenli olmuþ oldu
        option.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext)).GetName().Name); // AppContext in olduðu Assembly nin ismini al dedik
    });
});


// Use ExceptionHandler middleware i sayesinde hatalarý global olarak ele alýrýz. Bu middleware uygulamada herhangi bir hata fýrlatýldýðýnda o hatayý yakalar. 
// Biz bu middleware i customize ederek kendi CustomResponseDto nesnemizi dönebiliriz 
// Bu bir MVC uygulamasý ise geriye bir sayfa döneriz, Error sayfasý gibi bir sayfamýz olur, oraya yönlendiririz 

// DI larý tek bir yerden generic olarak yamayý saðlar
builder.Host.UseServiceProviderFactory
    (new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => containerBuilder.RegisterModule(new RepoServiceModule()));


var app = builder.Build();

// app.Use ile baþlayanlar hep middleware dir. Burada yeni bir tane yazarsak kirlenir, dýþarda yazýp burda çaðýrcaz, Extention metod yazýcaz
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCustomException(); // Exception middleware in yukarlarda olmasý daha mantýklý, direk hata varsa geri döner

app.UseAuthorization();

app.MapControllers();

app.Run();
