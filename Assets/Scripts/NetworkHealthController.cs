using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Attach to root NetworkBehaviour gameobject to control health for that object.  Must setup RectTransform in editor and Attributes in code.
/// </summary>
public class NetworkHealthController : NetworkBehaviour {
    private Attributes attributes;
    public Attributes ForGameObject { get { return attributes; }  set { attributes = value; CurrentHealth = attributes.CurrentHealth; } }
    public RectTransform HealthBar;
    public CanvasGroup cg;

    //private HUDManager hudManager;


    [SyncVar(hook = "OnChangeHealth")]
    private float CurrentHealth = 100f;

    public override void OnStartLocalPlayer()
    {
        cg.alpha = 0f;
    }

    private void Start()
    {
        GameManager.Instance.AttributesUpdatedFor.AddListener(AttributesUpdateListener);
    }

    public void TakeDamage(float amount)
    {
        if (!isServer)
            return;

        var health = ForGameObject.CurrentHealth;
        health -= amount;
        CurrentHealth = health;

        if(CurrentHealth < 0)
        {
            GameManager.Instance.EntityDiedEvent.Invoke(gameObject);
        }


    }

    void OnChangeHealth(float updatedHealth)
    {
        if (attributes == null || ForGameObject == null)
            return;

        UpdateHealthBar(updatedHealth);
            
        ForGameObject.CurrentHealth = updatedHealth;
    }

    void AttributesUpdateListener(GameObject player)
    {
        UpdateHealthBar(ForGameObject.CurrentHealth);
    }

    void UpdateHealthBar(float updatedHealth)
    {
        if (HealthBar != null)
        {
            var health = (updatedHealth / attributes.MaxHealth) * 100f;
            HealthBar.sizeDelta = new Vector2(health, HealthBar.sizeDelta.y);
        }
    }
}
