using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PortfoyApi.Models
{
    public class AppUser : IdentityUser
    {
        [Required, MaxLength(100)]
        public string? FullName { get; set; }

        [MaxLength(100)]
        public string? Section { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
