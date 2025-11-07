public static class StringExtensions
{
    public static string ToPythonLiteral(this string input)
    {
        return "\"" + input
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "")
            + "\"";
    }
}
