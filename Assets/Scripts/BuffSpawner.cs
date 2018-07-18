using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BuffSpawner : NetworkBehaviour {

    // Add all possible buff prefabs to this array
    public GameObject[] buffsAvailable;

    Transform[] BuffSpawnTransforms;


	// Use this for initialization
	void Start () {
        if (!isServer)
            return;

        if (buffsAvailable.Length > 0)
        {
            BuffSpawnTransforms = GetComponentsInChildren<Transform>();

            foreach (Transform buffLocation in BuffSpawnTransforms)
            {
                // Randomly picks from the array buffsAvailable with equal probability
                int index = Random.Range(0, buffsAvailable.Length);

                GameObject createdBuff = Instantiate(buffsAvailable[index], buffLocation.transform.position, Quaternion.identity);
                NetworkServer.Spawn(createdBuff);
            }
        } else { Debug.Log("No buffs have been assigned to buffsAvailable array in BuffSpawner"); }
        
	}
	
}
