using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadFilesUI : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI text_cityName;
    [SerializeField]
    public TextMeshProUGUI text_days;
    [SerializeField]
    public TextMeshProUGUI text_date;
    [SerializeField]
    public Button button_Delete;
    public DataInfo tabDataInfo;

    public void InfoUpdate(string cityName, int dayNumber, string date, DataInfo dataInfo)
    {
        text_cityName.text = cityName;
        text_days.text = "Days: " + dayNumber;
        text_date.text = date;
        tabDataInfo = dataInfo;
    }

    public void SelectData()
    {
        if (tabDataInfo != null)
        {
            DataManager.Instance.currentGameData = tabDataInfo;
        }
    }
}
