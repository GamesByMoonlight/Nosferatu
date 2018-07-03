using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum HUDState { none, gameStart, gamePlay, gameOver } //Doing this for now. Not the best method, probably.

[RequireComponent(typeof(Animator))]
public class HUDManager : MonoBehaviour
{

    public Text timeRemainingTextField;
    public Text healthStatsTextField;
    public Image healthBarImage;

    private HUDState state;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        state = HUDState.gameStart;
        OpenHUD();
    }

    private void Update()
    {
        UpdateTime();
    }

    /// <summary>
    /// Updates when player takes damage or increases max health.
    /// </summary>
    /// <param name="health"></param>
    /// <param name="maxHealth"></param>
    public void UpdateHealth(int health, int maxHealth)
    {
        healthStatsTextField.text = health + "/" + maxHealth + " HP";
        healthBarImage.rectTransform.localScale = new Vector2(Mathf.Clamp01(((float)health / (float)maxHealth)), healthBarImage.rectTransform.localScale.y);
    }

    /// <summary>
    /// Opens menu for this player. Does not pause game or impact other players.
    /// </summary>
    public void OpenMenu()
    {

    }

    private void UpdateTime()
    {
        if (GameManager.Instance == null)
            return;

        timeRemainingTextField.text = string.Format("{0:00}:{1:00}", ((int)GameManager.Instance.CurrentTime) / 60, GameManager.Instance.CurrentTime % 60);
    }

    private void OpenHUD()
    {
        animator.SetTrigger("appear");
    }


    #region EditorTestStuff

    #if UNITY_EDITOR
    [ContextMenu("TestHealthUpdate")]
    public void TestHealthUpdate()
    {
        UpdateHealth(Random.Range(0, 100), 100);
    }



    #endif

    #endregion

}
