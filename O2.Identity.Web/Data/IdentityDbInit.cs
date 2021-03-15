using System;
using Microsoft.AspNetCore.Identity;
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
            
            // for (int i = 0; i < 500; i++)
            // {
            //     //If there is already an Administrator role, abort
            //     //  if (context.Roles.Any(r => r.Name == "Administrator")) return;
            //     
            //     //Create the Administartor Role
            //     // await roleManager.CreateAsync(new IdentityRole("Administrator"));
            //     
            //    
            //     //Create the default Admin account and apply the Administrator role
            //     string user;
            //
            //     if (i == 0)
            //     {
            //          if (context.Users.Any(r => r.UserName == "demo@demo.com")) 
            //              return;
            //         user = "demo@demo.com";
            //         string password = "P@ssword1";
            //         userManager.CreateAsync(
            //             new O2User
            //             {
            //                 UserName = user, 
            //                 Email = user, 
            //                 Firstname = "demo", 
            //                 Lastname = "demo", 
            //                 RegistrationDate = DateTime.Now, 
            //                 EmailConfirmed = true, 
            //                 IsSpecialist = true
            //             }, 
            //             password).GetAwaiter().GetResult();
            //     }
            //     else
            //     {
            //         if (context.Users.Any(r => r.UserName == "demo"+i+"@demo.com")) 
            //              return;
            //         var userId = context.Users.Single(x => x.UserName == "demo@demo.com").Id;
            //         user = "demo" + i + "@demo.com";
            //         string password = "P@ssword1";
            //         userManager.CreateAsync(
            //             new O2User
            //             {
            //                 UserName = user, 
            //                 Email = user, 
            //                 SpecialistId = userId,
            //                 Firstname = "demo", 
            //                 Lastname = "demo", 
            //                 RegistrationDate = DateTime.Now, 
            //                 EmailConfirmed = true, 
            //                 IsSpecialist = true
            //             }, 
            //             password).GetAwaiter().GetResult();
            //     }
            //
            //
            //    
            // }
            // if (context.Users.Any(r => r.UserName == "demo2@demo.com"))
            //     return;
            // //Create the default Client account and apply the Administrator role
            // string user2 = "demo2@demo.com";
            // string password2 = "P@ssword1";
            // await userManager.CreateAsync(new O2User { UserName = user2, Email = user2, Firstname = "demo2", Lastname = "demo2",RegistrationDate = DateTime.Now,EmailConfirmed = true, IsSpecialist=false }, password2);
            // //   awa
            
        }

    }
}
