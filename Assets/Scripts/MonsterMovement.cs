using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonsterMovement : MonoBehaviour {

	private GameObject[] players;
	public int MoveSpeed = 3;
	public int RotationSpeed = 5;

	public float Range = 10f;

	public float Stop = 5f;

	private Animation animation;

	// Use this for initialization
	void Start () {
		this.animation  = this.GetComponent<Animation>();
		this.SetCurrentAnimation("dance");
	}
	
	// Update is called once per frame
	void Update () {
		ChasePlayers();
	}

	private void SetCurrentAnimation(string animationName) {
		if (!this.animation.IsPlaying(animationName)) {
			this.animation.Play(animationName, PlayMode.StopSameLayer);
		}
	}

	void ChasePlayers() {
		this.players = GameObject.FindGameObjectsWithTag("Player");
		if (this.players.Length == 0) {
			this.SetCurrentAnimation("dance");
			return;
		}
		
		var player = FindClosestPlayer();

		var distance = GetPlayerDistance(player);

		if (distance<= this.Range) {

			FacePlayer(player);

			//move towards player.  might need to use something else
			if (distance > Stop) {
				RunTowardsPlayer(player);
			}
			else {
				AttackPlayer(player);
			}

			return;
		}

		//player is too far away, just wait 
		this.SetCurrentAnimation("waitingforbattle");

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
		var closestPlayer = this.players[lastPlayerIndex].GetComponent<NetworkPlayerConnection>().PlayerAvatar; //TODO: find nearest player
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
		this.SetCurrentAnimation("run");
		// this.transform.position += this.transform.forward * MoveSpeed * Time.deltaTime;
		this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, MoveSpeed* Time.deltaTime);
	}

	private void AttackPlayer(GameObject player) {
		this.SetCurrentAnimation("attack");

	}
}
