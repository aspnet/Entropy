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
using System.Threading.Tasks;
#if NET45
using System.Web;
#endif
using Newtonsoft.Json;
using Project.ProjectReference;
using Project.SharedFiles;

namespace Project.Dependencies
{
    public class Program
    {
        public void Main()
        {
            // Dependency shared
            var data = JsonConvert.SerializeObject(new { message = "Hello World".ToLower2() });

#if NET45
            // Imported on net45 only
            data = HttpUtility.HtmlEncode(data);
#endif

            // Using shared code
            var tcs = new TaskCompletionSource<object>();
            TaskAsyncHelpers.ContinueWith(Task.Delay(1000), tcs);

            Console.WriteLine(data);

            Console.ReadLine();
        }
    }
}
