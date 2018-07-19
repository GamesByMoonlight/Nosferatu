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

    [SerializeField]
    [Header("This will override x rotation angle of BulletSpawn from inspector")]
    private float maxBulletAimAngle = -10f;
    [SerializeField]
    [Header("Denominator for % of maxBulletAimAngle to use")]
    private float baseProjectileDistance = 30f;
    [SerializeField]
    [Header("Denominator for % of baseProjectileDistance to use")]
    private float baseProjectileSpeed = 30f;

    private bool playerFired { get { return Mathf.Abs(Input.GetAxis(FireAxis)) > Mathf.Epsilon && Time.time - cooldown > WeaponAttributes.FireRate; } }

    private float cooldown;
    private bool bulletPoolSpawned = false;
    private BulletController[] BulletPool;
    private int nextBulletIndex = 0;

    

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
            Aim();
            FireOnNetwork(BulletSpawn.position, BulletSpawn.forward);
        }
	}

    void Aim()
    {
        RaycastHit target;
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out target, 100f, ~0, QueryTriggerInteraction.Ignore);
        BulletSpawn.transform.LookAt(target.point);

        var distance = target.distance;

        if(distance < Mathf.Epsilon)
        {
            var pointAtMaxDistance = Camera.main.transform.position + Camera.main.transform.forward * 100f;
            BulletSpawn.transform.LookAt(pointAtMaxDistance);
            distance = baseProjectileDistance;
        }
        distance = distance > baseProjectileDistance ? baseProjectileDistance : distance;

        var adjustAnglePercent = (distance / (baseProjectileDistance * (WeaponAttributes.ProjectileSpeed / baseProjectileSpeed)));
        BulletSpawn.transform.Rotate(Vector3.right, adjustAnglePercent * maxBulletAimAngle);
    }

    void FireOnNetwork(Vector3 position, Vector3 direction)
    {
        var x = Mathf.FloatToHalf(position.x);  
        var y = Mathf.FloatToHalf(position.y);
        var z = Mathf.FloatToHalf(position.z);
        var dirX = Mathf.FloatToHalf(direction.x);
        var dirY = Mathf.FloatToHalf(direction.y);
        var dirZ = Mathf.FloatToHalf(direction.z);

        Fire(x, y, z, dirX, dirY, dirZ);
        CmdFire(x, y, z, dirX, dirY, dirZ);
    }

    void Fire(ushort x, ushort y, ushort z, ushort dirX, ushort dirY, ushort dirZ)
    {
        var position = new Vector3(Mathf.HalfToFloat(x), Mathf.HalfToFloat(y), Mathf.HalfToFloat(z));
        var direction = new Vector3(Mathf.HalfToFloat(dirX), Mathf.HalfToFloat(dirY), Mathf.HalfToFloat(dirZ));

        BulletPool[nextBulletIndex++ % BulletPool.Length].Fired(position, direction * WeaponAttributes.ProjectileSpeed);
    }

    [Command]
    void CmdFire(ushort x, ushort y, ushort z, ushort dirX, ushort dirY, ushort dirZ)
    {
        RpcFire(x, y, z, dirX, dirY, dirZ);
    }

    [ClientRpc]
    void RpcFire(ushort x, ushort y, ushort z, ushort dirX, ushort dirY, ushort dirZ)
    {
        if (!isLocalPlayer)
            Fire(x, y, z, dirX, dirY, dirZ);
    }
}
