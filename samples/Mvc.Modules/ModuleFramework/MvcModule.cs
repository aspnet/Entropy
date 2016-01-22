using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Microsoft.AspNetCore.Mvc.ModuleFramework
{
    public abstract class MvcModule
    {
        public MvcModule()
        {
            Actions = new List<ModuleActionInfo>();

            Get = new ModuleActionCollection(Actions, "GET");
            Post = new ModuleActionCollection(Actions, "POST");
        }

        public ModuleActionCollection Get { get; private set; }

        public ModuleActionCollection Post { get; private set; }

        public List<ModuleActionInfo> Actions { get; private set; }

        public ActionContext ActionContext { get; set; }

        public ViewDataDictionary ViewData { get; set; }
    }
}