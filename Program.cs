using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Movies_API.MovieContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add Postgres
builder.Services.AddDbContext<MovieDBContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("movies")));

//Add Cors
builder.Services.AddCors(options =>
{
    var moviesAppUrl = builder.Configuration.GetValue<string>("MoviesApp");
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(moviesAppUrl).AllowAnyMethod().AllowAnyHeader();
    });
});



//Add Repository


//Add Filters


//Add Caching
builder.Services.AddResponseCaching();


//Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Add Cors
app.UseCors();

app.UseResponseCaching();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

