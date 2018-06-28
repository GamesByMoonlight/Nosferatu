using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HealthForPlayer : NetworkBehaviour {
    public const int MAX_HEALTH = 100;

    [SyncVar(hook = "OnChangeHealth")]
    public int CurrentHealth = 100;
    public RectTransform HealthBar;
    public bool DestroyOnDeath = false;

    private NetworkStartPosition[] spawnPoints;

    void Start()
    {
        if (isLocalPlayer)
        {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        }
    }

	public void TakeDamage(int amount)
    {
        if (!isServer)
            return;

        CurrentHealth -= amount;

        if(CurrentHealth < 0)
        {
            if (DestroyOnDeath)
            {
                Destroy(gameObject);
            }
            else
            {
                CurrentHealth = MAX_HEALTH;
                RpcRespawn();
            }
        }


    }

    void OnChangeHealth(int updatedHealth)
    {
        HealthBar.sizeDelta = new Vector2(updatedHealth, HealthBar.sizeDelta.y);
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            Vector3 spawnPoint = Vector3.zero;

            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            }

            transform.position = spawnPoint;
        }
            
    }
}
