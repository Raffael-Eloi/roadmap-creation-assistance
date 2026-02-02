using System.Text;

namespace RoadmapCreationAssistance.API.Extensions;

public static class StringManipulationExtension
{
    public static string ToBase64(this string value)
    {
        byte[] plainTextBytes = Encoding.UTF8.GetBytes(value);
        return Convert.ToBase64String(plainTextBytes);
    }
}