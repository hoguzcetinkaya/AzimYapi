using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AzimYapi.Models
{
    public class KategoriModel
    {
        public class Kategori
        {
            [Key]
            public int Id { get; set; }
            public string KategoriAd { get; set; }
        }
       
        
    }
}