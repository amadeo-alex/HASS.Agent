using HASS.Agent.Base.Models.Entity;

namespace HASS.Agent.Test.Base.Models;

[TestClass]
public class EntityCategoryTest
{
    [TestMethod]
    public void TestParsing()
    {
        var rootCategory = "root";
        var category = new EntityCategory(rootCategory, null);
        category.Add("level1/level2/level3", typeof(Buffer));
        category.Add("nxlevel1/nxlevel2", typeof(int));
        category.Add("level1/level2/level3/level4", typeof(string));
        category.Add("level1/level2a/asdasd", typeof(Byte));
        category.Add("asd/123/efg", typeof(double));
        category.Add("level1/level2a", typeof(Array));
        category.Add("asd/123/efg", typeof(bool));

        var level1Category = category.SubCategories.FirstOrDefault(c => c.Name == "level1");
        Assert.IsNotNull(level1Category, "level1 category missing");
        Assert.AreEqual(level1Category.Name, "level1", "level1 category contains improper name");

        var level2Category = level1Category.SubCategories.FirstOrDefault(c => c.Name == "level2");
        Assert.IsNotNull(level2Category, "level2 category missing");
        Assert.AreEqual(level2Category.Name, "level2", "level2 category contains improper name");

        var level3Category = level2Category.SubCategories.FirstOrDefault(c => c.Name == "level3");
        Assert.IsNotNull(level3Category, "level3 category missing");
        Assert.AreEqual(level3Category.Name, "level3", "level3 category contains improper name");

        var bufferEntity = level3Category.SubCategories.FirstOrDefault(c => c.Name == "Buffer");
        Assert.IsNotNull(bufferEntity, "buffer entity is null");
        Assert.AreEqual(bufferEntity.Name, "Buffer", "buffer entity name is improper");
        Assert.AreEqual(bufferEntity.EntityType, true, "buffer entity is not marked properly");

        var level2ACategory = level1Category.SubCategories.FirstOrDefault(c => c.Name == "level2a");
        Assert.IsNotNull(level2ACategory, "level2a category missing");
        Assert.AreEqual(level2ACategory.Name, "level2a", "level2a category contains improper name");

        var asdasdCategory = level2ACategory.SubCategories.FirstOrDefault(c => c.Name == "asdasd");
        Assert.IsNotNull(asdasdCategory, "asdasd category missing");
        Assert.AreEqual(asdasdCategory.Name, "asdasd", "asdasd category contains improper name");

        var byteEntity = asdasdCategory.SubCategories.FirstOrDefault(c => c.Name == "Byte");
        Assert.IsNotNull(byteEntity, "byte entity is null");
        Assert.AreEqual(byteEntity.Name, "Byte", "byte entity name is improper");
        Assert.AreEqual(byteEntity.EntityType, true, "byte entity is not marked properly");

        var asdCategory = category.SubCategories.FirstOrDefault(c => c.Name == "asd");
        Assert.IsNotNull(asdCategory, "asd category missing");
        Assert.AreEqual(asdCategory.Name, "asd", "asd category contains improper name");

        var v123Category = asdCategory.SubCategories.FirstOrDefault(c => c.Name == "123");
        Assert.IsNotNull(v123Category, "123 category missing");
        Assert.AreEqual(v123Category.Name, "123", "123 category contains improper name");

        var efgCategory = v123Category.SubCategories.FirstOrDefault(c => c.Name == "efg");
        Assert.IsNotNull(efgCategory, "efg category missing");
        Assert.AreEqual(efgCategory.Name, "efg", "efg category contains improper name");

        var boolEntity = efgCategory.SubCategories.FirstOrDefault(c => c.Name == "Boolean");
        Assert.IsNotNull(boolEntity, "bool entity is null");
        Assert.AreEqual(boolEntity.Name, "Boolean", "bool entity name is improper");
        Assert.AreEqual(boolEntity.EntityType, true, "bool entity is not marked properly");
    }
}