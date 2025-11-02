using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace PortfoyApi.Data
{
    public class AppDbContext: IdentityDbContext<AppUser>
    {
    }
}
