using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum HUDState { none, gameStart, gamePlay, gameOver } //Doing this for now. Not the best method, probably.

[RequireComponent(typeof(Animator))]
public class HUDManager : MonoBehaviour
{

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

    private void OpenHUD()
    {
        animator.SetTrigger("appear");
    }


}
