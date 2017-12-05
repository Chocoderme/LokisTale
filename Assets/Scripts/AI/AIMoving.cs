using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
public class AIMoving : MonoBehaviour {

	public float depth = 0;

	private GameObject player;
	private NavMeshAgent m_Agent;
	private bool isJumping;
	private EntityAnimation mAnimation;
	private bool isAttacking;

	// Use this for initialization
	void Start () {

		if (transform.position.z != depth)
			Debug.LogWarning ("\"" + gameObject.name + "\" is not align on the good depth");

		m_Agent = GetComponent<NavMeshAgent>();
		mAnimation = GetComponent<EntityAnimation> ();
		player = GameObject.FindGameObjectWithTag ("Player");

		/*if (!m_Agent.isOnNavMesh) {
			Debug.LogWarning (m_Agent + " (agent) is not on the nav mesh, auto replacing");
			NavMeshHit hit = new NavMeshHit ();
			m_Agent.FindClosestEdge (out hit);
			transform.position = hit.position;
		}*/


		isJumping = false;
		isAttacking = false;
		//Debug.Log ("test");
		m_Agent.Warp (transform.position);

	}

	void Update() {

		GetComponentInChildren<Animator> ().transform.LookAt (transform.position + transform.forward);

		//Debug.Log (transform.forward);

		//if (Vector3.right)
		//	Debug.Log (transform.forward);//GetComponentInChildren<Animator> ().transform.RotateAround (transform.position, Vector3.up, 20);
	}

	public void	MoveTowardPlayer() {

		if (!player) {
			mAnimation.Idle ();
			return;
		}
		// Idle
		/*if (transform.position.x <= (player.transform.position.x + 2.0f) &&
		    transform.position.x >= (player.transform.position.x - 2.0f)) {

			m_Agent.destination = transform.position;
			mAnimation.Idle ();
		}*/
		
		//else {
			
			// Move
		if (!isJumping && !isAttacking) {
			m_Agent.destination = player.transform.position;
			mAnimation.Run ();
		}

			// Jump if collide with a meshLink
			if (isJumping == false && m_Agent.isOnOffMeshLink) {
				isJumping = true;
				m_Agent.autoTraverseOffMeshLink = false;
				StartCoroutine (Jump (1.0f));
			}
		//}
	}

	public void EscapePlayer() {
		Vector3 direction = -(player.transform.position - transform.position);
		direction.Normalize ();

		Vector3 destination = direction * 5.0f + transform.position;

		// Move
		m_Agent.SetDestination (destination);

		// Jump if collide with a meshLink
		if (isJumping ==false && m_Agent.isOnOffMeshLink) {
			isJumping = true;
			m_Agent.autoTraverseOffMeshLink = false;
			StartCoroutine (Jump (0.5f));
		}
	}

	public IEnumerator Jump (float duration) {
		OffMeshLinkData data = m_Agent.currentOffMeshLinkData;
		Vector3 startPos = transform.position;
		Vector3 endPos = data.endPos;// + Vector3.up * m_Agent.baseOffset;
		float normalizedTime = 0.0f;
		m_Agent.isStopped = true;

		float height = Mathf.Abs (endPos.y - startPos.y) / 2.0f;
		if (height < 2.0f)
			height = 2.0f;

		// rotate
		m_Agent.updateRotation = false;
		if (transform.position.x - player.transform.position.x >= 0) {
			// Left
			transform.LookAt(transform.position + Vector3.left);
		} else {
			// Right
			transform.LookAt(transform.position + Vector3.right);
		}

		mAnimation.Jumping ();
		yield return new WaitForSeconds (0.35f);

		while (normalizedTime < 1.0f)
		{
			float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
			transform.position = Vector3.Lerp (startPos, endPos, normalizedTime) + yOffset * Vector3.up;
			normalizedTime += Time.deltaTime / duration;
			yield return null;
		}

		mAnimation.Landing ();
		yield return new WaitForSeconds (0.36f);

		m_Agent.Warp (endPos);
		m_Agent.isStopped = false;
		isJumping = false;
		m_Agent.updateRotation = true;
	}

	public IEnumerator Attack (GameObject player, int dmg) {
		isAttacking = true;

		mAnimation.Attack ();
		yield return new WaitForSeconds (0.35f);
		player.GetComponent<Health> ().Damage (dmg);
		yield return new WaitForSeconds (0.30f);
		//Debug.Log ("Fin attack");

		isAttacking = false;
	}
}
