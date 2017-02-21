using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using LeadifyTest.Models;

namespace LeadifyTest
{
    public class MainDbContext : DbContext
    {
        public MainDbContext()
            : base("name=DefaultConnection")
        { 
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Contacts> Contacts { get; set; }
    }
}