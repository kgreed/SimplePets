using System;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using SimplePets.Module.BusinessObjects;
namespace SimplePets.Module.Win.Controllers
{
    public static class Helpers
    {
        
        public static SimplePetsEFCoreDbContext MakeDb()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            var optionsBuilder = new DbContextOptionsBuilder<SimplePetsEFCoreDbContext>()
                .UseSqlServer(connectionString);
            return new SimplePetsEFCoreDbContext(optionsBuilder.Options);
        }
    }
}
