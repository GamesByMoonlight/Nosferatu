using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkFireController : NetworkBehaviour {
    public string FireAxis = "Fire1";
    public GameObject BulletPrefab;
    public Transform BulletSpawn;
    [SerializeField]
    private int BulletPoolLength = 20;
    [SerializeField]
    public Attributes WeaponAttributes;

    private bool playerFired { get { return Mathf.Abs(Input.GetAxis(FireAxis)) > Mathf.Epsilon && Time.time - cooldown > WeaponAttributes.FireRate; } }
    private bool alreadyFired { get { return lastIndexFired == nextBulletIndex; } }

    private float cooldown;
    private bool bulletPoolSpawned = false;
    private BulletController[] BulletPool;
    private int nextBulletIndex = 0;
    private int lastIndexFired = -1;

    private void Awake()
    {
        cooldown = Time.time;
    }

    private void Start()
    {
        if (!isServer)
        {
            StartCoroutine(WaitForBulletsToSpawn(BulletPoolLength));
        }
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

    IEnumerator WaitForBulletsToSpawn(int length)
    {
        BulletController[] bullets = GetComponentsInChildren<BulletController>();

        while (bullets.Length != length)
        {
            yield return new WaitForSeconds(.1f);
            bullets = GetComponentsInChildren<BulletController>();
        }

        BulletPool = bullets;
    }

    // Update is called once per frame
    void Update () {
        if (!isLocalPlayer)
            return;


        if(playerFired) 
        {
            cooldown = Time.time;
            var position = BulletSpawn.position;
            var velocity = BulletSpawn.forward * WeaponAttributes.ProjectileSpeed;

            Fire(position, velocity);
            CmdFire(position, velocity);
        }
	}

    void Fire(Vector3 position, Vector3 velocity)
    {
        if (alreadyFired)
            return;

        lastIndexFired = nextBulletIndex;
        BulletPool[nextBulletIndex % BulletPool.Length].Fired(position, velocity);
    }

    [Command]
    void CmdFire(Vector3 position, Vector3 velocity)
    {
        //BulletPool[nextBulletIndex++ % BulletPool.Length].Fired(position, velocity);    // Set server bullets to fire immediately 
        Fire(position, velocity);
        RpcFire(position, velocity);    // Then update clients
    }

    [ClientRpc]
    void RpcFire(Vector3 position, Vector3 velocity)
    {
       // //position = GetComponent<NetworkSyncMessenger>().SyncTransform.position - position;
       // Debug.Log("****************************");
       // var nVector = new Vector3(Mathf.HalfToFloat(Mathf.FloatToHalf(position.x)), Mathf.HalfToFloat(Mathf.FloatToHalf(position.y)), Mathf.HalfToFloat(Mathf.FloatToHalf(position.z)));
       //// Debug.Log("Original: " + position.ToString("F4") + " | Net: " + nVector.ToString("F4"));
       // Debug.Log((position - nVector).ToString("F4"));



        if (alreadyFired)
        {
            ++nextBulletIndex;
            return;
        }

        

        Fire(position, velocity);
        ++nextBulletIndex;
    }
}
