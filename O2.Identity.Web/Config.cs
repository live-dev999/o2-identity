﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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

            urls.Add("Mvc", "http://shop.antonmarkov.com");//configuration.GetValue<string>("MvcClient"));
            urls.Add("BasketApi", "http://api-basket.o2bus.com");//configuration.GetValue<string>("BasketApiClient"));

            return urls;

        }
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                 new ApiResource("basket", "Basket Api"),
                 new ApiResource("certificates", "Certificate Api"),
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
        public static IEnumerable<Client> GetClients()
        {

            return new List<Client>()
            {
                new Client
                {
                    ClientId = "mvc",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    ClientSecrets = new []
                    {
                        new Secret("secret".Sha256())
                    },
                    RedirectUris = {"http://localhost:5500/signin-oidc"},
                    PostLogoutRedirectUris = {$"http://localhost:5500/signout-callback-oidc"},
                    
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowAccessTokensViaBrowser = false,
                    RequireConsent = false,
                    
                    AllowedScopes = new List<string>()
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "orders",
                        "basket",
                        "certificates"
                    }
                    
                    //
                    // RedirectUris = {$"{clientUrls["Mvc"]}/signin-oidc"},
                    // PostLogoutRedirectUris = {$"{clientUrls["Mvc"]}/signout-callback-oidc"},
                    // AllowAccessTokensViaBrowser = false,
                    // AllowOfflineAccess = true,
                    // RequireConsent = false,
                    // AlwaysIncludeUserClaimsInIdToken = true,
                    // AllowedScopes = new List<string>
                    // {
                    //
                    //     IdentityServerConstants.StandardScopes.OpenId,
                    //     IdentityServerConstants.StandardScopes.Profile,
                    //     IdentityServerConstants.StandardScopes.OfflineAccess,
                    //   //  IdentityServerConstants.StandardScopes.Email,
                    //      "orders",
                    //     "basket",
                    //
                    // }

                },
        //         new Client
        //         {
        //             ClientId = "basketswaggerui",
        //             ClientName = "Basket Swagger UI",
        //             AllowedGrantTypes = GrantTypes.Implicit,
        //             AllowAccessTokensViaBrowser = true,
        //
        //             RedirectUris = { $"{clientUrls["BasketApi"]}/swagger/o2c.html" },
        //             PostLogoutRedirectUris = { $"{clientUrls["BasketApi"]}/swagger/" },
        //
        //              AllowedScopes = new List<string>
        //              {
        //
        //                 "basket"
        //              }
        // }
            };
        }

    }
}