// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using TagHelperSample.Web.Models;

namespace TagHelperSample.Web.Services
{
    public class MoviesService
    {
        private const string QuotesTokenKey = "quotes";
        private const string FeaturedTokenKey = "featured";

        private readonly Random _random = new Random();

        private readonly IMemoryCache _memoryCache;
        private readonly ISignalTokenProviderService _tokenProviderService;

        public MoviesService(IMemoryCache memoryCache, ISignalTokenProviderService tokenProviderService)
        {
            _memoryCache = memoryCache;
            _tokenProviderService = tokenProviderService;
        }

        public IEnumerable<FeaturedMovies> GetFeaturedMovies()
        {
            return _memoryCache.GetOrCreate(FeaturedTokenKey, (entry) =>
                {
                    entry.SetOptions(new MemoryCacheEntryOptions()
                        .AddExpirationToken(_tokenProviderService.GetToken(FeaturedTokenKey)));
                    return GetMovies().OrderBy(m => m.Rank).Take(2);
                }
            );
        }

        public void UpdateMovieRating()
        {
            _tokenProviderService.SignalToken(FeaturedTokenKey);
        }

        public string GetCriticsQuote(string movieName)
        {
            var quotes = new[]
            {
                "A must see for iguana lovers everywhere",
                "Slightly better than watching paint dry",
                "Never felt more relieved seeing the credits roll",
                "Bravo!"
            };

            return _memoryCache.GetOrCreate(movieName, (entry) =>
                {
                    entry.SetOptions(new MemoryCacheEntryOptions()
                        .AddExpirationToken(_tokenProviderService.GetToken(QuotesTokenKey)));
                    return quotes[_random.Next(0, quotes.Length)];
                }
            );
        }

        public void UpdateCriticsQuotes()
        {
            _tokenProviderService.SignalToken(QuotesTokenKey);
        }

        private IEnumerable<FeaturedMovies> GetMovies()
        {
            yield return new FeaturedMovies { Name = "A day in the life of a blue whale", Rank = _random.Next(1, 10) };
            yield return new FeaturedMovies { Name = "FlashForward", Rank = _random.Next(1, 10) };
            yield return new FeaturedMovies { Name = "Frontier", Rank = _random.Next(1, 10) };
            yield return new FeaturedMovies { Name = "Attack of the space spiders", Rank = _random.Next(1, 10) };
            yield return new FeaturedMovies { Name = "Rift 3", Rank = _random.Next(1, 10) };
        }
    }
}