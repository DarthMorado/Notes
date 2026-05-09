namespace DarthNotes.Web.Extensions;

public static class StringExtensions
{
    public static string Left(this string value, int length)
    {
        if (string.IsNullOrEmpty(value)) return value;

        return value.Length <= length ? value : value[0..length];
    }
}