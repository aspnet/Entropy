using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.Mvc.ModuleFramework
{
    public class ModuleActionCollection
    {
	    public ModuleActionCollection(List<ModuleActionInfo> actions, string verb)
	    {
            Actions = actions;
            Verb = verb;
	    }

        public Func<object> this[string path]
        {
            set
            {
                Actions.Add(new ModuleActionInfo()
                {
                    Func = value,
                    Path = path,
                    Verb = Verb,
                });
            }
        }

        private List<ModuleActionInfo> Actions { get; set; }

        public string Verb { get; private set; }
    }
}