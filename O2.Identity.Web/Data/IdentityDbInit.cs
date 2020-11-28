﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreatedAsync();
            context.Database.Migrate();
            //If there is already an Administrator role, abort
            //  if (context.Roles.Any(r => r.Name == "Administrator")) return;

            //Create the Administartor Role
            // await roleManager.CreateAsync(new IdentityRole("Administrator"));
            if (context.Users.Any(r => r.UserName == "demo@demo.com")) return;
            //Create the default Admin account and apply the Administrator role
            string user = "demo@demo.com";
            string password = "P@ssword1";
            await userManager.CreateAsync(new O2User { UserName = user, Email = user, Firstname = "demo", Lastname = "demo", EmailConfirmed = true, IsSpecialist=true }, password);
            
            if (context.Users.Any(r => r.UserName == "demo2@demo.com"))
                return;
            //Create the default Client account and apply the Administrator role
            string user2 = "demo2@demo.com";
            string password2 = "P@ssword1";
            await userManager.CreateAsync(new O2User { UserName = user2, Email = user2, Firstname = "demo2", Lastname = "demo2", EmailConfirmed = true, IsSpecialist=false }, password2);
            //   await userManager.AddToRoleAsync(await userManager.FindByNameAsync(user), "Administrator");
        }

    }
}
