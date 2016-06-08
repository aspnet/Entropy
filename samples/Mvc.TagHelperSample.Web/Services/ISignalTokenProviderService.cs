// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Primitives;

namespace TagHelperSample.Web.Services
{
    public interface ISignalTokenProviderService
    {
        IChangeToken GetToken(string key);

        void SignalToken(string key);
    }
}