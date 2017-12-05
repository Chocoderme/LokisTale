using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BrainMovementType {
	MEET_PLAYER,
	ESCAPE,
	DODGE
}

[RequireComponent(typeof(Health))]
public class AIBrain : MonoBehaviour {

	private Health health;

	//private Dictionary<string, int> ThinkingMovementResults = new Dictionary<string, int> ();
	//private Dictionary<string, int> ThinkingAttackResults = new Dictionary<string, int> ();

	private BrainMovementType Movement;
	private AIMoving behaviour;

    public bool isActive = false;
    public bool isReady = false;

	private float timeSinceLastAttack;

	public float attackCooldown = 1.0f;
	public int damage = 20;

	// Use this for initialization
	void Start () {
		health = GetComponent<Health> ();
		behaviour = GetComponent<AIMoving> ();

		timeSinceLastAttack = 0.0f;

        Invoke("setReady", 2f);
	}

    private void setReady()
    {
        isReady = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (isActive)
        {
            Thinking();
            Acting();
        }

		timeSinceLastAttack += Time.deltaTime;
	}

	void Thinking() {
		int tmpValue;

		// Meet player ?
		int bestValue = ShouldMeetPlayer ();
		BrainMovementType bestMovement = BrainMovementType.MEET_PLAYER;
		//Debug.Log (" player : " + bestValue);
		// Escape ?
		tmpValue = ShouldEscape ();
		if (tmpValue > bestValue) {
			bestValue = tmpValue;
			bestMovement = BrainMovementType.ESCAPE;
		}
		//Debug.Log (" escape : " + bestValue);
		// DODGE BULLET ?

		Movement = bestMovement;
	}

	int ShouldMeetPlayer () {
		Health.HealthInfo healthInfos = health.GetHealthInfos ();
		float maxLife = healthInfos.MaxHealth;
		float currentLife = healthInfos.currentHealth;
		//Debug.Log (currentLife);
		//Debug.Log (maxLife / 20.0f * 18.0f);
		// 90%
		float lifePercent = (currentLife * 100) / maxLife;

		if (lifePercent >= 90.0f) {
			return 20;
		}
		// 75%
		if (lifePercent >= 75.0f) {
			return 10;
		}
		// 50%
		if (lifePercent >= 50.0f) {
			return 5;
		}
		// 25%
		if (lifePercent >= 25.0f) {
			return 2;
		}

		return 0;
	}

	int ShouldEscape() {
		Health.HealthInfo healthInfos = health.GetHealthInfos ();
		float maxLife = healthInfos.MaxHealth;
		float currentLife = healthInfos.currentHealth;
		float lifePercent = (currentLife * 100) / maxLife;

		// 25%
		if (lifePercent >= 25.0f) {
			return 20;
		}
		// 50%
		if (lifePercent >= 50.0f) {
			return 10;
		}
		// 75%
		if (lifePercent >= 75.0f) {
			return 5;
		}
		// 90%
		if (lifePercent >= 90.0f) {
			return 2;
		}

		return 0;
	}

	void Acting() {
		// MOVE
		if (Movement == BrainMovementType.MEET_PLAYER) {
			behaviour.MoveTowardPlayer ();
		}

		if (Movement == BrainMovementType.ESCAPE) {
			behaviour.EscapePlayer ();
			Debug.Log("escape");
		}

		// ATTACK;
	}

	void OnCollisionStay(Collision obj) {
		if (obj.gameObject.tag == "Player") {

			if (timeSinceLastAttack >= attackCooldown) {
				timeSinceLastAttack = 0.0f;
				StartCoroutine (behaviour.Attack (obj.gameObject, damage));
			}
		}
	}
}
