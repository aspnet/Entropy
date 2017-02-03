Mvc.DynamicCorsPolicyProvider Sample
------------------------------------
This sample demonstrates a way to dynamically load CORS policies in a running application without restarting it. 

ICorsPolicyProvider is the DI injectable service through which MVC retrieves CORS policies for a request. The default CORS policy provider gets the policies from an in-memory configuration. Since we want to be able to change the policies dynamically without restarting the application, a custom CORS policy provider is implemented, which watches a file (through the IFileProvider API) containing the CORS settings and refreshes the policies on any changes to the file.