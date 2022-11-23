﻿using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbContext(DbContextOptions<DbContext> options, ServiceLifetime service) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //User MODEL BUILDER
            //Auto generate ID
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();

            //Beat MODEL BUILDER
            //Auto generate ID
            modelBuilder.Entity<Beat>()
                .Property(w => w.Id)
                .ValueGeneratedOnAdd();
            //Add One To Many Relationship
            modelBuilder.Entity<Beat>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        //Mapping to entity classes
        public DbSet<Beat> _beatEntries { get; set; }
        public DbSet<User> _userEntries { get; set; }
    }
}