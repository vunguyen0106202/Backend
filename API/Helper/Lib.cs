using System;

namespace API.Helper
{
    public static class Lib
    {
        public static string GenerateOTP()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

    }
}
