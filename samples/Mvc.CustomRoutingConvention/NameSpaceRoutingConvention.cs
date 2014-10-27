using Microsoft.AspNet.Mvc.ApplicationModels;

namespace NamespaceRouting
{
    internal class NameSpaceRoutingConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                if (controller.AttributeRoutes.Count == 0)
                {
                    //Create new attribute Route for the controller
                    var attributeRouteModel = new AttributeRouteModel();

                    //Replace the . in the namespace with a / to create the attribute route 
                    //Ex: MySite.Admin namespace will correspond to MySite/Admin attribute route
                    //Then attach [controller], [action] and optional {id?} token. 
                    //[Controller] and [action] is replaced with the controller and action 
                    //name to generate the final template
                    var template = controller.ControllerType.Namespace.Replace('.', '/') + "/[controller]/[action]/{id?}";
                    attributeRouteModel.Template = template;

                    controller.AttributeRoutes.Add(attributeRouteModel);
                }
            }

            //You can continue to put attribute route templates for the controller actions depending on the way you want them to behave            
        }
    }
}