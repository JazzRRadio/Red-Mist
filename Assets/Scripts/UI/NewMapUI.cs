using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewMapUI : MonoBehaviour
{
    public Toggle randomSeedToggle;

    private string cityName;
    private int seed;

    // Takes the name from the name field.
    public void GetName(string nameField)
    {
        cityName = nameField != null ? nameField : "DefaultName";
    }


    // Takes the seed number from the seed field.
    public void GetSeed(string seedField)
    {
        seed = int.Parse(seedField);
    }

    // Generates a new save file and run the game scene.
    public void NewMap()
    {
        if (!randomSeedToggle.isOn)
        {
            System.Random random = new System.Random();
            int randomSeed = random.Next();
            seed = randomSeed;
        }

        DataInfo newData = DataManager.NewSaveFile(cityName, seed);

        if (newData != null)
        {
            DataManager.Instance.currentGameData = newData;
            DataManager.Instance.gameState = DataManager.GameState.NewMap;
            SceneManager.LoadScene(0);
        }
        else
        {
            // A pop up appears warn you that the city name already exists.
        }
    }
}
