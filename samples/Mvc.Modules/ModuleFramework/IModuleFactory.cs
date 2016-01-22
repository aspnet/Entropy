namespace Microsoft.AspNetCore.Mvc.ModuleFramework
{
    public interface IModuleFactory
    {
        object CreateModule(ActionContext context);

        void ReleaseModule(object module);
    }
}