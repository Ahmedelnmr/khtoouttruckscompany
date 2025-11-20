using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Infrastructure.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // IMPORTANT: نفس connection string اللي في appsettings.json
            builder.UseSqlServer("Server=.;Database=KhutootDB;Trusted_Connection=True;TrustServerCertificate=True;");

            return new ApplicationDbContext(builder.Options);
        }
    }
}
