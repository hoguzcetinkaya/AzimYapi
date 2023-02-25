using AzimYapi.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzimYapi.Db
{
    public class IdentityContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityContext() : base("dbConnection")
        {

        }
    }
}