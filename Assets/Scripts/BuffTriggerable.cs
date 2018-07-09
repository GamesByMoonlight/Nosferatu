using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTriggerable : BuffBase {

    bool checkForPlayerInput = false;
    NetworkPlayerConnection player;

    private void OnTriggerEnter(Collider collision)
    {
        player = collision.gameObject.GetComponentInParent<NetworkPlayerConnection>();

        if (player != null && player.isLocalPlayer)
        {
            checkForPlayerInput = true;
        }
    }

    private void Update()
    {
        if (checkForPlayerInput)
        {
            //if (Input.GetButtonDown("Submit")) 
            if (Input.GetKeyDown(KeyCode.E)) 
            {
                player.ModifyAttributes(Attributes);
                //Debug.Log("Buff Triggered");
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        var player = collision.gameObject.GetComponentInParent<NetworkPlayerConnection>();

        if (player != null && player.isLocalPlayer)
        {
            checkForPlayerInput = false;
            player = null;
        }
    }
}
