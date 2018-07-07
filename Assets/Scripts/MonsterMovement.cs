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

    private GameObject[] players;
	public int MoveSpeed = 3;
	public int RotationSpeed = 5;

	public float Range = 10f;

	public float Stop = 5f;

	private Animation animation;

	// Use this for initialization
	void Start () {
		this.animation  = this.GetComponent<Animation>();
        if(hasAuthority)
		    this.SetCurrentAnimation(AnimationTypes.dance);
	}
	
	// Update is called once per frame
	void Update () {
        if (!hasAuthority)
            return;

		ChasePlayers();
	}

	private void SetCurrentAnimation(AnimationTypes animationName) {
        RpcSetCurrentAnimation(animationName);
	}

    

    [ClientRpc]
    private void RpcSetCurrentAnimation(AnimationTypes animationName)
    {
        if(this.animation == null)
            this.animation = this.GetComponent<Animation>();

        if (!this.animation.IsPlaying(animationName.ToString()))
        {
            this.animation.Play(animationName.ToString(), PlayMode.StopSameLayer);
        }
    }

	void ChasePlayers() {
        if (!hasAuthority)
            return;

		this.players = GameObject.FindGameObjectsWithTag("Player");
		if (this.players.Length == 0) {
			this.SetCurrentAnimation(AnimationTypes.dance);
			return;
		}
		
		var player = FindClosestPlayer(); 

		var distance = GetPlayerDistance( GetPlayerAvatar( player) );

		if (distance<= this.Range) {

			FacePlayer( GetPlayerAvatar(player));

			//move towards player.  might need to use something else
			if (distance > Stop) {
				RunTowardsPlayer( GetPlayerAvatar( player));
			}
			else {
				AttackPlayer(player);
			}

			return;
		}

		//player is too far away, just wait 
		this.SetCurrentAnimation(AnimationTypes.waitingforbattle);

	}

	private GameObject FindClosestPlayer() {
		//may need to only run this every few seconds for performance reasons
		//TODO: check game delta time

		//loop thru players and find the nearest one
		var lastPlayerIndex = -1;
		var lastDistance = 10000000.0f;

		for (var i = 0;i<this.players.Length;i++) {
			var possibleTarget = this.players[i];
			var distance = Vector3.Distance(this.transform.position, possibleTarget.transform.position);
			if (distance < lastDistance) {
				lastDistance = distance;
				lastPlayerIndex = i;
			}
		}

		//all the players are gone?
		if (lastPlayerIndex == -1) {
			return null;
		}

		//grab the playerAVatar from the clostest tagged network object
		var closestPlayer = this.players[lastPlayerIndex];//.GetComponent<NetworkPlayerConnection>().PlayerAvatar; //TODO: find nearest player
		return closestPlayer;
	}

	private GameObject GetPlayerAvatar(GameObject rootPlayerObject) {
		return rootPlayerObject.GetComponent<NetworkPlayerConnection>().PlayerAvatar;
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


	public float AttackCoolDownTime = 1.5f;
	private float lastAttackTime = 0.0f;

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

	public float AttackDamage = 10.0f;
	private IEnumerator DealDamage(GameObject player) {
		yield return new  WaitForSeconds(0.5f);

		var playerHeath = player.GetComponent<NetworkHealthController>();
		playerHeath.TakeDamage( AttackDamage);

		yield return null;


	}
}
