using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControllerExample : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
        var health = collision.gameObject.GetComponent<HealthForPlayer>();
        if(health != null)
        {
            health.TakeDamage(10);
        }

        Destroy(gameObject);
    }
}
