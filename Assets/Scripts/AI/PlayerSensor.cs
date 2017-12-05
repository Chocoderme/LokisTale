using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSensor : MonoBehaviour {

    [SerializeField] float MinDistanceToPlayer = 1f;

    private GameObject Player;

	// Use this for initialization
	void Start () {
        Player = GameObject.FindWithTag("Player");

        if (!Player)
        {
            Debug.LogError("PlayerSensor cannot find Player instance");
        }
	}
	
    public bool CanMoveTowardPlayer()
    {
       return Mathf.Abs(Vector2.Distance(transform.position, Player.transform.position)) <= MinDistanceToPlayer;
    }

    public bool CanRunAwayFromPlayer()
    {
        // Can always do that ?
        return true;
    }

    public bool CanHideFromPlayer()
    {
        // Check position of hiding point
        return false;
    }

    public Vector2 GetPositionPlayer()
    {
        return Player.transform.position;
    }

    public Vector2 GetDirectionToPlayer()
    {
        return (transform.position - Player.transform.position).normalized;
    }


}
