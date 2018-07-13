using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BuffAutoTrigger : BuffBase {

    public float bombDamage = 50;

	// Use this for initialization
	void Start () {
		
	}

    private void OnTriggerEnter(Collider collision)
    {
        var player = collision.gameObject.GetComponentInParent<NetworkPlayerConnection>();
        var playerhealth = collision.gameObject.GetComponentInParent<NetworkHealthController>();

        // Check for being a valid object, is a local player, and is on the opposite team.
        if (player != null && 
            player.isLocalPlayer && 
            player.PlayerAttributes.Team != this.Attributes.attributes.Team &&
            this.isServer)
        {
            playerhealth.TakeDamage(bombDamage);
            Debug.Log("Debuff Triggered");
            Destroy(this.gameObject);
        }
    }
}
