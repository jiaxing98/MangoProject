namespace MangoWeb
{
    public enum APIType
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public static class Settings
    {
        public const string ADMIN = "Admin";
        public const string CUSTOMER = "Customer";

        public static string ProductAPIBase { get; set; }
        public static string CartAPIBase { get; set; }
        public static string CouponAPIBase { get; set; }

    }
}
