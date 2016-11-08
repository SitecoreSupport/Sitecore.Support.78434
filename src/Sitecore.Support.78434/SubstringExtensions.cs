namespace Sitecore.Support
{
    public static class SubstringExtensions
    {
        public static string Between(this string value, string a, string b)
        {
            int num = value.IndexOf(a);
            int num2 = value.LastIndexOf(b);
            if (num == -1)
            {
                return "";
            }
            if (num2 == -1)
            {
                return "";
            }
            int num3 = num + a.Length;
            if (num3 >= num2)
            {
                return "";
            }
            return value.Substring(num3, num2 - num3);
        }
    }
}
