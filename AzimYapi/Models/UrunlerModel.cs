using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AzimYapi.Models
{
    public class UrunlerModel
    {
        public class Urun
        {
            [Key]
            public int Id { get; set; }
            [Required]
            public string UrunAdi { get; set; }
            [Required]
            public int fiyat { get; set; }
            public int stok { get; set; }
            public bool enCokSatan { get; set; }
            public string urunGorselAdi { get; set; }

            public int KategoriId { get; set; }
        }
    }
}