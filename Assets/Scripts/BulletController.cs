using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SyncObjectManager;

public class BulletController : NetworkBehaviour {
    [SyncVar]
    public NetworkInstanceId playerNetID;

    private float Attack = 20f;
    private float baseScale = .3f / 20f;
    private Rigidbody rb;

    [SerializeField]
    private GameObject bulletGameObject;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    private void Start()
    {
        transform.SetParent(Manager.Instance.ConnectedObjects[playerNetID].transform);
        SetChildrenActive(false);
    }

    private void SetChildrenActive(bool active)
    {
        bulletGameObject.gameObject.SetActive(active);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;

        var health = other.gameObject.GetComponentsInParent<NetworkHealthController>();
        if (health.Length > 0 && health[0].gameObject != transform.parent.gameObject)
        {
            // an isServer check is made in TakeDamage() also
            health[0].TakeDamage(Attack);
            RpcSetActive(false);
        }
    }

    [ClientRpc]
    private void RpcSetActive(bool active)
    {
        CancelInvoke();
        SetChildrenActive(active);
    }

    [ClientRpc]
    public void RpcFired(Vector3 position, Vector3 velocity, float attack)
    {
        Attack = attack;

        transform.position = position;
        rb.velocity = velocity;
        transform.localScale = new Vector3(baseScale * Attack, baseScale * Attack, baseScale * Attack);

        SetChildrenActive(true);
        CancelInvoke();
        Invoke("Shutoff", 10f);
    }

    private void Shutoff()
    {
        SetChildrenActive(false);
    }
}
