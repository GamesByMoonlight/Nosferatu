using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
        var health = collision.gameObject.GetComponentsInParent<NetworkHealthController>();
        if (health != null)
        {
            health[0].TakeDamage(10);
        }

        Destroy(gameObject);
    }
}
