using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class sceneChanger : MonoBehaviour
{
    // public string in which user will input the next scene name
    public string sceneName;
    public int goldAmount;
    public Text goldText;

    void Awake()
    {
        goldAmount = GameObject.FindGameObjectsWithTag("Gold").Length;
    }


    void Update()
    {
        goldText.text = "Gold Remaining: " + goldAmount;
        if (goldAmount <= 0)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
