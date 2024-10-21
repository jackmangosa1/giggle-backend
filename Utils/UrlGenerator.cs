using System.Web;

namespace ServiceManagementAPI.Utils
{
    public static class UrlGenerator
    {
        public static string GenerateEmailConfirmationLink(string userId, string token, string baseUrl)
        {
            token = HttpUtility.UrlEncode(token);
            return $"{baseUrl}/api/auth/verifyEmail?userId={userId}&token={token}";
        }

        public static string GeneratePasswordResetLink(string userId, string token, string baseUrl)
        {
            token = HttpUtility.UrlEncode(token);
            return $"{baseUrl}/reset-password?userId={userId}&token={token}";
        }
    }
}
