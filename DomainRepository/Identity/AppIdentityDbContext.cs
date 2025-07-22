using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DomainRepository.Identity
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        private readonly IConfiguration _config;
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options, IConfiguration config)
            : base(options)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_config.GetConnectionString("PFConnectionString"));
        }
    }
}
