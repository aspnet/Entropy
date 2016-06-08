// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace TagHelperSample.Web.Services
{
    public class SignalTokenProviderService : ISignalTokenProviderService
    {
        private readonly ConcurrentDictionary<string, ChangeTokenInfo> _changeTokens
            = new ConcurrentDictionary<string, ChangeTokenInfo>();

        public IChangeToken GetToken(string key)
        {
            return _changeTokens.GetOrAdd(
                key,
                _ =>
                {
                    var cancellationTokenSource = new CancellationTokenSource();
                    var changeToken = new CancellationChangeToken(cancellationTokenSource.Token);
                    return new ChangeTokenInfo(changeToken, cancellationTokenSource);
                }).ChangeToken;
        }

        public void SignalToken(string key)
        {
            ChangeTokenInfo changeTokenInfo;
            if (_changeTokens.TryRemove(key, out changeTokenInfo))
            {
                changeTokenInfo.TokenSource.Cancel();
            }
        }

        private class ChangeTokenInfo
        {
            public ChangeTokenInfo(IChangeToken changeToken, CancellationTokenSource tokenSource)
            {
                ChangeToken = changeToken;
                TokenSource = tokenSource;
            }

            public IChangeToken ChangeToken { get; }

            public CancellationTokenSource TokenSource { get; }
        }
    }
}
