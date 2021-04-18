using System.Text.RegularExpressions;

namespace Shop.Infrastructure.Helpers
{
    public static class RegexHelper
    {
        const string patternEmail = @"\w[-\w.+]*@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,14}";
        const string patternPhone = @"^\d{11}$";

        public static (bool Succeeded, string Message) VerifyEmail(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return (false, "请输入邮箱地址");
            var regex = new Regex(patternEmail);
            if (!regex.IsMatch(input))
                return (false, "邮箱地址格式错误");
            return (true, "邮箱地址格式正确");
        }

        public static (bool Succeeded, string Message) VerifyPhone(string input)
        {
            if (string.IsNullOrWhiteSpace(input.Trim()))
                return (false, "请输入手机号码");
            var regex = new Regex(patternPhone);
            if (!regex.IsMatch(input.Trim()))
                return (false, "手机号格式错误");
            return (true, "手机号格式正确");
        }
    }
}
