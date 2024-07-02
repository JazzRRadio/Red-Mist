using System;
using TMPro;
using UnityEngine;

public class ResourceUI : MonoBehaviour
{
    [SerializeField]
    private GameObject tooltip;
    [SerializeField]
    private TextMeshProUGUI text_ResourceAmount;
    [SerializeField]
    private TextMeshProUGUI text_ResourceName;
    [SerializeField]
    private ResourceAssets sc_ResourceAssets;

    public enum Format
    {
        Amount,
        AmountAndDelta,
    }

    public Format format;

    private void OnEnable()
    {
        ResourceAssets.OnValueChanged += UpdateResource;
    }

    private void OnDisable()
    {
        ResourceAssets.OnValueChanged -= UpdateResource;
    }

    // Place the name of the resource in the tooltip
    public void ResourceStats(int level = 0)
    {
        bool _hasLevel = sc_ResourceAssets.hasLevel;

        text_ResourceName.text = sc_ResourceAssets.values[ResourceAssets.IndexLevel(level, _hasLevel)].name;
    }

    // Refresh the resource stat in UI.
    private void UpdateResource()
    {
        int level = UIDirector.Instance.sc_ResourceMenuList.actualLevel;
        UpdateResource(level);
    }

    public void UpdateResource(int level)
    {
        bool _hasLevel = sc_ResourceAssets.hasLevel;

        int index = _hasLevel ? level - 1  : 0;

        string _amount = sc_ResourceAssets.values[index].amount.ToString("N0");

        if (format == Format.AmountAndDelta && sc_ResourceAssets.values[index].delta != 0)
        {
            string _delta = sc_ResourceAssets.values[index].delta.ToString("N0");

            text_ResourceAmount.text = $"{_amount}{Environment.NewLine}{_delta}";
        }
        else
        {
            text_ResourceAmount.text = _amount;
        }
    }
}
