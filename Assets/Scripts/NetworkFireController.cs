using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkFireController : NetworkBehaviour {
    public string FireAxis = "Fire1";
    public GameObject BulletPrefab;
    public Transform BulletSpawn;
    [HideInInspector] public Attributes WeaponAttributes;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update () {
        if (!isLocalPlayer)
            return;

        if(Mathf.Abs(Input.GetAxis(FireAxis)) > Mathf.Epsilon)
        {
            CmdFire(mainCamera.transform.forward);
        }
	}

    [Command]
    void CmdFire(Vector3 direction)
    {
        var bullet = Instantiate(BulletPrefab, BulletSpawn.position, BulletSpawn.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * WeaponAttributes.ProjectileSpeed;
        NetworkServer.Spawn(bullet);
        Destroy(bullet, 2f);
    }
}
