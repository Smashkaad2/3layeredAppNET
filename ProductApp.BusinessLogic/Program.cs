using ProductApp.BusinessLogic.Models;
using ProductApp.BusinessLogic.Services;
using ProductApp.BusinessLogic.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddHttpClient();
builder.Services.AddScoped<IProductRepository, ProductApiRepository>();
builder.Services.AddScoped<ApiProductService>();// Solo registrar como Scoped, no como Singleton
builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<GrpcProductService>();

// Configuración de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Middleware y pipeline de solicitudes
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");
app.MapGrpcService<ProductGrpcServiceImpl>();
app.MapGet("/", () => "Esta es una aplicación gRPC. Cliente gRPC requerido para comunicarse.");

app.Run();