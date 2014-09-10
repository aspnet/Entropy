using Microsoft.AspNet.Mvc.ReflectedModelBuilder;
using System.Text;

namespace NamespaceRouting
{
    internal class NameSpaceRoutingConvention : IReflectedApplicationModelConvention
    {
        public void OnModelCreated(ReflectedApplicationModel model)
        {
            foreach (var controller in model.Controllers)
            {
                if (controller.AttributeRouteModel == null)
                {
                    //Create new attribute Route for the controller
                    controller.AttributeRouteModel = new ReflectedAttributeRouteModel();

                    //Replace the . in the namespace with a / to create the attribute route 
                    //Ex: MySite.Admin namespace will corrospond to MySite/Admin attribute route
                    //Then attach [controller], [action] and optional {id?} token. 
                    //[Controller] and [action] is replaced with the controller and action 
                    //name to generate the final template
                    var template = controller.ControllerType.Namespace.Replace('.', '/') + "/[controller]/[action]/{id?}";
                    controller.AttributeRouteModel.Template = template;
                }
            }

            //You can continue to put attribute route templates for the controller actions depending on the way you want them to behave            
        }
    }
}