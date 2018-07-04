using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSpawner : MonoBehaviour {

    public GameObject[] buffsAvailable;

    Transform[] BuffSpawnTransforms;


	// Use this for initialization
	void Start () {
        BuffSpawnTransforms = GetComponentsInChildren<Transform>();

        foreach (Transform buffLocation in BuffSpawnTransforms)
        {
            int index = Random.Range(0, buffsAvailable.Length);

            Instantiate(buffsAvailable[index], buffLocation.transform.position, Quaternion.identity);
        }
	}
	
}
