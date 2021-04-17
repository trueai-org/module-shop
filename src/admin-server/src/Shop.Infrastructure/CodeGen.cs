using System;

namespace Shop.Infrastructure
{
    public static class CodeGen
    {
        private readonly static object obj = new object();

        /// <summary>
        /// 生成长度{length}随机数字组合
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenRandomNumber(int length = 6)
        {
            var code = string.Empty;
            if (length <= 0)
                return code;
            var start = Convert.ToInt32(Math.Pow(10, length - 1));
            var end = Convert.ToInt32(Math.Pow(10, length));
            lock (obj)
            {
                code = new Random().Next(start, end).ToString();
            }
            return code;
        }
    }
}
