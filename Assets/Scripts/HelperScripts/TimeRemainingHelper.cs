using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeRemainingHelper : MonoBehaviour {
    public Text TimeRemainingText;

    string message;

	void Start () {
        message = TimeRemainingText.text;
	}
	
	// Update is called once per frame
	void Update () {
        if (GameManager.Instance == null)
            return;
        TimeRemainingText.text = message + string.Format("{0:00}:{1:00}", ((int)GameManager.Instance.CurrentTime) / 60, GameManager.Instance.CurrentTime % 60);
	}
}
