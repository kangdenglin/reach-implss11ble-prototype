using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Score : MonoBehaviour
{
    public static int totalScore;

    public TextMeshProUGUI finalScoreText;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("game over score:" + totalScore.ToString());
        finalScoreText.text = "Your Score is: " + totalScore.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
