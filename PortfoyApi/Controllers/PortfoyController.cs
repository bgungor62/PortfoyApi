using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfoyApi.Data;
using PortfoyApi.Models;
using PortfoyApi.Utils;
using System.Security.Claims;
using static PortfoyApi.Dtos.PortfoyDto;

namespace PortfoyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfoyController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PortfoyController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ResponsePortfoyDto>> PortfoyCreate([FromBody] CreatePortfoyDto dto)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);//token içinden kullanıcı id sini alıyoruz

            var baseSlug = SlugHelper.ToSlug(dto.Title);
            var slug = baseSlug;
            int i = 2;
            while (await _context.Portfoys.AnyAsync(x => x.Slug == slug))
            {
                slug = $"{baseSlug}-{i}";
                i++;
            }
            var entity = new Portfoy
            {
                Title = dto.Title,
                Content = dto.Content,
                Slug = slug,
                Status = dto.Status,
                UserId = uid,
                CreatedAt = DateTime.UtcNow
            };

            _context.Portfoys.Add(entity);
            _context.SaveChangesAsync();

            var response = new ResponsePortfoyDto
            {
                PID = entity.PID,
                Title = entity.Title,
                Slug = entity.Slug,
                Content = entity.Content,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                UserId = entity.UserId
            };

            return CreatedAtAction(nameof(GetBySlug), new { slug = entity.Slug }, response);
        }


        [HttpGet("{slug}")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponsePortfoyDto>> GetBySlug(string slug)
        {
            var b = await _context.Portfoys.Include(x => x.User).FirstOrDefaultAsync(x => x.Slug == slug);

            if (b is null) return NotFound();//slug e göre portföy bulunamadı

            if (!b.Status)
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (uid != b.UserId) return Forbid(); //başka bir kullanıcı aktif olmayan portföye erişmeye çalışıyor
            }
            return Ok(new ResponsePortfoyDto
            {
                PID = b.PID,
                Title = b.Title,
                Slug = b.Slug,
                Content = b.Content,
                UserId = b.UserId,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt,
                Status = b.Status,
                UserEmail = b.User?.Email,
                UserFullName = b.User?.FullName
            });

        }
    }
}
