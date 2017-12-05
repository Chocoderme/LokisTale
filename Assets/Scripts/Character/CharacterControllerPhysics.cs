using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterControllerPhysics : MonoBehaviour {

    [SerializeField] float MaxVelocity = 20.0f;
    [SerializeField] float Acceleration = 20.0f;
    [SerializeField] float JumpForce = 200.0f;
    [SerializeField] float GroundedDistanceToFloor = 0.55f;
    [SerializeField] bool DoubleJump = true;
    [SerializeField] float TimeResetJump = 0.2f;
    [SerializeField] bool MustReleaseButtonJump = true;
    [SerializeField] float DeccelrationTime = 0.1f;
    [SerializeField] bool VerboseDebug = false;

    // Components
    private Rigidbody mRigidBody;
    private Transform MeshChild;

    // Tools variables
    private bool mGrounded = false;
    private bool Jumping = false;
    private float TimeElapsedSinceLastJump = 0f;
    private int CurrentJump = 0;
    private RaycastHit Hit = new RaycastHit();
    private Quaternion RotationOrigin;

    void Awake () {
        mRigidBody = GetComponent<Rigidbody>();
        MeshChild = transform.Find("Mesh");
        RotationOrigin = transform.rotation;
	}

    public bool IsGrounded()
    {
        return mGrounded;
    }

    // Calculate distance from Player position (NOT PLAYER BOUND) and the floor
    private void CalculateDistanceGround()
    {
        bool PreviousGrounded = mGrounded;

        if (Physics.Raycast(transform.position, -Vector3.up, out Hit))
        {
            mGrounded = GroundedDistanceToFloor > Hit.distance;
            if (VerboseDebug)
                Debug.Log("Distance between Player and ground " + Hit.distance + " limit " + GroundedDistanceToFloor);
            if (mGrounded != PreviousGrounded && mGrounded && VerboseDebug)
            {
                Debug.Log("Player Grounded");
            }
        }
        else
        {
            if (PreviousGrounded != mGrounded && VerboseDebug)
                Debug.Log("The player is falling");
            mGrounded = false;
        }
    }

    private void PerformJump()
    {
        bool Jump = false;

        if (MustReleaseButtonJump)
        {
            if (Input.GetButtonDown("Jump") && !Jumping)
            {
                Jump = true;
                Jumping = true;
            }
            else if (Input.GetButtonUp("Jump") && Jumping)
            {
                Jumping = false;
            }
        }
        else
        {
            Jump = Input.GetButton("Jump");
        }


        // Detect if grounded;
        CalculateDistanceGround();

        if (mGrounded)
        {
            CurrentJump = 0;
        }

        TimeElapsedSinceLastJump += Time.fixedDeltaTime;

        // if the player pressed jump button
        if (Jump && TimeElapsedSinceLastJump >= TimeResetJump)
        {
            if ((mGrounded && !DoubleJump) || ((DoubleJump && CurrentJump < 2) || CurrentJump == 0))
            {
                mRigidBody.AddForce(transform.up * JumpForce);
                if (VerboseDebug)
                    Debug.Log("Add force " + transform.up * JumpForce);
                CurrentJump++;
                TimeElapsedSinceLastJump = 0f;
            }
        }
    }

    private void PerformMovement()
    {
        float Horizontal = Input.GetAxis("Horizontal");

        // There is input movement
        if (Mathf.Abs(Horizontal) > float.Epsilon)
        {
            float forceToApply = 0f;
            forceToApply = Acceleration * Horizontal;

            // Apply less force if same direction of velocity
            if (Horizontal > 0 && mRigidBody.velocity.x > 0)
                forceToApply = Mathf.Min(MaxVelocity - mRigidBody.velocity.x, forceToApply);
            else if (Horizontal < 0 && mRigidBody.velocity.x < 0)
                forceToApply = Mathf.Max(-MaxVelocity - mRigidBody.velocity.x, forceToApply);

            // Apply more force if opposite direction of velocity
            if (Horizontal < 0 && mRigidBody.velocity.x > 0 || Horizontal > 0 && mRigidBody.velocity.x < 0)
            {
                forceToApply += Mathf.Abs(mRigidBody.velocity.x) * (Horizontal < 0 ? -1 : 1) * Acceleration;
            }
            //mRigidBody.velocity = new Vector2(forceToApply, mRigidBody.velocity.y);

            mRigidBody.AddForce(new Vector2(forceToApply, 0));

            if (Horizontal < 0)
                transform.rotation = Quaternion.Euler(0, RotationOrigin.eulerAngles.y + 45, 0);
            else if (Horizontal > 0)
                transform.rotation = Quaternion.Euler(0, RotationOrigin.eulerAngles.y - 45, 0);
        }

        // Prevent player to go too fast
        if (mRigidBody.velocity.magnitude > MaxVelocity)
        {
            mRigidBody.velocity = mRigidBody.velocity.normalized * MaxVelocity;
        }

        // if no input and still moving : slowing down
        if (Mathf.Abs(Horizontal) <= float.Epsilon && mRigidBody.velocity.magnitude > float.Epsilon)
        {
            StartCoroutine(SlowDownPlayer(mRigidBody.velocity.x, 0, DeccelrationTime));
        }
    }

    private IEnumerator SlowDownPlayer(float SpeedStart, float SpeedEnd, float time)
    {
        float elapsedTime = Time.deltaTime;
        while (elapsedTime < time)
        {
            // if player start moving again, kill that Coroutine
            if (Mathf.Abs(Input.GetAxis("Horizontal")) > float.Epsilon)
                break;
            mRigidBody.velocity = new Vector2(Mathf.Lerp(SpeedStart, SpeedEnd, elapsedTime / time), mRigidBody.velocity.y);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void FixedUpdate()
    {
        PerformJump();
        PerformMovement();
    }
}
