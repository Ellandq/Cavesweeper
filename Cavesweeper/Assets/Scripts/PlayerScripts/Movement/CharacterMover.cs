using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    private CharacterController controller;
    private Transform groundCheck;

    [Header("Object Information")]
    private LayerMask groundMask;
    public MovementType Movement { get; set; }

    private Coroutine coroutine;

    [Header("Movement Settings")] 
    private Quaternion currentMovementAngle;

    [Header("Crouching Settings")]
    private const float StartingHorizontalScale = 1f;
    private const float CrouchingHorizontalScale = 0.6f;
    private const float StateChangeSpeed = 12f;


    private void Awake ()
    {
        IsMovementEnabled = true;
        controller = GetComponent<CharacterController>();
        groundCheck = transform.GetChild(0).GetComponent<Transform>();
        groundMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Prop"));
    }

    protected void Move (Vector3 moveVector)
    {
        if (!IsMovementEnabled) return;
        if (IsGrounded) UpdateCurrentMovementAngle();
        moveVector = currentMovementAngle * moveVector;
            
        controller.Move(moveVector * Time.deltaTime);
    }

    protected void Crouch ()
    {
        if (coroutine != null){
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(ChangeCrouchingState());
    }
    
    protected void Walk ()
    {
        // TODO
    }

    protected void Run ()
    {
        // TODO
        if (IsSprinting) IsSprinting = false;
        if (IsWalking) IsWalking = false;
    }

    protected void Sprint ()
    {
        // TODO
    }

    private void UpdateCurrentMovementAngle()
    {
        currentMovementAngle = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }

    protected bool IsGrounded => controller.isGrounded || Physics.CheckSphere(groundCheck.position, .5f, groundMask);
    
    protected bool IsJumping { get; set; }

    protected bool IsCrouching { get; set; }

    protected bool IsSprinting { get; set; }

    protected bool IsWalking { get; set; }
    
    public bool IsMovementEnabled { get; set; }

    #region Coroutines

    private IEnumerator ChangeCrouchingState () 
    {
        Vector3 currentScale = transform.localScale;
        if (IsCrouching)
        {
            while (CrouchingHorizontalScale != currentScale.y)
            {

                if (CrouchingHorizontalScale >= currentScale.y - .05f)
                {
                    currentScale.y = Mathf.MoveTowards(currentScale.y, CrouchingHorizontalScale, Time.deltaTime * StateChangeSpeed);
                    transform.localScale = currentScale;
                }
                else
                {
                    currentScale.y = Mathf.Lerp(currentScale.y, CrouchingHorizontalScale, Time.deltaTime * StateChangeSpeed);
                    transform.localScale = currentScale;
                }

                yield return null;
            }
        }
        else 
        {
            while (StartingHorizontalScale != currentScale.y)
            {
                if (StartingHorizontalScale <= currentScale.y - .05f)
                {
                    currentScale.y = Mathf.MoveTowards(currentScale.y, StartingHorizontalScale, Time.deltaTime * StateChangeSpeed);
                    transform.localScale = currentScale;
                }
                else
                {
                    currentScale.y = Mathf.Lerp(currentScale.y, StartingHorizontalScale, Time.deltaTime * StateChangeSpeed);
                    transform.localScale = currentScale;
                }

                yield return null;
            }
        }
        coroutine = null;
    }

    #endregion
}

public enum MoveDirection{
    Forwards, Backwards, Left, Right
}

public enum MovementType {
    Walk, Run, Sprint
}
