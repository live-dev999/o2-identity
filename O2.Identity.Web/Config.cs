// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

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
            urls.Add("O2BusinessSpa", Environment.GetEnvironmentVariable("O2BusinessSpa") ?? configuration["O2BusinessSpa"]);
            urls.Add("PFRCenterSPA", Environment.GetEnvironmentVariable("PFRCenterSPA") ?? configuration["PFRCenterSPA"]);
            
            urls.Add("certificate-api", Environment.GetEnvironmentVariable("CertificateApi") ?? configuration["CertificateApi"]);
            
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

                 new ApiResource("CertificateApi","Certificate API of O2 Platform")
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
                    ClientId = "client",
                    ClientName="Console Client",
                    ClientSecrets = new []{ new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = new List<string>()
                    {
                        "CertificateApi"
                    }
                },
                
                new Client{
                    ClientId="pfr-center-spa",
                    ClientName="PFR Center SPA",
                    RequirePkce = true,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    RequireClientSecret = false,
                    AllowedGrantTypes=GrantTypes.Code,
                    RedirectUris = new List<string>{ $"{clientUrls["PFRCenterSPA"]}/auth/callback", $"{clientUrls["PFRCenterSPA"]}/assets/silent-callback.html"},
                    PostLogoutRedirectUris = {$"{clientUrls["PFRCenterSPA"]}/"},
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    AllowedCorsOrigins = {$"{clientUrls["PFRCenterSPA"]}"},
                    
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AccessTokenLifetime = 3600
                },
                new Client
                {
                    ClientId = "o2business-spa",
                    ClientName = "o2business-client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RedirectUris = {$"{clientUrls["O2BusinessSpa"]}/auth-callback"},
                    PostLogoutRedirectUris = {$"{clientUrls["O2BusinessSpa"]}/"},
                    // AllowedCorsOrigins = {"http://app.o2bus.com"},
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
                },
                new Client
                {
                    ClientId = "certificateswaggerui",
                    ClientName="Certificate Swagger UI",
                    AllowAccessTokensViaBrowser = true,
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RedirectUris = { $"{clientUrls["certificate-api"]}/swagger/o2c.html" },
                    PostLogoutRedirectUris = { $"{clientUrls["certificate-api"]}/swagger/" },

                    AllowedScopes = new List<string>()
                    {
                        "CertificateApi"
                    }
                },
            };
        }

    }
}
