var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
