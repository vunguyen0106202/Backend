using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace API.Helpers
{
    public static class Constants
    {
        public static class Strings
        {
            public static class JwtClaimIdentifiers
            {
                public const string Rol = "rol", Id = "id";
            }
            public static class JwtClaims
            {
                public const string ApiAccess = "api_access";
            }
        }
    }
    public static class BannerType
    {
        public static int BannerTop = 1;
        public static int BannerBottom = 2;
        public static int BannerLeft = 3;
        public static int BannerRight = 4;
        public static int BannerFlashSale = 5;
    }

    public static class PaymentType
    {
        public static int COD = 1;
        public static int Vnpay = 2;
        public static int Momo = 3;
        public static int Paypal = 4;
    }

}
