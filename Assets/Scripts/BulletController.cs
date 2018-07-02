using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletController : NetworkBehaviour {
    [SyncVar]
    public float Attack = 20f;
    [SyncVar]
    public float Speed = 10f;

    private float baseScale = .3f / 20f;

    private void Start()
    {
        transform.localScale = new Vector3(baseScale * Attack, baseScale * Attack, baseScale * Attack);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isServer)
            return;

        var health = collision.gameObject.GetComponentsInParent<NetworkHealthController>();
        if (health.Length > 0)
        {
            // an isServer check is made in TakeDamage() also
            health[0].TakeDamage(Attack);
        }

        Destroy(gameObject);
    }
}
