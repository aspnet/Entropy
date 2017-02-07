// Copyright (c) .NET Foundation. All rights reserved.
// See License.txt in the project root for license information

using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace EntropyTests
{
    public class XunitLogger : ILogger, IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly string _categoryName;

        public XunitLogger(ITestOutputHelper output, string categoryName)
        {
            _output = output;
            _categoryName = categoryName;
        }

        public void Log<TState>(
            LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            try
            {
                var firstLinePrefix = $"| {logLevel} [{_categoryName}]: ";
                var lines = formatter(state, exception).Split('\n');
                _output.WriteLine(firstLinePrefix + lines.First());

                var additionalLinePrefix = "|" + new string(' ', firstLinePrefix.Length - 1);

                foreach (var line in lines.Skip(1))
                {
                    _output.WriteLine(additionalLinePrefix + line.Trim('\r'));
                }
            }
            catch (InvalidOperationException)
            {
                // This error usually says: System.InvalidOperationException: There is no currently active test.
                // Swallow this as it shouldn't cause the test to fail.
            }
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state)
            => new NullScope();

        private class NullScope : IDisposable
        {
            public void Dispose()
            {
            }
        }

        public void Dispose()
        {
        }
    }
}
