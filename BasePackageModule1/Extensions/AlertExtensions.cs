using Microsoft.AspNetCore.Mvc;

namespace BasePackageModule1.Extensions
{
    public static class AlertExtensions
    {
        public static IActionResult WithSuccess(this IActionResult result, string title, string body)
        {
            return Alert(result, "success", title, body);
        }

        public static IActionResult WithInfo(this IActionResult result, string title, string body)
        {
            return Alert(result, "info", title, body);
        }

        public static IActionResult WithWarning(this IActionResult result, string title, string body)
        {
            return Alert(result, "warn", title, body);
        }

        public static IActionResult WithError(this IActionResult result, string title, string body)
        {
            return Alert(result, "error", title, body);
        }

        private static IActionResult Alert(IActionResult result, string type, string title, string body)
        {
            return new AlertDecoratorResult(result, type, title, body);
        }
    }
}
