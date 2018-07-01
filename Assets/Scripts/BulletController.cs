using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletController : NetworkBehaviour {
    [SyncVar]
    [HideInInspector] public float Attack = 1f;
    [SyncVar]
    [HideInInspector] public float Speed = 10f;

    private float baseScale = .3f / 20f;

    private void Start()
    {
        transform.localScale = new Vector3(baseScale * Attack, baseScale * Attack, baseScale * Attack);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var health = collision.gameObject.GetComponentsInParent<NetworkHealthController>();
        if (health.Length > 0)
        {
            // an isServer check is made in TakeDamage()
            health[0].TakeDamage(Attack);
        }

        Destroy(gameObject);
    }
}
