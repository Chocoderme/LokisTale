using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DIRECTION
{
    LEFT,
    RIGHT
}

[RequireComponent(typeof(Rigidbody))]
public class CharacterControllerLegacy : MonoBehaviour {

    [SerializeField] float MaxVelocity = 20.0f;
    [SerializeField] float Acceleration = 20.0f;
    [SerializeField] float JumpForce = 200.0f;
    [SerializeField] float GroundedDistanceToFloor = 0.55f;
    [SerializeField] bool DoubleJump = true;
    [SerializeField] float TimeResetJump = 0.2f;
    [SerializeField] bool MustReleaseButtonJump = true;
    [SerializeField] float DeccelrationTime = 0.1f;
    [SerializeField] bool VerboseDebug = false;
    [SerializeField] float Gravity = 9.81f;
    [SerializeField] bool CameraFollowing = true;

    // Components
    private Rigidbody mRigidBody;
    private Transform MeshChild;
    private Collider mCollider;

    //Camera
    private Camera mCamera;

    // Public variables
    private Vector2 DirectionGravity = Vector2.down;
    public bool preventActions = false;

    // Tools variables
    private bool mGrounded = false;
    private bool Jumping = false;
    private float TimeElapsedSinceLastJump = 0f;
    private int CurrentJump = 0;
    private RaycastHit Hit = new RaycastHit();
    private Quaternion RotationOrigin;
    private Vector2 OffsetCamera;
    private EntityAnimation mAnimation;

    void Awake()
    {
        mRigidBody = GetComponent<Rigidbody>();
        mAnimation = GetComponent<EntityAnimation>();
        mCollider = GetComponentInChildren<Collider>();
        MeshChild = transform.Find("Model");
        RotationOrigin = transform.rotation;
        mCamera = Camera.main;
        OffsetCamera = mCamera.transform.position - transform.position;
    }

    public bool IsGrounded()
    {
        return mGrounded;
    }

    public bool isMoving()
    {
        return Input.GetAxis("Horizontal") != 0;
    }

     // Calculate distance from Player position (NOT PLAYER BOUND) and the floor
    private void CalculateDistanceGround()
    {
        bool PreviousGrounded = mGrounded;
        //Debug.Log(mCollider.bounds.size);
        Vector3 p1 = mCollider.bounds.size;
        p1.y = 0;
        p1.z = 0;

        mGrounded = false;
        if (Physics.Raycast(transform.position - p1 / 2, -Vector3.up, out Hit))
        {
            mGrounded = GroundedDistanceToFloor > Hit.distance;
           // if (VerboseDebug)
            //    Debug.Log("Grounded("+mGrounded+")Distance between Player and ground " + Hit.distance + " limit " + GroundedDistanceToFloor);
            if (PreviousGrounded == false && mGrounded && VerboseDebug)
            {
                //Debug.Log("Player Grounded");
            }
            if (GroundedDistanceToFloor > Hit.distance)
                Debug.DrawRay(transform.position - p1 / 2, -Vector3.up, Color.green, .01f);
            else
                Debug.DrawRay(transform.position - p1 / 2, -Vector3.up, Color.red, .01f);
        }
        if (Physics.Raycast(transform.position + p1 / 2, -Vector3.up, out Hit))
        {
            if (!mGrounded)
                mGrounded = GroundedDistanceToFloor > Hit.distance;
            // if (VerboseDebug)
            //    Debug.Log("Grounded("+mGrounded+")Distance between Player and ground " + Hit.distance + " limit " + GroundedDistanceToFloor);
            if (PreviousGrounded == false && mGrounded && VerboseDebug)
            {
                Debug.Log("Player Grounded");
            }
            if (GroundedDistanceToFloor > Hit.distance)
                Debug.DrawRay(transform.position + p1 / 2, -Vector3.up, Color.green, .01f);
            else
                Debug.DrawRay(transform.position + p1 / 2, -Vector3.up, Color.red, .01f);
        }
        if (Physics.Raycast(transform.position, -Vector3.up, out Hit))
        {
            if (!mGrounded)
                mGrounded = GroundedDistanceToFloor > Hit.distance;
            // if (VerboseDebug)
            //    Debug.Log("Grounded("+mGrounded+")Distance between Player and ground " + Hit.distance + " limit " + GroundedDistanceToFloor);
            if (PreviousGrounded == false && mGrounded && VerboseDebug)
            {
                Debug.Log("Player Grounded");
            }
            if (GroundedDistanceToFloor > Hit.distance)
                Debug.DrawRay(transform.position, -Vector3.up, Color.green, .01f);
            else
                Debug.DrawRay(transform.position, -Vector3.up, Color.red, .01f);
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
				mAnimation.Idle ();
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
        ApplyGravity();
    }

    private void PerformMovement()
    {
        float Horizontal = Input.GetAxis("Horizontal");

        // There is input movement
        if (Mathf.Abs(Horizontal) > float.Epsilon)
        {
            SetDirection((Horizontal < 0 ? DIRECTION.LEFT : DIRECTION.RIGHT));
            mRigidBody.MovePosition(transform.position + transform.forward * Mathf.Abs(Horizontal) * Acceleration * Time.fixedDeltaTime);
			if (!Jumping && IsGrounded())
            	mAnimation.Run();
			else
				mAnimation.Idle();

           /* float forceToApply = 0f;
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

            mRigidBody.AddForce(new Vector2(forceToApply, 0));*/

            // if input and still force on it : slowing down
             /*if (mRigidBody.velocity.magnitude > float.Epsilon)
             {
                 StartCoroutine(SlowDownPlayer(mRigidBody.velocity.x, 0, DeccelrationTime));
             }*/
        } else
        {
            mAnimation.Idle();
        }

        // Prevent player to go too fast
        if (mRigidBody.velocity.magnitude > MaxVelocity)
        {
            mRigidBody.velocity = mRigidBody.velocity.normalized * MaxVelocity;
        }
    }

    public void ApplyGravity()
    {
        if (!mGrounded)
        {
            mRigidBody.AddForce(transform.up * -1 * Gravity);
        }
    }

    public void SetDirection(DIRECTION direction)
    {
        mRigidBody.MoveRotation(Quaternion.Euler(0, RotationOrigin.eulerAngles.y + (direction == DIRECTION.LEFT ? -1 : 1) * 90, 0));
        MeshChild.rotation = Quaternion.Euler(0, RotationOrigin.eulerAngles.y + (direction == DIRECTION.LEFT ? 1 : -1) * 45, 0);
    }

    private IEnumerator SlowDownPlayer(float SpeedStart, float SpeedEnd, float time)
    {
        float elapsedTime = 0;
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
        if (!preventActions)
        {
            PerformJump();
            PerformMovement();
        }
        else
        {
            mAnimation.Idle();
            ApplyGravity();
        }
        PerformCameraFollowing();
    }

    void PerformCameraFollowing()
    {
        if (CameraFollowing)
        {
            mCamera.transform.position = new Vector3(transform.position.x, transform.position.y, mCamera.transform.position.z);
        }
    }

    public void ToggleCameraFollowing()
    {
        CameraFollowing = !CameraFollowing;
    }


}
