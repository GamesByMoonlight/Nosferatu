using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum HUDState { none, gameStart, gamePlay, inGameMenu, gameOver } //Doing this for now. Not the best method, probably.

[RequireComponent(typeof(Animator))]
public class HUDManager : MonoBehaviour
{

    public Text timeRemainingTextField;
    public Text healthStatsTextField;
    public Image healthBarImage;
    public Image compass;

    public bool testingOffline;

    private HUDState state;
    private Animator animator;
    private GameObject localPlayer;
    private NetworkPlayerConnection localPlayerConnection;
    private InGameMenuManager menuManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        menuManager = FindObjectOfType<InGameMenuManager>();
    }

    private void Start()
    {
        state = HUDState.gameStart;
        OpenHUD();
    }

    private void Update()
    {
        CheckMenu();
        UpdateTime();
        UpdateCompass();
        UpdateHealth();
    }

    /// <summary>
    /// Checks for localPlayer then updates health via other method
    /// </summary>
    private void UpdateHealth()
    {
        if (localPlayerConnection == null)
        {
            InitializeNetworkedItems();
            return; //Try again next frame
        }

        UpdateHealth(localPlayerConnection.PlayerAttributes.CurrentHealth, localPlayerConnection.PlayerAttributes.MaxHealth);
    }

    /// <summary>
    /// Updates when player takes damage or increases max health.
    /// </summary>
    /// <param name="health"></param>
    /// <param name="maxHealth"></param>
    private void UpdateHealth(float health, float maxHealth)
    {
        healthStatsTextField.text = health + "/" + maxHealth + " HP";
        healthBarImage.rectTransform.localScale = new Vector2(Mathf.Clamp01(((float)health / (float)maxHealth)), healthBarImage.rectTransform.localScale.y);
    }


    /// <summary>
    /// Closes menu for this player and sets HUD state appropriately.
    /// </summary>
    public void CloseMenu()
    {
        if (state == HUDState.inGameMenu)
            state = HUDState.gamePlay;
    }

    private void InitializeNetworkedItems()
    {
        if (testingOffline) //This is just to find a player when testing in offline scenes, etc.
        {
            FPSMouseLookController fmlc = FindObjectOfType<FPSMouseLookController>();
            if (fmlc != null)
            {
                localPlayer = fmlc.gameObject;
                Debug.LogWarning("USING PLAYER FOUND IN SCENE, NOT FROM GAME MANAGER. FIX NOT IN OFFLINE TEST MODE.");
            }
        }
        else
        {
            localPlayer = GameManager.Instance.LocalPlayer;
            if(localPlayer != null)
            {
                localPlayerConnection = localPlayer.GetComponent<NetworkPlayerConnection>();
            }
        }
    }

    private void UpdateTime()
    {
        if (GameManager.Instance == null)
            return;

        timeRemainingTextField.text = string.Format("{0:00}:{1:00}", ((int)GameManager.Instance.CurrentTime) / 60, GameManager.Instance.CurrentTime % 60);
    }

    /// <summary>
    /// Rotates the compass so that it is always pointing "north" (vector3.forward of the world for now)
    /// </summary>
    private void UpdateCompass()
    {
        if (localPlayer == null)
        {
            InitializeNetworkedItems();
            return; //Try again next frame
        }

        float playerAngle = localPlayer.transform.rotation.eulerAngles.y;
        compass.transform.rotation = Quaternion.Euler(compass.transform.rotation.eulerAngles.x, compass.transform.rotation.eulerAngles.y, playerAngle);
    }

    private void OpenHUD()
    {
        animator.SetTrigger("appear");
        state = HUDState.gamePlay;
    }

    private void CheckMenu()
    {
        if (state != HUDState.gamePlay)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            state = HUDState.inGameMenu;
            menuManager.OpenMenu(this);
        }
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
