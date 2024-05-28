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
        Parse(categoryStrings, categoryStrings[level..], entityType, level);
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
        if (category != null)
        {
            if (nextLevel < categoryStrings.Length)
            {
                category?.Add(categorySubstrings, entityType, nextLevel);
            }
            else if (nextLevel == categoryStrings.Length && entityType != null)
            {
                category?.Add(new EntityCategory(entityType));
            }
        }
        else if (category == null && categoryStrings[level] != Name)
        {
            var nextCategory = new EntityCategory(categoryStrings, entityType, level);
            SubCategories.Add(nextCategory);
        }
        else if (category == null && categoryStrings[level] == Name && nextLevel < categoryStrings.Length)
        {
            var nextCategory = new EntityCategory(categoryStrings, entityType, nextLevel);
            SubCategories.Add(nextCategory);
        }
        else if (nextLevel == categoryStrings.Length && entityType != null)
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

    private void Add(EntityCategory entityCategory)
    {
        SubCategories.Add(entityCategory);
    }
}
