using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Bater_Ponto.Models;
using Bater_Ponto.Identity;

namespace Bater_Ponto.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<RegistroPonto> RegistrosPonto { get; set; }
    }
}