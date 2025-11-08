using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PortfoyApi.Data;
using PortfoyApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// veritabaný baðlantýsý ekledik
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection")));


// kimlik doðrulama için Entity Framework kullanmasýný saðladýk
builder.Services.AddIdentityCore<AppUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;

}).AddEntityFrameworkStores<AppDbContext>().AddSignInManager();

//JWT AYARLARI

var jwt = builder.Configuration.GetSection("jwtSetting");


// güvenlik anahtarýný aldýk ve byte dizisine çevirdik
//neden byte dizisi: çünkü güvenlik anahtarlarý genellikle byte dizisi olarak iþlenir ve bu, þifreleme algoritmalarýnýn gereksinimlerine daha uygun bir formattýr.
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //token'ýn geçerli bir issuer (yani token'ý oluþturan sunucu) tarafýndan oluþturulup oluþturulmadýðýný doðrular
            ValidateIssuer = true,
            //token'ýn geçerli bir audience (yani token'ýn hedeflendiði kullanýcý veya sistem) için oluþturulup oluþturulmadýðýný doðrular
            ValidateAudience = true,
            ValidateLifetime = true,//token'ýn süresinin dolup dolmadýðýný doðrular
            ValidateIssuerSigningKey = true,//token'ýn imzasýnýn geçerli bir imza anahtarý ile oluþturulup oluþturulmadýðýný doðrular
            ValidIssuer = jwt["Issuer"],//token'ýn geçerli issuer deðeri
            ValidAudience = jwt["Audience"],//token'ýn geçerli audience deðeri
            IssuerSigningKey = key, //token'ýn imzasýný doðrulamak için kullanýlan güvenlik anahtarý
            ClockSkew = TimeSpan.Zero //token süresinin dolma süresini sýfýrladýk
        };
    });


builder.Services.AddCors(o =>
{
    o.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:3000", "http://localhost:5053", "https://localhost:7116", "http://localhost:5173") //sadece bu adresten gelen isteklere izin verdik)
    .AllowCredentials()//kimlik doðrulama bilgilerine izin verdik
    .AllowAnyMethod()//herhangi bir HTTP metoduna izin verdik
    .AllowAnyHeader()//herhangi bir HTTP baþlýðýna izin verdik
    );
});




// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    var scheme = new OpenApiSecurityScheme
    {
        Name = "Authentication",
        Type = SecuritySchemeType.Http,//http türünde bir güvenlik þemasý local için
        Scheme = "bearer",//bearer token kullanýyoruz
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer {token}",
        //güvenlik þemasýna referans verdik
        Reference = new OpenApiReference
        {
            Type=ReferenceType.SecurityScheme,
            Id= "Bearer"
        }

    };
    x.AddSecurityDefinition("Bearer", scheme); //güvenlik tanýmýný ekledik
    x.AddSecurityRequirement(new OpenApiSecurityRequirement { { scheme, Array.Empty<string>() } }); //güvenlik gereksinimini ekledik

});
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors();
app.UseAuthentication();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
