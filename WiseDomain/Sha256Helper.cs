using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace WiseDomain
{
    public static class Sha256Helper
    {
        public static string ToSha256Hash(this string value)
        {
            var sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                var enc = Encoding.UTF8;
                var result = hash.ComputeHash(enc.GetBytes(value));

                foreach (var b in result)
                    sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
