using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    // Start is called before the first frame update
    //public string sceneName; // The name of the scene you want to transition to
    public static void changeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
