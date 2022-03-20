using MasterDetails.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MasterDetails.Context
{
    public class MyDBContext:DbContext
    {
        public MyDBContext() : base("ProductDB")
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Item> Items { get; set; }
    }
}