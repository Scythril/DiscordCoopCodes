using DiscordCoopCodes.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordCoopCodes.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Coop> Coops { get; set; }
        public DbSet<CoopStatus> CoopStatuses { get; set; }
        public DbSet<DiscordUser> DiscordUsers { get; set; }
        public DbSet<UserCoopXref> UserCoopXrefs { get; set; }


        public ApplicationDbContext() : base(GetOptions())
        {
        }

        public ApplicationDbContext(string connString) : base(GetOptions(connString))
        {
        }

        private static DbContextOptions GetOptions()
        {
            var Configuration = new ConfigurationBuilder()
    .AddUserSecrets<Secrets>()
    .Build();
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), Configuration["ConnectionStrings:DefaultConnection"]).Options;
        }

        private static DbContextOptions GetOptions(string connString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connString).Options;
        }




        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserCoopXref>().HasKey(x => new { x.DiscordUserId, x.CoopId });
        }
    }
}
