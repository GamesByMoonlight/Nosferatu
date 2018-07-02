﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkFireController : NetworkBehaviour {
    public string FireAxis = "Fire1";
    public GameObject BulletPrefab;
    public Transform BulletSpawn;
    [SerializeField]
    public Attributes WeaponAttributes;

    private float cooldown;

    private void Awake()
    {
        cooldown = Time.time;
    }

    // Update is called once per frame
    void Update () {
        if (!isLocalPlayer)
            return;


        if(PlayerFire()) 
        {
            cooldown = Time.time;
            CmdFire(BulletSpawn.position, BulletSpawn.rotation);
        }
	}

    bool PlayerFire()
    {
        return Mathf.Abs(Input.GetAxis(FireAxis)) > Mathf.Epsilon && Time.time - cooldown > WeaponAttributes.FireRate;
        //return Input.GetButtonDown(FireAxis) && Time.time - cooldown > WeaponAttributes.FireRate
    }

    [Command]
    void CmdFire(Vector3 position, Quaternion direction)
    {
        var bullet = Instantiate(BulletPrefab, position, direction);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * WeaponAttributes.ProjectileSpeed;
        bullet.GetComponent<BulletController>().Attack = WeaponAttributes.Attack;
        bullet.GetComponent<BulletController>().Speed = WeaponAttributes.ProjectileSpeed;
        NetworkServer.Spawn(bullet);
        Destroy(bullet, WeaponAttributes.ProjectileRange);
    }
}
