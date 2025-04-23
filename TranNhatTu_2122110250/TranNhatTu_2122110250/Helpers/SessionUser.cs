namespace TranNhatTu_2122110250.Helpers
{
    public static class SessionUser
    {
        public static string GetUsername(HttpContext context) => context.Session.GetString("Username") ?? "Khách";
        public static string GetEmail(HttpContext context) => context.Session.GetString("Email") ?? "unknown@example.com";
        public static int? GetUserId(HttpContext context) => context.Session.GetInt32("UserId");
    }

}
