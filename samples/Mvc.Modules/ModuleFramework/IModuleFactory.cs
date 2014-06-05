namespace Microsoft.AspNet.Mvc.ModuleFramework
{
    public interface IModuleFactory
    {
        object CreateModule(ActionContext context);

        void ReleaseModule(object module);
    }
}