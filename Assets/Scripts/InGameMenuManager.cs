using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class InGameMenuManager : MonoBehaviour
{

    public enum MenuState { hidden, active }

    private MenuState state;
    private Animator animator;
    private HUDManager hudManager;
    private float openTime = 1f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenMenu(HUDManager hud)
    {
        hudManager = hud;
        animator.SetTrigger("appear");
        StartCoroutine(WaitForOpen());
    }

    public void CloseMenu()
    {
        state = MenuState.hidden;
        Cursor.visible = false;
        animator.SetTrigger("hide");
        hudManager.CloseMenu();
    }

    public void QuitButton()
    {
        if (state != MenuState.active)
            return;

        //TODO link to proper quitting routine.
    }

    public void ResumeButton()
    {
        if (state != MenuState.active)
            return;

        CloseMenu();
    }


    private IEnumerator WaitForOpen()
    {
        yield return new WaitForSeconds(openTime);
        Cursor.visible = true; 
        state = MenuState.active;
    }
}
