using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace O2.Identity.Web.Data
{
    public class InitDb
    {
        public static void PreparaDb(ApplicationDbContext context)
        {
            context.Database.Migrate();
        }
    }
}
