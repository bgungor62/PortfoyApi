using PortfoyApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortfoyApi.Dtos
{
    public class PortfoyDto
    {
        public class CreatePortfoyDto
        {
            public string Title { get; set; } = default!;

            public string Content { get; set; } = default!;

            public bool Status { get; set; } = false; //false aktif silinmemiş yani,true yaparsa listelemede göstermeyeceğiz
        }

        public class ResponsePortfoyDto
        {
            public int PID { get; set; }
            public string Title { get; set; } = default!;
            public string Slug { get; set; } = default!;
            public string Content { get; set; } = default!;
            public string UserId { get; set; } = default!;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow!;
            public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow!;
            public bool Status { get; set; } = false;
            public string? UserEmail { get; set; }
            public string? UserFullName { get; set; }
        }
    }
}
