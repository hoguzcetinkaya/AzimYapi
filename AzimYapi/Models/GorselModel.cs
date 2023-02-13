using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AzimYapi.Models
{
    public class GorselModel
    {
        public class Gorsel
        {
            [Key]
            public int Id { get; set; }
            public string GorselTuru { get; set; } // Karosel - Logo
            public string GorselAdi { get; set; }
            public bool Aktif { get; set; }
        }
        
    }
}