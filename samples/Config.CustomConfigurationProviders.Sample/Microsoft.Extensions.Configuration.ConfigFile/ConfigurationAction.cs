// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Extensions.Configuration.ConfigFile
{
    /// <summary>
    /// This represents the action for that *.config element.
    /// There are 3 possible element names:
    ///     <list type="number">
    ///         <item>
    ///             <term>add</term>
    ///         </item>
    ///         <item>
    ///             <term>remove</term>
    ///         </item>
    ///         <item>
    ///             <term>clear</term>
    ///         </item>
    ///     </list>
    /// https://msdn.microsoft.com/en-us/library/aa903313(v=vs.71).aspx
    /// </summary>
    internal enum ConfigurationAction
    {
        Add,
        Remove,
        Clear
    }
}