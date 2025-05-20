using ProductApp.BusinessLogic.GrpcServices;
using ProductApp.BusinessLogic.Services;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ProductApiService>();

// Configuración de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración CORS si es necesario
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
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

app.UseEndpoints(endpoints =>
{
    // Mapeo de servicios gRPC
    endpoints.MapGrpcService<ProductGrpcService>();
    // Mapeo de controladores API (si necesitas una API REST adicional)
    endpoints.MapControllers();
});

app.Run();