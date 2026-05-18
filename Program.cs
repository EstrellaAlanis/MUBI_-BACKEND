using Mubi.Api.Config;
using Mubi.Api.Extensions;
using Mubi.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProjectServices(builder.Configuration);
builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// TEMPORAL: comentado para poder ver errores reales de Swagger
// app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// En desarrollo se usa HTTP para evitar bloqueos de certificado entre React y ASP.NET Core.
// app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();
app.Run();