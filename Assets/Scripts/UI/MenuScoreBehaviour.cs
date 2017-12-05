using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuScoreBehaviour : MonoBehaviour {

    int Score = 0;
    string TextToDisplay = "";
    public TextMeshPro Text;

	// Use this for initialization
	void Start () {
        if (GameManager.GetInstance())
        {
            Score = GameManager.GetInstance().mScore;
            TextToDisplay = "Score \n" + Score.ToString();
        }
        UpdateAllInputScore();
    }
	
    void UpdateAllInputScore()
    {
        Text.SetText(TextToDisplay);
    }
}
