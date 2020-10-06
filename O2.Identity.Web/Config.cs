// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Claims;

namespace O2.Identity.Web
{
    public class Config
    {
        //public static Dictionary<string, string> ClientUrls { get; private set; }
        public static Dictionary<string, string> GetUrls(IConfiguration configuration)
        {
            Dictionary<string, string> urls = new Dictionary<string, string>();

            urls.Add("Mvc", Environment.GetEnvironmentVariable("MvcClient") ?? configuration["MvcClient"]);
            urls.Add("basket", Environment.GetEnvironmentVariable("BasketApi") ?? configuration["BasketApi"]);
            urls.Add("orders", Environment.GetEnvironmentVariable("OrdersApi") ?? configuration["OrdersApi"]);

            Console.WriteLine(" ========================= CONFIG IDENITY ========================== ");
            foreach (var item in urls)
            {
                Console.WriteLine($"key={item.Key}   value={item.Value}");
            }
            Console.WriteLine(" ================= END SETTINGS ====================\r\n");
            return urls;

        }
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                 new ApiResource("basket", "Shopping Cart Api"),
                 new ApiResource("orders", "Ordering Api"),
            };
        }



        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>()
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
               // new IdentityResources.Email()
            };
        }
        public static IEnumerable<Client> GetClients(Dictionary<string, string> clientUrls)
        {

            return new List<Client>()
            {
                new Client
                {
                    ClientId = "o2business-spa",
                    ClientName = "o2business-client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RedirectUris = {"http://localhost:4200/auth-callback"},
                    PostLogoutRedirectUris = {"http://localhost:4200/"},
                    AllowedCorsOrigins = {"http://localhost:4200"},
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        // IdentityServerConstants.StandardScopes.OfflineAccess,
                        // IdentityServerConstants.StandardScopes.Email
                    },
                    AllowAccessTokensViaBrowser = true,
                    AccessTokenLifetime = 3600
                },
                new Client
                {
                    ClientId = "mvc",
                    ClientSecrets = new [] { new Secret("secret".Sha256())},
                    AllowedGrantTypes = GrantTypes.Hybrid,

                    RedirectUris = {$"{clientUrls["Mvc"]}/signin-oidc"},
                    PostLogoutRedirectUris = {$"{clientUrls["Mvc"]}/signout-callback-oidc"},
                    AllowAccessTokensViaBrowser = false,
                    AllowOfflineAccess = true,
                    RequireConsent = false,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowedScopes = new List<string>
                    {

                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                      //  IdentityServerConstants.StandardScopes.Email,
                         "orders",
                        "basket",

                    }

                },
                new Client
                {
                    ClientId = "basketswaggerui",
                    ClientName = "Basket Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{clientUrls["basket"]}/swagger/o2c.html" },
                    PostLogoutRedirectUris = { $"{clientUrls["basket"]}/swagger/" },

                    AllowedScopes = new List<string>
                    {

                        "basket"
                    }
                },
                new Client
                {
                    ClientId = "orderswaggerui",
                    ClientName = "Order Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{clientUrls["orders"]}/swagger/o2c.html" },
                    PostLogoutRedirectUris = { $"{clientUrls["orders"]}/swagger/" },

                    AllowedScopes = new List<string>
                    {

                        "orders"
                    }
                }
            };
        }

    }
}
