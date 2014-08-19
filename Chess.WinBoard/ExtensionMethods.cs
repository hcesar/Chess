namespace System
{
    internal static class ExtensionMethods
    {
        public static TEnum ToEnum<TEnum>(this string value) where TEnum : struct
        {
            var type = typeof(TEnum);
            if (!type.IsEnum)
                throw new InvalidOperationException("Type is not enum: " + type.FullName);

            TEnum ret = default(TEnum);
            Enum.TryParse<TEnum>(value, true, out ret);

            return ret;
        }
    }
}