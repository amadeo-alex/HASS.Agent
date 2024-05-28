using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.PointOfService;

namespace HASS.Agent.Base.Models.Entity;
public class EntityCategory //TODO(Amadeo): interface?
{
    public string Name { get; set; } = string.Empty;
    public List<EntityCategory> SubCategories { get; set; } = [];
    public bool EntityType { get; set; }

    public EntityCategory(string categoryString, Type? entityType)
    {
        var split = categoryString.Split('/');
        Name = split[0];
        Parse(split, split, entityType, 0);
    }

    public EntityCategory(string[] categoryStrings, Type? entityType, int level = 0)
    {
        Name = categoryStrings[level];
        Parse(categoryStrings, categoryStrings[level..],entityType, level);
    }

    public EntityCategory(Type entityType)
    {
        Name = entityType.Name;
        EntityType = true;
    }

    private void Parse(string[] categoryStrings, string[] categorySubstrings, Type? entityType, int level)
    {
        var nextLevel = level + 1;

        var category = SubCategories.FirstOrDefault(c => c.Name == categoryStrings[level]);
        if (category == null && categoryStrings[level] != Name)
        {

            var nextCategory = new EntityCategory(categoryStrings, entityType, level);
            SubCategories.Add(nextCategory);
            //nextCategory.Add(categorySubstrings, entityType, nextLevel);

            return;
        }else if(category == null && categoryStrings[level] == Name && nextLevel < categoryStrings.Length)
        {
            var nextCategory = new EntityCategory(categoryStrings, entityType, nextLevel);
            SubCategories.Add(nextCategory);
        }
        
        if (category != null && nextLevel < categoryStrings.Length)
        {
            //var nextCategory = new EntityCategory(categoryStrings, entityType, nextLevel);
            category?.Add(categorySubstrings, entityType, nextLevel);
        }
        
        if (nextLevel == categoryStrings.Length && entityType != null)
        {
            SubCategories.Add(new EntityCategory(entityType));
        }
    }

    public void Add(string categoryString, Type? entityType, int level = 0)
    {
        var split = categoryString.Split('/');
        Parse(split, split, entityType, level);
    }

    private void Add(string[] categoryStrings, Type? entityType, int level = 0)
    {
        Parse(categoryStrings, categoryStrings, entityType, level);
    }


    /*    public static EntityCategory? ParseX(string categoryString, Type? entityType = null)
        {
            var split = categoryString.Split('/');
            return ParseInternal(split, 0, entityType);
        }*/

    /*    private static EntityCategory? ParseInternal(string[] categories, int index, Type? entityType)
        {
            if (index >= categories.Length)
                return null;

            var nextIndex = index + 1;
            var nextCategory = ParseInternalX(categories, nextIndex, entityType);
            var category = new EntityCategory
            {
                Name = categories[index],
            };

            if (nextCategory != null && !category.SubCategories.Contains(nextCategory))
                category.SubCategories.Add(nextCategory);

            if (nextCategory == null && entityType != null)
                category.EntityTypes.Add(entityType);

            return category;
        }*/
}
