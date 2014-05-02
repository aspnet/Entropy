// Copyright (c) Microsoft Open Technologies, Inc.
// All Rights Reserved
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING
// WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF
// TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR
// NON-INFRINGEMENT.
// See the Apache 2 License for the specific language governing
// permissions and limitations under the License.

using System;
using Microsoft.Net.Runtime;

namespace Runtime.ApplicationEnvironment
{
    public class Program
    {
        private readonly IApplicationEnvironment _environment;

        public Program(IApplicationEnvironment environment)
        {
            _environment = environment;
        }

        public void Main()
        {
            Console.WriteLine("======================================================");
            Console.WriteLine("ApplicationName: {0}", _environment.ApplicationName);
            Console.WriteLine("ApplicationBasePath: {0}", _environment.ApplicationBasePath);
            Console.WriteLine("TargetFramework: {0}", _environment.TargetFramework);
            Console.WriteLine("Version: {0}", _environment.Version);
            Console.WriteLine("======================================================");
            Console.ReadLine();
        }
    }
}
