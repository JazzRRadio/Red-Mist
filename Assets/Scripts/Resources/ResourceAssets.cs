using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resources", menuName = "Game Assests/Resources")]

public class ResourceAssets : ScriptableObject
{
    public enum ResourcesType
    {
        Food,
        Goods,
        Materials,
        Money,
        Homes,
        Population
    }

    public ResourcesType type;

    public bool hasLevel = false;
    public bool stored = false;

    public List<Values> values;
    public List<string> resourcesName;

    public static Action OnValueChanged;

    // Void to determinane if the resource has levels. If so, adjusts the index in the list.
    // If it has no levels, it returns 0.
    public static int IndexLevel(int level, bool hasLevel)
    {
        int index = hasLevel ? level - 1 : 0;
        return index;
    }

    // Adds a new level to the resource.
    public Values AddNewLevel(int level)
    {
        Values _values = new Values();

        int index = IndexLevel(level, hasLevel);

        // Names the resource. Takes the name from the name list.
        if (resourcesName.Count > 0)
        {
            if (resourcesName.Count > ResourceManager.currentGameLevel - 1)
            {
                _values.name = resourcesName[index];
            }

            else
            {
                _values.name = resourcesName[0];
            }

        }

        else
        {
            _values.name = type.ToString();
        }

        return _values;
    }

    // Check whether a cost is allowable for the resource.
    public bool CheckCost(int _amount, int level = 0)
    {
        bool affordable;

        affordable = _amount <= values[IndexLevel(level, hasLevel)].amount ? true : false;

        return affordable;
    }

    // Changes a resource stat.
    public void ChangeAmount(int _amount, int level = 0)
    {
        values[IndexLevel(level, hasLevel)].amount += _amount;
        OnValueChanged?.Invoke();
    }

    public void ChangeDelta(int _amount, int level = 0)
    {
        values[IndexLevel(level, hasLevel)].delta += _amount;
        OnValueChanged?.Invoke();
    }

    public void ChangeStorage(int _amount, int level = 0)
    {
        values[IndexLevel(level, hasLevel)].maxStorage += _amount;
        OnValueChanged?.Invoke();
    }
}

[Serializable]
public class Values
{
    public string name;

    public int
    amount,
    delta,
    maxStorage;
}

