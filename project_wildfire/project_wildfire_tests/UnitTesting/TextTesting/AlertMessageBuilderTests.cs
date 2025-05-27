using NUnit.Framework;
using System.Collections.Generic;

[TestFixture]
public class AlertMessageBuilderTests
{
    [Test]
    public void BuildAlertMessage_OneFire_ReturnsCorrectMessage()
    {
        var message = AlertMessageBuilder.BuildAlertMessage(
            "Home", "123 Main St", new List<double> { 2.345 });

        var expected = "🔥 There is 1 active fire near 'Home' at 123 Main St! It is within 2.35 miles of your saved location. Please be ready to evacuate!";
        Assert.That(message, Is.EqualTo(expected));
    }

    [Test]
    public void BuildAlertMessage_MultipleFires_ReturnsCorrectMessage()
    {
        var message = AlertMessageBuilder.BuildAlertMessage(
            "Cabin", "456 Forest Rd", new List<double> { 1.234, 5.678, 9.876 });

        var expected = "🔥 There are 3 active fires near 'Cabin' at 456 Forest Rd! These fires are within 1.23, 5.68, 9.88 miles of your saved location. For your safety, please begin to evacuate!";
        Assert.That(message, Is.EqualTo(expected));
    }
}
