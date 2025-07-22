using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region edited 
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//builder.Services.AddMvc().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
//builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
var curVersion = new ApiVersion(1, 0);
builder.Services.AddApiVersioning(config =>
{
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.DefaultApiVersion = curVersion;
    //opt.ReportApiVersions = true;
    config.ApiVersionReader = new UrlSegmentApiVersionReader();
    config.ApiVersionReader = ApiVersionReader.Combine(
      new HeaderApiVersionReader("X-Version"),
      new QueryStringApiVersionReader("ver", "version"));
});

var targetFrameworkAttribute = Assembly.GetExecutingAssembly()
                                        .GetCustomAttributes(typeof(System.Runtime.Versioning.TargetFrameworkAttribute), false)
                                        .SingleOrDefault();
string APIPackageVerion = targetFrameworkAttribute == null ? "UnKnown" : (targetFrameworkAttribute as System.Runtime.Versioning.TargetFrameworkAttribute).FrameworkName;
APIPackageVerion = APIPackageVerion + $" - {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version}";

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{    
    app.UseHttpsRedirection();
//app.UseSwagger() and app.UseSwaggerUI() should typically be enabled only in development.In production, these can expose internal API details.
//Move them inside the development block unless you specifically want Swagger in production.

    //app.UseSwagger();
    //app.UseSwaggerUI();
}
else
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    app.Urls.Add($"http://*:{port}");
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();

app.MapControllers();


app.Run();

// EXPOSE 8080
