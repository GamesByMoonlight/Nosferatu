using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class MonsterMovement : NetworkBehaviour {
    private enum AnimationTypes : byte
    {
        dance,
        run,
        attack,
        waitingforbattle,
    }

    private List<NetworkPlayerConnection> players = new List<NetworkPlayerConnection>(5);
    public AttributesObject MonsterAttributes;
    private Attributes attributes = new Attributes();

    public int MoveSpeed { get { return (int)attributes.ForwardSpeed; } } // = 3;
	public int RotationSpeed { get { return (int)attributes.StrafeSpeed; } } // = 5;
    public float DetectionRange { get { return attributes.ProjectileRange; } } // = 10f;
    public float FightingRange { get { return attributes.ProjectileSpeed; } } // = 5f;
    public float AttackDamage { get { return attributes.Attack; } } // = 10.0f;
    public float AttackCoolDownTime { get { return attributes.FireRate; } } // = 1.5f;
    public float CheckForPlayerEvery = 1f;

    private float lastAttackTime = 0.0f;
    private float LastPlayerCheckTime = -100f; // Guarantee a check on first frame
    private GameObject closestPlayer;
    private Animation _animation;

	// Use this for initialization
	void Start () {
		this._animation  = this.GetComponent<Animation>();
        if(hasAuthority)
		    this.SetCurrentAnimation(AnimationTypes.dance);
        MonsterAttributes.Initialize(attributes);
	}
	
	// Update is called once per frame
	void Update () {
        if (!hasAuthority)
            return;

		ChasePlayers();
	}


    // ------- On Trigger Enter/Exit definitions ----------------------------------------------------------------
    // Using Colliders, we can detect when a player is close enough.  This is far less expensive then using GameObject.FindGameObjectsWithTag every frame.
    //
    private void OnTriggerEnter(Collider other)
    {
		//check that we have a parent when we collide w/ something
		if (other.gameObject.transform.parent == null) {
			return;
		}
        var player = other.gameObject.transform.parent.GetComponent<NetworkPlayerConnection>();
        if(player != null && !players.Contains(player))
        {
            players.Add(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
		//check that we have a parent when we collide w/ something
		if (other.gameObject.transform.parent == null) {
			return;
		}
        var player = other.gameObject.transform.parent.GetComponent<NetworkPlayerConnection>();
        if (player != null && players.Contains(player))
        {
            players.Remove(player);
        }
    }
    // ------------------------------------------------------------------------------------------------------------
    // ----


    private void SetCurrentAnimation(AnimationTypes animationName) {
        // Put this check on the server so we don't send network traffic every frame, from every skeleton 
        if (!this._animation.IsPlaying(animationName.ToString()))
            RpcSetCurrentAnimation(animationName);
	}

    

    [ClientRpc]
    private void RpcSetCurrentAnimation(AnimationTypes animationName)
    {
        if(this._animation == null)
            this._animation = this.GetComponent<Animation>();

        if (!this._animation.IsPlaying(animationName.ToString()))
        {
            this._animation.Play(animationName.ToString(), PlayMode.StopSameLayer);
        }
    }

	void ChasePlayers() {
        if (!hasAuthority)
            return;

		//this.players = GameObject.FindGameObjectsWithTag("Player"); // Too expensive an operation to use every frame
		if (this.players.Count == 0) {
			this.SetCurrentAnimation(AnimationTypes.dance);
			return;
		}
		
		var player = FindClosestPlayerAvatar(); 

		var distance = GetPlayerDistance(  player );

		if (distance<= this.DetectionRange) {

			FacePlayer( player );

			//move towards player.  might need to use something else
			if (distance > FightingRange) {
				lastAttackTime = 100;
				RunTowardsPlayer( player );
			}
			else {
				AttackPlayer(player);
			}

			return;
		}

		//player is too far away, just wait 
		this.SetCurrentAnimation(AnimationTypes.waitingforbattle);

	}

	private GameObject FindClosestPlayerAvatar() {
        //may need to only run this every few seconds for performance reasons
        if (Time.time - LastPlayerCheckTime < CheckForPlayerEvery)
            return closestPlayer;
        LastPlayerCheckTime = Time.time;

		//loop thru players and find the nearest one
		var lastPlayerIndex = -1;
		var lastDistance = 10000000.0f;

		for (var i = 0; i<this.players.Count; i++) {
			var possibleTarget = this.players[i];
			var distance = Vector3.Distance(this.transform.position, possibleTarget.PlayerAvatar.transform.position);
			if (distance < lastDistance) {
				lastDistance = distance;
				lastPlayerIndex = i;
			}
		}

		//all the players are gone?
		if (lastPlayerIndex == -1) {
			return null;
		}

		//grab the player avatar from the clostest tagged network object
		closestPlayer = this.players[lastPlayerIndex].PlayerAvatar;
		return closestPlayer;
	}

	private float GetPlayerDistance(GameObject player) {
		var distance = Vector3.Distance(this.transform.position, player.transform.position);
		return distance;
	}

	private void FacePlayer(GameObject player) {
		//rotate towards player
		var rotation = Quaternion.LookRotation(player.transform.position - this.transform.position);
		this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, RotationSpeed*Time.deltaTime);			

	}

	private void RunTowardsPlayer(GameObject player) {
		this.SetCurrentAnimation(AnimationTypes.run);
		// this.transform.position += this.transform.forward * MoveSpeed * Time.deltaTime;
		this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, MoveSpeed* Time.deltaTime);
	}


	

	private void AttackPlayer(GameObject player) {
		if (!this.isServer) {
			return;
		}


		if (lastAttackTime <= AttackCoolDownTime) {
			lastAttackTime += Time.deltaTime;
			return;
		}

		lastAttackTime = 0.0f;

		this.SetCurrentAnimation(AnimationTypes.attack);
		StartCoroutine(DealDamage(player));
	}

	
	private IEnumerator DealDamage(GameObject playerAvatar) {
		yield return new  WaitForSeconds(0.5f);

		var playerHeath = playerAvatar.transform.parent.GetComponent<NetworkHealthController>();
		playerHeath.TakeDamage(AttackDamage);

		yield return null;


	}
}
