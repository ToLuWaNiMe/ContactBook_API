using ContactBookApi;
using ContactBookApi.Models;
using ContactBookApi.Services.IServices;
using ContactBookApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<ContactApiContext>()
    .AddDefaultTokenProviders();
builder.Services.AddDbContext<ContactApiContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration["ConnectionStrings:AppDbContextConnection"]
    );
});
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGeneratorService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });





    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;



})
    .AddJwtBearer(options =>
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["ApiSettings:JwtOptions:Secret"]!));
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            IssuerSigningKey = key,
            ValidIssuer = builder.Configuration["ApiSettings:JwtOptions:Issuer"],
            ValidAudience = builder.Configuration["ApiSettings:JwtOptions:Audience"],
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
builder.Services.AddCors(options => options.AddPolicy(name: "NgOrigins",
    policy =>

    {

        policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();

    }));


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
