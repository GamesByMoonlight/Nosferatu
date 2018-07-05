using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkFireController : NetworkBehaviour {
    public string FireAxis = "Fire1";
    public GameObject BulletPrefab;
    public Transform BulletSpawn;
    public int BulletPoolLength = 20;
    [SerializeField]
    public Attributes WeaponAttributes;

    private float cooldown;
    private BulletController[] BulletPool;
    private bool bulletPoolSpawned = false;

    private void Awake()
    {
        cooldown = Time.time;
    }

    public void SpawnBulletPool()
    {
        if (!isServer)
            Debug.LogError("Bullet Pool must be initiated on server only");
        if (bulletPoolSpawned)
            Debug.LogWarning("Bullet pool has already been spawned on server");

        BulletPool = new BulletController[BulletPoolLength];
        for(int i = 0; i < BulletPoolLength; ++i)
        {
            var bullet = Instantiate(BulletPrefab);
            NetworkServer.Spawn(bullet);
            BulletPool[i] = bullet.GetComponent<BulletController>();
        }
    }

    // Update is called once per frame
    void Update () {
        if (!isLocalPlayer)
            return;


        if(PlayerFire()) 
        {
            cooldown = Time.time;
            CmdFire(BulletSpawn);
        }
	}

    bool PlayerFire()
    {
        return Mathf.Abs(Input.GetAxis(FireAxis)) > Mathf.Epsilon && Time.time - cooldown > WeaponAttributes.FireRate;
    }

    [Command]
    void CmdFire(Transform spawn)
    {
        for(int i = 0; i < BulletPool.Length; ++i)
        {
            if(!BulletPool[i].gameObject.activeInHierarchy)
            {
                BulletPool[i].RpcFired(spawn.position, spawn.direction, spawn.forward * WeaponAttributes.ProjectileSpeed, WeaponAttributes.Attack);
                //BulletPool[i].GetComponent<Rigidbody>().velocity = bullet.transform.forward * WeaponAttributes.ProjectileSpeed;
            }
        }
        
        //Destroy(bullet, WeaponAttributes.ProjectileRange);
    }
}
