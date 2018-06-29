using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using SyncObjectManager;
using System;

public class NetworkPlayerControllerExample : NetworkBehaviour
{
    public Transform BulletSpawn;
    public Transform CameraPosition;
    public GameObject BulletPrefab;
    public float Speed = 30f;
    public float BulletSpeed = 6f;

    public override void OnStartLocalPlayer()
    {
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.position = CameraPosition.position;
        Camera.main.transform.rotation = transform.rotation;
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        UpdateMovement();

        if (Input.GetButtonDown("Fire1"))
            CmdFire();
    }    

    void UpdateMovement()
    {
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * Speed;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);
    }

    [Command]
    void CmdFire()
    {
        var bullet = Instantiate(BulletPrefab, BulletSpawn.position, BulletSpawn.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * BulletSpeed;
        NetworkServer.Spawn(bullet);
        Destroy(bullet, 2f);
    }
}
