using System;
using System.Globalization;
using System.Text;

namespace Shop.Infrastructure.Helpers
{
    public static class StringHelper
    {
        public static string ToUrlFriendly(this string name)
        {
            // Fallback for product variations
            if (string.IsNullOrWhiteSpace(name))
            {
                return Guid.NewGuid().ToString();
            }

            name = name.ToLower();
            name = RemoveDiacritics(name);
            name = ConvertEdgeCases(name);
            name = name.Replace(" ", "-");
            name = name.Strip(c =>
                c != '-'
                && c != '_'
                && !Char.IsLetter(c)
                && !Char.IsDigit(c)
                );

            while (name.Contains("--"))
                name = name.Replace("--", "-");

            if (name.Length > 200)
                name = name.Substring(0, 200);

            if (string.IsNullOrWhiteSpace(name))
            {
                return Guid.NewGuid().ToString();
            }

            return name;
        }

        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string Strip(this string subject, params char[] stripped)
        {
            if (stripped == null || stripped.Length == 0 || String.IsNullOrEmpty(subject))
            {
                return subject;
            }

            var result = new char[subject.Length];

            var cursor = 0;
            for (var i = 0; i < subject.Length; i++)
            {
                char current = subject[i];
                if (Array.IndexOf(stripped, current) < 0)
                {
                    result[cursor++] = current;
                }
            }

            return new string(result, 0, cursor);
        }

        public static string Strip(this string subject, Func<char, bool> predicate)
        {

            var result = new char[subject.Length];

            var cursor = 0;
            for (var i = 0; i < subject.Length; i++)
            {
                char current = subject[i];
                if (!predicate(current))
                {
                    result[cursor++] = current;
                }
            }

            return new string(result, 0, cursor);
        }

        public static string TrimStart(this string target, string trimString)
        {
            if (string.IsNullOrEmpty(trimString)) return target;

            string result = target;
            while (result.StartsWith(trimString))
            {
                result = result.Substring(trimString.Length);
            }

            return result;
        }

        public static string TrimEnd(this string target, string trimString)
        {
            if (string.IsNullOrEmpty(trimString)) return target;

            string result = target;
            while (result.EndsWith(trimString))
            {
                result = result.Substring(0, result.Length - trimString.Length);
            }

            return result;
        }

        private static string ConvertEdgeCases(string text)
        {
            var sb = new StringBuilder();
            foreach (var c in text)
            {
                sb.Append(ConvertEdgeCases(c));
            }

            return sb.ToString();
        }

        private static string ConvertEdgeCases(char c)
        {
            string swap;
            switch (c)
            {
                case 'ı':
                    swap = "i";
                    break;
                case 'ł':
                case 'Ł':
                    swap = "l";
                    break;
                case 'đ':
                    swap = "d";
                    break;
                case 'ß':
                    swap = "ss";
                    break;
                case 'ø':
                    swap = "o";
                    break;
                case 'Þ':
                    swap = "th";
                    break;
                default:
                    swap = c.ToString();
                    break;
            }

            return swap;
        }

        /// <summary>
        /// 邮箱加密***
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string EmailEncryption(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return string.Empty;
            var split = email.Split('@');
            if (split != null && split.Length >= 2)
            {
                var before = split[0];
                if (before.Length > 0)
                {
                    var len = before.Length / 3;
                    len = len <= 0 ? 1 : len;

                    email = before.Substring(0, len) + "***" +
                            before.Substring(before.Length - len)
                            + "@" + split[1];
                }
            }
            return email;
        }

        /// <summary>
        /// 手机号码加密***
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string PhoneEncryption(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return string.Empty;
            if (phone.Length > 0)
            {
                var len = phone.Length / 3;
                len = len <= 0 ? 1 : len;
                phone = phone.Substring(0, len) + "***" + phone.Substring(phone.Length - len);
            }
            return phone;
        }
    }
}
