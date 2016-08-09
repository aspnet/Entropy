using Microsoft.Extensions.Localization;

namespace HelloServices
{
    public class GreetingService
    {
        private IStringLocalizer<GreetingService> _localizer;

        public GreetingService(IStringLocalizer<GreetingService> localizer)
        {
            _localizer = localizer;
        }

        public string SayHello()
        {
            return _localizer["Hello"];
        }
    }
}
