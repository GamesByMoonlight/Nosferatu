using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonsterMovement : MonoBehaviour {

	private GameObject[] players;
	public int MoveSpeed = 3;
	public int RotationSpeed = 5;

	public float Range = 10f;

	public float Stop = 5f;



	private Animation _animation;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		this._animation  = this.GetComponent<Animation>();
		this.SetCurrentAnimation("dance");

		ChasePlayers();

	}

	private void SetCurrentAnimation(string animationName) {

		if (!this._animation.IsPlaying(animationName)) {
			this._animation.Play(animationName);
		}
	}

	void ChasePlayers() {
		this.players = GameObject.FindGameObjectsWithTag("Player");
		if (this.players.Length == 0) {
			return;
		}
		

		var player = this.players[0]; //TODO: find nearest player

		var distance = Vector3.Distance(this.transform.position, player.transform.position);

		if (distance<= this.Range) {

			//rotate towards player
			var rotation = Quaternion.LookRotation(player.transform.position - this.transform.position);
			this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, RotationSpeed*Time.deltaTime);			

			//move towards player.  might need to use something else
			if (distance > Stop) {
				this.SetCurrentAnimation("run");

				this.transform.position += this.transform.forward * MoveSpeed * Time.deltaTime;
				// this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, MoveSpeed* Time.deltaTime);
			}
			else {
				this.SetCurrentAnimation("attack");
				

			}
		}
	}
}
