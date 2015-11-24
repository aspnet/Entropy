// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using EFLocalizationSample.Models;
using Microsoft.Extensions.Localization;

namespace EFLocalizationSample
{
    public class EFStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly LocalizationDBContext _db;

        public EFStringLocalizerFactory()
        {
            _db = new LocalizationDBContext();
            _db.AddRange(
                new Culture
                {
                    Name = "en-US",
                    Resources = new List<Resource>() { new Resource { Key = "Hello", Value = "Hello" } }
                },
                new Culture
                {
                    Name = "fr-FR",
                    Resources = new List<Resource>() { new Resource { Key = "Hello", Value = "Bonjour" } }
                },
                new Culture
                {
                    Name = "es-ES",
                    Resources = new List<Resource>() { new Resource { Key = "Hello", Value = "Hola" } }
                },
                new Culture
                {
                    Name = "jp-JP",
                    Resources = new List<Resource>() { new Resource { Key = "Hello", Value = "こんにちは" } }
                },
                new Culture
                {
                    Name = "zh",
                    Resources = new List<Resource>() { new Resource { Key = "Hello", Value = "您好" } }
                },
                new Culture
                {
                    Name = "zh-CN",
                    Resources = new List<Resource>() { new Resource { Key = "Hello", Value = "您好" } }
                }
                );
            _db.SaveChanges();
        }

        public IStringLocalizer Create(Type resourceSource)
        {

            return new EFStringLocalizer(_db);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new EFStringLocalizer(_db);
        }
    }
}
