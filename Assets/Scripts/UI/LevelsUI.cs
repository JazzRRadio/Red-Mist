using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelsUI : MonoBehaviour
{
    [SerializeField]
    private GameObject button;
    [SerializeField]
    private Transform tf_ButtonParent;
    [SerializeField]
    private GameObject panel;

    private List<GameObject> listButtons = new List<GameObject>();

    private List<string> romanNumbers = new List<string> { "0", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" };

    private bool show;

    protected Action OnClickAction;
    public int actualLevel;

    // Add a button per level.
    public void AddButton(int level)
    {
        GameObject newButton = Instantiate(button, tf_ButtonParent);
        listButtons.Add(newButton);

        TextMeshProUGUI _text = newButton.GetComponentInChildren<TextMeshProUGUI>();
        _text.text = romanNumbers[level];
        Button _button = newButton.GetComponent<Button>();

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => ButtonClick(level));
        // LAMBDA EXPRESSIONS (() => void(whit a parameter))
    }

    // Open and close the resource panel, and update by level.
    private void ButtonClick(int level)
    {
        if (show && actualLevel == level)
        {
            show = false;
            panel.SetActive(false);
        }

        else if (show && actualLevel != level)
        {
            actualLevel = level;
            OnClickAction?.Invoke();
        }

        else if (!show)
        {
            actualLevel = level;
            OnClickAction?.Invoke();
            panel.SetActive(true);
            show = true;
        }
    }

    // Destroy all the button.
    public void DestroyButtons()
    {
        if (listButtons.Count > 0)
        {
            foreach (GameObject button in listButtons)
            {
                Destroy(button);
            }
        }
    }
}
