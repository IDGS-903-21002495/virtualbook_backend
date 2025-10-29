using Microsoft.EntityFrameworkCore;

namespace virtualbook_backend.Data
{
    public class VirtualBookDbContext : DbContext
    {
        public VirtualBookDbContext()
        {
        }
        public VirtualBookDbContext(DbContextOptions<VirtualBookDbContext> options) : base(options)
        {
        }
    }
}
