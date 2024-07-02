using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public GameObject tooltip;

    public void ShowTooltip(bool show)
    {
        tooltip.SetActive(show);
    }
}
