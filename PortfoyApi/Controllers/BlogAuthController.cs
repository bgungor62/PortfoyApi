using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PortfoyApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static PortfoyApi.Dtos.AuthDto;
using static System.Net.WebRequestMethods;

namespace PortfoyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogAuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly SignInManager<AppUser> _signInManager;

        private readonly IConfiguration _configuration;

        public BlogAuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("userLogin")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null)
            {
                return Unauthorized(new { message = "Kullanıcı Bulunamadı." });
            }

            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);

            if (!passwordCheck.Succeeded)
            {
                return Unauthorized(new { message = "Geçersiz e-posta veya parola." });
            }

            var token = GenerateToken(user);
            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRegisterDto dto)
        {
            var exists = await _userManager.FindByEmailAsync(dto.Email);
            if (exists != null)
            {
                return Conflict(new { message = "Bu Mail adresi ile zaten bir kayıt var." });
                // Conflict(409) HTTP durumu döner.
            }
            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);//yeni kullanıcı oluşturma

            if (!result.Succeeded)
            {
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });//kullanıcı oluşturulamazsa hata döner 400
            }

            var token = GenerateToken(user);//işlem başarılı ise token oluşturulur
            return Ok(new { token });

        }



        private object GenerateToken(AppUser user)
        {
            var jwt = _configuration.GetSection("jwtSetting");//appsettings.json dosyasındaki JwtSettings bölümünü alır.

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)); //Güvenlik anahtarı oluşturulur.

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);//İmzalama kimlik bilgileri oluşturulur.


            //token için claimler oluşturma hangi bilgiler token içinde olacak
            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Sub, user.Id),//Subject: Token'ın konusu, genellikle 
                new (JwtRegisteredClaimNames.Email,user.Email ?? ""),//Email: Kullanıcının e-posta adresi
                new (ClaimTypes.Name, user.UserName ?? user.Email ?? ""),//Name: Kullanıcının kullanıcı adı
                new(ClaimTypes.NameIdentifier,user.Id)//NameIdentifier: Kullanıcının benzersiz kimliği
            };

            var expires = DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpiryMinutes"]!));//Token'ın geçerlilik süresi ayarlanır.

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],//Token'ı veren
                audience: jwt["Audience"],//Token'ın hedef kitlesi
                claims: claims,//token içindeki bilgiler
                expires: expires,//tokenın geçerlilik süresi
                signingCredentials: creds//tokenın imzalama bilgileri
                );

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expires,
                Email = user.Email ?? "",
                FullName = user.FullName
            };

        }
    }
}
