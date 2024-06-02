using HASS.Agent.Base.Models;

namespace HASS.Agent.Test.Base.Models;

[TestClass]
public class ConfiguredEntityTest
{
    [TestMethod]
    public void TestComparison()
    {
        var g1 = Guid.NewGuid();
        var g2 = Guid.NewGuid();

        var cs1 = new ConfiguredEntity();
        var cs2 = new ConfiguredEntity();

        Assert.AreEqual(cs1, cs2);

        cs1.Type = typeof(byte).Name;
        Assert.AreNotEqual(cs1, cs2);

        cs2.Type = typeof(byte).Name;
        Assert.AreEqual(cs1, cs2);

        cs1.UniqueId = g1;
        cs2.UniqueId = g2;
        Assert.AreNotEqual(cs1, cs2);

        cs2.UniqueId = g1;
        Assert.AreEqual(cs1, cs2);

        cs1.Properties["a"] = "a";
        Assert.AreNotEqual(cs1, cs2);

        cs2.Properties["a"] = "a";
        Assert.AreEqual(cs1, cs2);
    }
}
