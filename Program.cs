using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Movies_API.APIBehaviour;
using Movies_API.Filters;
using Movies_API.MovieContext;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ParseBadRequestFilter));
}).ConfigureApiBehaviorOptions(BadRequestBehavior.Parse);

//Add Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy
                          .WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          //.AllowAnyOrigin()
                          .AllowAnyMethod();
                      });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add Postgres
builder.Services.AddDbContext<MovieDBContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("movies")));


//Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

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
app.UseCors(MyAllowSpecificOrigins);

app.UseResponseCaching();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

