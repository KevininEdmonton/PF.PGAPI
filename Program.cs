using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PFAPI.SupportModels;
using PFAPI.utility;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var Configuration = builder.Configuration;

#region edited 
// AddAuthentication
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidIssuer = Configuration["Application:AppName"],
    ValidateAudience = true,
    ValidAudience = Configuration["JwtIssuerOptions:Audience"],
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AuthSettings:SecretKey"])),
    RequireExpirationTime = false,
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
};
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(configureOptions =>
{
    configureOptions.ClaimsIssuer = Configuration["Application:AppName"];
    configureOptions.TokenValidationParameters = tokenValidationParameters;
    configureOptions.SaveToken = true;

    configureOptions.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

List<string> theOperationlist = Policy4ModuleOperations.GetAllOperationList();

// api user claim policy
builder.Services.AddAuthorization(options =>
{
    foreach (string theOP in theOperationlist)
        options.AddPolicy(theOP, policy => policy.RequireClaim(theOP));

});

builder.Services.AddCors(cfg =>
{
    cfg.AddPolicy("pfapi", bldr =>
    {
        bldr.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("https://pfapi-697989298692.us-west1.run.app/");
    });

    cfg.AddPolicy("pastfuture", bldr =>
    {
        bldr.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("https://pastfuture-web-697989298692.us-west1.run.app/");
    });

    cfg.AddPolicy("AnyGet", bldr =>
    {
        bldr.AllowAnyHeader()
            .WithMethods("GET")
            .AllowAnyOrigin();
    });

    cfg.AddPolicy("OpenAPIPolicy", bldr =>
    {
        bldr.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });

});


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

builder.Services.AddSwaggerGen(
               setupAction =>
               {
                   #region swagger 5.0.1 +
                   setupAction.SwaggerDoc(
                           $"v1",
                           new Microsoft.OpenApi.Models.OpenApiInfo()//Swashbuckle.AspNetCore.Swagger.Info() //Microsoft.OpenApi.Models.OpenApiInfo()
                           {
                               Title =  "PF API",    // Configuration["Application:AppName"],
                               Version = curVersion.ToString(),
                               Description = APIPackageVerion,
                               Contact = new Microsoft.OpenApi.Models.OpenApiContact() //Swashbuckle.AspNetCore.Swagger.Contact()// Microsoft.OpenApi.Models.OpenApiContact()
                               {
                                   Email = "kevin.shen@live.ca",
                                   Name = "Kevin S",
                                   Url = new Uri("https://pastfuture-web-697989298692.us-west1.run.app/")
                               },
                               License = new Microsoft.OpenApi.Models.OpenApiLicense()//Swashbuckle.AspNetCore.Swagger.License()// Microsoft.OpenApi.Models.OpenApiLicense()
                               {
                                   Name = "MIT License",
                                   Url = new Uri("https://opensource.org/licenses/MIT")
                               }
                           });

                   var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                   var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
                   setupAction.IncludeXmlComments(xmlCommentsFullPath);

                   setupAction.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                   {
                       Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                       In = ParameterLocation.Header,
                       Name = "Authorization",
                       Type = SecuritySchemeType.ApiKey
                   });

                   setupAction.OperationFilter<SecurityRequirementsOperationFilter>();

                   #endregion
               }
           );

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{    
    app.UseHttpsRedirection();
}
else
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    app.Urls.Add($"http://*:{port}");
}
app.UseSwagger(c =>
{
    c.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0; 
});
//app.UseSwaggerUI();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("./v1/swagger.json", "My API V1");
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();

// EXPOSE 8080
