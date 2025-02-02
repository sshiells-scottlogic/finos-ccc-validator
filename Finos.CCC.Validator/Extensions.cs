namespace Finos.CCC.Validator;

internal static class Extensions
{
    public static string ToPassOrFail(this bool isValid)
    {
        return isValid ? "SUCCESS" : "FAILED";
    }
}