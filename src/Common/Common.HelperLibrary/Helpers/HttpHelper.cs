using Microsoft.AspNetCore.Http;

namespace Common.HelperLibrary.Helpers
{
    public static class HttpHelper
    {
        public static string GetUserId(HttpContext httpContext)
        {
            httpContext.Request.Headers.TryGetValue("claim_userId", out var traceValue);
            string userId = traceValue.ToString();
            return userId;
        }
    }
}
