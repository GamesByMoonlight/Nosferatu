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
    private bool bulletPoolSpawned = false;
    private BulletController[] BulletPool;
    private int nextBulletIndex = 0;

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
            var bullet = Instantiate(BulletPrefab).GetComponent<BulletController>();
            bullet.playerNetID = netId;
            NetworkServer.Spawn(bullet.gameObject);
            BulletPool[i] = bullet;
        }

        bulletPoolSpawned = true;
    }

    // Update is called once per frame
    void Update () {
        if (!isLocalPlayer)
            return;


        if(PlayerFire()) 
        {
            cooldown = Time.time;
            CmdFire(BulletSpawn.position, BulletSpawn.forward * WeaponAttributes.ProjectileSpeed, WeaponAttributes.Attack);
        }
	}

    bool PlayerFire()
    {
        return Mathf.Abs(Input.GetAxis(FireAxis)) > Mathf.Epsilon && Time.time - cooldown > WeaponAttributes.FireRate;
    }

    [Command]
    void CmdFire(Vector3 position, Vector3 velocity, float attack)
    {
        BulletPool[nextBulletIndex++ % BulletPool.Length].RpcFired(position, velocity, attack);
        if(nextBulletIndex >= 100000)
        {
            nextBulletIndex = 0;
        }
    }
}
