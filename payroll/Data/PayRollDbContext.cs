using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace payroll.Data
{
    public class PayRollDbContext : IdentityDbContext<IdentityUser>
    {
        public PayRollDbContext(DbContextOptions<PayRollDbContext> options) : base(options)  
        {
        }
    }
}
