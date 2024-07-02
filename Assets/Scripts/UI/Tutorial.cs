using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;


public class Tutorial : MonoBehaviour
{
    [SerializeField]
    private GameObject dialoguesPanel;
    [SerializeField]
    private GameObject acceptButton;
    [SerializeField]
    private GameObject buildingButton;
    [SerializeField]
    private TextMeshProUGUI dialoguesText;

    private Button bu_AcceptButton;
    private Button bu_buildingButton;

    private List<string> dialogue = new List<string> {
        "Welcome to the Red Mist Prototype", // 0
        "To move the cameras you can use the WASD keys or move the mouse while pressing the right button of the mouse.", // 1
        "To build, click on the town hall icon and place it with another click on the ground. If you press the [ESC] key, you cancel the placement.", // 2
        "In the panel above you find all the resources you have, if you put the cursor over the numbers, you will be able to see their names.", // 3
        "In the panel below, by clicking on [I], you find the blueprint of the building you can build.", // 4
        "With this, you can start playing. Good luck" // 5
        };
    private int index = 0;

    private void Awake()
    {
        bu_AcceptButton = acceptButton.GetComponent<Button>();
        bu_buildingButton = buildingButton.GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        dialoguesText.text = dialogue[index];
        bu_AcceptButton.onClick.AddListener(NextText);
    }

    // Update is called once per frame
    void Update()
    {
        PlaceBuilding();
    }

    private void OnDisable()
    {
        bu_AcceptButton.onClick.RemoveAllListeners();
    }

    private void PlaceBuilding()
    {
        if (BuildingPlacer.isPlaced && index == 2)
        {
            NextText();
            acceptButton.SetActive(true);
            buildingButton.SetActive(false);
            dialoguesPanel.SetActive(true);
        }

        else if (!BuildingPlacer.currentlyPlacing && index == 2)
        {
            dialoguesPanel.SetActive(true);
            acceptButton.SetActive(false);
            buildingButton.SetActive(true);
        }

        else if (BuildingPlacer.currentlyPlacing)
        {
            dialoguesPanel.SetActive(false);
        }
    }

    private void NextText()
    {
        index++;
        if (index >= dialogue.Count)
        {
            dialoguesPanel.SetActive(false);
        }
        else
        {
            dialoguesText.text = dialogue[index];
        }
    }
}
