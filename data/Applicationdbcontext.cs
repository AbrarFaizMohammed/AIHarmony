using AIHarmony.Models;
using Microsoft.EntityFrameworkCore;

namespace AIHarmony.data
{
    public class Applicationdbcontext: DbContext
    {
        public Applicationdbcontext(DbContextOptions<Applicationdbcontext> options) : base(options)
        {            
        }

        public DbSet<Users> Users { get; set; }

        public DbSet<ConfidentialWords> ConfidentialWords { get; set; }
    }
}