using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages.Internal;
using Microsoft.Extensions.Options;

namespace Mvc.CustomRazorPageHandlers
{
  public class CustomHandlerPageApplicationModelProvider : DefaultPageApplicationModelProvider
  {
    public CustomHandlerPageApplicationModelProvider(IModelMetadataProvider modelMetadataProvider, IOptions<MvcOptions> options)
            : base(modelMetadataProvider, options)
    {
    }

    protected override PageHandlerModel CreateHandlerModel(MethodInfo method)
    {
      if (method == null)
      {
          throw new ArgumentNullException(nameof(method));
      }

      if (!IsHandler(method))
      {
          return null;
      }

      if (!TryParseHandlerMethod(method.Name, out var httpMethod, out var handlerName))
      {
          return null;
      }

      var handlerModel = new PageHandlerModel(
          method,
          method.GetCustomAttributes(inherit: true))
      {
          Name = method.Name,
          HandlerName = handlerName,
          HttpMethod = httpMethod,
      };

      var methodParameters = handlerModel.MethodInfo.GetParameters();

      for (var i = 0; i < methodParameters.Length; i++)
      {
          var parameter = methodParameters[i];
          var parameterModel = CreateParameterModel(parameter);
          parameterModel.Handler = handlerModel;

          handlerModel.Parameters.Add(parameterModel);
      }

      return handlerModel;
    }

    static bool TryParseHandlerMethod(string methodName, out string httpMethod, out string handler)
    {
        httpMethod = null;
        handler = null;

        // Now we parse the method name according to our conventions to determine the required HTTP method
        // and optional 'handler name'.
        //
        // Valid names look like:
        //  - Get
        //  - GetCustomer
        //  - PostCustomer
        //  - DeleteCustomer

        var length = methodName.Length;
        if (methodName.EndsWith("Async", StringComparison.Ordinal))
        {
            length -= "Async".Length;
        }

        if (length == 0)
        {
            // Method is named "Async". Bail.
            return false;
        }

        var handlerNameStart = 1;
        for (; handlerNameStart < length; handlerNameStart++)
        {
            if (char.IsUpper(methodName[handlerNameStart]))
            {
                break;
            }
        }

        httpMethod = methodName.Substring(0, handlerNameStart);

        if (string.Equals(httpMethod, "GET", StringComparison.OrdinalIgnoreCase) || 
            string.Equals(httpMethod, "POST", StringComparison.OrdinalIgnoreCase))
        {
          // Do nothing. The remaining portion is the handler method.
          // e.g. PostCustomerAsync
          // HttpMethod = "POST", Handler = "Customer"
          handler = handlerNameStart == length ? null : methodName.Substring(handlerNameStart, length - handlerNameStart);
        }
        else if (
          string.Equals(httpMethod, "DELETE", StringComparison.OrdinalIgnoreCase) || 
          string.Equals(httpMethod, "PUT", StringComparison.OrdinalIgnoreCase))
        {
          // e.g. DeleteUser
          // HttpMethod = "POST", Handler = "DeleteUser"
          httpMethod = "POST";
          handler = methodName;
        }
        else
        {
            return false;
        }

        return true;
    }
  }
}
