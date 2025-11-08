using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortfoyApi.Models
{
    public class Portfoy
    {
        [Key]
        public int PID { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; } = default!;

        [Required, MaxLength(200)]
        public string Slug { get; set; } = default!;

        [Required]
        public string Content { get; set; } = default!;

        [Required]
        public string UserId { get; set; } = default!;

        [ForeignKey(nameof(UserId))]
        public AppUser? User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow!;

        public DateTime? UpdatedAt { get; set; }=DateTime.UtcNow!;

        public bool Status { get; set; } = false; //false aktif silinmemiş yani,true yaparsa listelemede göstermeyeceğiz


    }
}
