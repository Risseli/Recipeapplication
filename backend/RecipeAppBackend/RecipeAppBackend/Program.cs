using Microsoft.EntityFrameworkCore;
using RecipeAppBackend;
using RecipeAppBackend.Data;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient<Seed>();  //Includes the seed for base/example data
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());    //Iclude automapper in project

//Include different interfaces/classes
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Adds DataContext to app and connects it to the database
builder.Services.AddDbContext<DataContext>(options =>
{   //Gets the connectionString from appsettings.json
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();


if (args.Length == 1 && args[0].ToLower() == "seeddata") //for seeding the database
    SeedData(app);

void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<Seed>();
        service.SeedDataContext();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
