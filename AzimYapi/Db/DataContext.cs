using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AzimYapi.Db
{
    public class DataContext : DbContext
    {
        public DataContext() : base("dbConnection")
        {

        }

        public DbSet<Models.UrunlerModel.Urun> Urun { get; set; }
        public DbSet<Models.KategoriModel.Kategori> Kategori { get; set; }
        public DbSet<Models.GorselModel.Gorsel> Gorsel{ get; set; }


    }
}