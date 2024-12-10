public static class ColorExtensions
{
    public static string ToTMPColorCode(this UnityEngine.Color c, bool alpha = false)
    {
        string alphaString = alpha ? $"{(int) (c.a * 255):X2}" : "";
        return $"#{(int)(c.r * 255):X2}{(int)(c.g * 255):X2}{(int)(c.b * 255):X2}{alphaString}";
    }
}