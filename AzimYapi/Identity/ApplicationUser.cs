using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzimYapi.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string sifre { get; set; }
    }
}