# Use the same Controller for multiple areas
  
This sample shows how to add a convention to MVC options (in `Startup`) that allows multiple areas to use the same Controller.

The `MultipleAreasControllerConvention` implements `IApplicationModelConvention`, and adds all the specified areas as `RouteValues` to the application model.

In this sample, Products, Services and Manage areas use the same `HomeController` to serve their Index view by listing the area names in the `MultipleAreasAttribute`.