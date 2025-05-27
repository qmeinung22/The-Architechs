using System.Collections.Generic;
using System.Linq;

public static class AlertMessageBuilder
{
    public static string BuildAlertMessage(string locationTitle, string locationAddress, List<double> distances)
    {
        int fireCount = distances.Count;
        string distancesList = string.Join(", ", distances.Select(d => $"{d:F2}"));

        return fireCount == 1
            ? $"🔥 There is 1 active fire near '{locationTitle}' at {locationAddress}! It is within {distancesList} miles of your saved location. Please be ready to evacuate!"
            : $"🔥 There are {fireCount} active fires near '{locationTitle}' at {locationAddress}! These fires are within {distancesList} miles of your saved location. For your safety, please begin to evacuate!";
    }
}
