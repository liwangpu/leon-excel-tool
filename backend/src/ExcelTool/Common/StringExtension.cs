namespace ExcelTool.Common
{
    public static class StringExtension
    {
        public static bool ExistHanziCharacter(this string str)
        {
            char[] c = str.ToCharArray();
            if (string.IsNullOrWhiteSpace(str)) { return false; }
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] >= 0x4e00 && c[i] <= 0x9fbb)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
