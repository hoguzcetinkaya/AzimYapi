using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzimYapi.Models.ViewModels
{
    public class UrunKategori
    {
        public IEnumerable<KategoriModel.Kategori> Kategori{ get; set; }
        public IEnumerable<UrunlerModel.Urun> Urun { get; set; }
    }
}