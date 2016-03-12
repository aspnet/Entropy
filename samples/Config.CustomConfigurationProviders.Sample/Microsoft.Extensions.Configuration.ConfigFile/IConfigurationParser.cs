// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.Extensions.Configuration.ConfigFile
{
    public interface IConfigurationParser
    {
        /// <summary>
        /// true if the parser can parse the given element, false otherwise.
        /// </summary>
        /// <param name="element">Element to consider</param>
        bool CanParseElement(XElement element);

        /// <summary>
        /// Parses an Xml element and adds it to the results dictionary
        /// </summary>
        /// <param name="element">Xml element to parse</param>
        /// <param name="context">The context dictionary</param>
        /// <param name="results">The key/values in the configuration dictionary</param>
        /// <example>
        /// Given a config file:
        ///   &lt;configuration&gt;
        ///     &lt;sectionZ&gt;
        ///       &lt;nestedSectionA&gt;
        ///         &lt;add key=&quot;keyone&quot; value=&quot;value1&quot; /&gt;
        ///       &lt;/nestedSectionA&gt;
        ///     &lt;/sectionZ&gt;
        ///   &lt;/configuration&gt;
        /// while parsing the element &lt;add key=&quot;keyone&quot; value=&quot;value1&quot; /&gt;, the current elements on the stack could be:
        /// <list type="number">
        ///     <item>
        ///         <description>&quot;nestedSectionA&quot;</description>
        ///     </item>
        ///     <item>
        ///         <description>&quot;sectionZ&quot;</description>
        ///     </item>
        /// </list>
        /// </example>
        void ParseElement(XElement element, Stack<string> context, SortedDictionary<string, string> results);
    }
}
