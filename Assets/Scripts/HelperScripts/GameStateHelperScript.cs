using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateHelperScript : MonoBehaviour {
    public Text GameStateText;

    private string message;
	// Use this for initialization
	void Start () {
        message = GameStateText.text;
	}
	
	// Update is called once per frame
	void Update () {
		switch(GameManager.Instance.CurrentState)
        {
            case GameState.ReadyToStart:
                GameStateText.text = message + "Stopped";
                break;
            case GameState.Running:
                GameStateText.text = message + "Running";
                break;
            case GameState.VampireWinsByElimination:
                GameStateText.text = "Game over.  Vampire wins by eliminating adventurers.";
                break;
            case GameState.VampireWinsByTime:
                GameStateText.text ="Game over.  Vampire wins by time.";
                break;
            case GameState.AdventurersWin:
                GameStateText.text = "Match ended.  Adventurers win!";
                break;
        }
	}
}
