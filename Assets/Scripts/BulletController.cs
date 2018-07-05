using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletController : NetworkBehaviour {
    private float Attack = 20f;
    private float baseScale = .3f / 20f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        gameObject.SetActive(false);
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

        gameObject.SetActive(false);
    }

    [ClientRpc]
    public void RpcFired(Vector3 position, Vector3 velocity, float attack)
    {
        transform.position = position;
        rb.velocity = velocity;
        transform.localScale = new Vector3(baseScale * Attack, baseScale * Attack, baseScale * Attack);
        gameObject.SetActive(true);
        Invoke("Shutoff", 10f);
    }

    private void Shutoff()
    {
        gameObject.SetActive(false);
    }
}
