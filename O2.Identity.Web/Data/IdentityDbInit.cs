﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using O2.Identity.Web.Models;

namespace O2.Identity.Web.Data
{
    public class IdentityDbInit
    {
        //This example just creates an Administrator role and one Admin users
        public static async void Initialize(
            ApplicationDbContext context,
            UserManager<O2User> userManager)
        {
            //create database schema if none exists
            // _context.Database.EnsureCreated();
            context.Database.Migrate();
            //If there is already an Administrator role, abort
            //  if (context.Roles.Any(r => r.Name == "Administrator")) return;

            //Create the Administartor Role
            // await roleManager.CreateAsync(new IdentityRole("Administrator"));
            if (context.Users.Any(r => r.UserName == "demo@o2bionics.com")) return;
            //Create the default Admin account and apply the Administrator role
            string user = "demo@o2bionics.com";
            string password = "Pass@word";
            await userManager.CreateAsync(new O2User { UserName = user, Email = user, EmailConfirmed = true }, password);
            //await userManager.AddToRoleAsync(await userManager.FindByNameAsync(user), "Administrator");
            //await context.SaveChangesAsync();
        }

    }
}
