using Microsoft.Extensions.Localization;

namespace Yelling
{
    public class YellingService
    {
        private IStringLocalizer<YellingService> _localizer;

        public YellingService(IStringLocalizer<YellingService> localizer)
        {
            _localizer = localizer;
        }

        public string GetYellingHello()
        {
            return _localizer["Hello"];
        }
    }
}
