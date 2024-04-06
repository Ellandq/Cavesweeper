using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : CharacterMover
{
    [Header ("Movement Information")]
    private Dictionary<MoveDirection, bool> moveStatus;
    private Vector3 adjustedMovementVector;
    private Vector3 movementVector;
    private Vector3 jumpingHorizontalMovementVector;
    private float currentSpeed;
    private float speedMultiplier;
    private bool ignoreNextSprintInput;
    private bool ignoreNextWalkingInput;
    private bool ignoreNextJumpInput;

    [Header ("Movement settings")] 
    private readonly List<float> movementSpeed = new List<float>(){ 4f, 10f, 16f };
    private const float CrouchingSpeedMultiplier = .4f;
    private const float DefaultSpeedMultiplier = 1f;
    private const float JumpHeight = 1.8f;
    private const float JumpMovementReduction = 0.5f;
    private const float GravityMultiplier = 4f;


    private void Start ()
    {
        moveStatus = new Dictionary<MoveDirection, bool>(4)
        {
            { MoveDirection.Forwards, false },
            { MoveDirection.Backwards, false },
            { MoveDirection.Left, false },
            { MoveDirection.Right, false }
        };

        currentSpeed = movementSpeed[(int)MovementType.Run];
        speedMultiplier = DefaultSpeedMultiplier;

        PlayerInput input = InputManager.GetInputHandle();

        // Forwards Movement
        input.AddListenerOnButtonDown(ChangeForwardMovementState, "Move Forwards");
        input.AddListenerOnButtonUp(ChangeForwardMovementState, "Move Forwards");

        // Backwards Movement
        input.AddListenerOnButtonDown(ChangeBackwardsMovementState, "Move Backwards");
        input.AddListenerOnButtonUp(ChangeBackwardsMovementState, "Move Backwards");

        // Left Movement
        input.AddListenerOnButtonDown(ChangeLeftMovementState, "Move Left");
        input.AddListenerOnButtonUp(ChangeLeftMovementState, "Move Left");

        // Right Movement
        input.AddListenerOnButtonDown(ChangeRightMovementState, "Move Right");
        input.AddListenerOnButtonUp(ChangeRightMovementState, "Move Right");

        // Sprinting
        input.AddListenerOnButtonDown(ChangeSprintingState, "Sprint");
        input.AddListenerOnButtonUp(ChangeSprintingState, "Sprint");

        // Walking
        input.AddListenerOnButtonDown(ChangeWalkingState, "Walk");
        input.AddListenerOnButtonUp(ChangeWalkingState, "Walk");

        // Crouching
        input.AddListenerOnButtonDown(ChangeCrouchingState, "Crouch");
        input.AddListenerOnButtonUp(ChangeCrouchingState, "Crouch");

        // Jumping
        input.AddListenerOnButtonDown(Jump, "Jump");
        input.AddListenerOnButtonUp(Jump, "Jump");

    }
    
    private void Update()
    {
        Vector3 horizontalMovement = movementVector.normalized * (currentSpeed * speedMultiplier);
        
        if (!IsGrounded)
        {
            Vector3 jumpingMovementMask = new Vector3(Mathf.Sign(jumpingHorizontalMovementVector.x), 0f, Mathf.Sign(jumpingHorizontalMovementVector.z)) - jumpingHorizontalMovementVector.normalized;
            horizontalMovement *= JumpMovementReduction;
            horizontalMovement.Scale(jumpingMovementMask);
            horizontalMovement += jumpingHorizontalMovementVector;
        }
        
        adjustedMovementVector.x = horizontalMovement.x;
        adjustedMovementVector.z = horizontalMovement.z;
        

        ApplyGravity();
        
        Move(adjustedMovementVector);
    }

    #region Movement

        private void Jump ()
        {
            if (ignoreNextJumpInput){
                ignoreNextJumpInput = false;
                return;
            }
            ignoreNextJumpInput = true;
            if (IsGrounded)
            {
                jumpingHorizontalMovementVector = adjustedMovementVector;
                jumpingHorizontalMovementVector.y = 0f;
                adjustedMovementVector.y = Mathf.Sqrt(Physics.gravity.y * GravityMultiplier * -2f * JumpHeight);
                IsJumping = true;
            }
        }

        private void ApplyGravity ()
        {
            if (IsGrounded && adjustedMovementVector.y < 0f)
            {
                adjustedMovementVector.y = -2f;
                IsJumping = false;
            }
            else
            {
                adjustedMovementVector.y += Physics.gravity.y * GravityMultiplier * Time.deltaTime;
            }
        }

        private void ChangeForwardMovementState ()
        {
            Vector3 directionalMovement = new Vector3(0f, 0f, 1f);
            if (moveStatus[MoveDirection.Forwards]) 
            {
                movementVector -= directionalMovement;
            } 
            else 
            {
                movementVector += directionalMovement;
            }

            moveStatus[MoveDirection.Forwards] = !moveStatus[MoveDirection.Forwards];
        }

        private void ChangeBackwardsMovementState ()
        {
            Vector3 directionalMovement = new Vector3(0f, 0f, -1f);
            if (moveStatus[MoveDirection.Backwards]) 
            {
                movementVector -= directionalMovement;
            } 
            else 
            {
                movementVector += directionalMovement;
            }

            moveStatus[MoveDirection.Backwards] = !moveStatus[MoveDirection.Backwards];
        }

        private void ChangeLeftMovementState ()
        {
            Vector3 directionalMovement = new Vector3(-1f, 0f, 0f);
            if (moveStatus[MoveDirection.Left]) 
            {
                movementVector -= directionalMovement;
            } 
            else 
            {
                movementVector += directionalMovement;
            }

            moveStatus[MoveDirection.Left] = !moveStatus[MoveDirection.Left];
        }

        private void ChangeRightMovementState ()
        {
            Vector3 directionalMovement = new Vector3(1f, 0f, 0f);
            if (moveStatus[MoveDirection.Right]) 
            {
                movementVector -= directionalMovement;
            } 
            else 
            {
                movementVector += directionalMovement;
            }

            moveStatus[MoveDirection.Right] = !moveStatus[MoveDirection.Right];
        }

    #endregion

    #region Movement Type 

        private void ChangeCrouchingState ()
        {
            IsCrouching = !IsCrouching;

            speedMultiplier = IsCrouching ? CrouchingSpeedMultiplier : DefaultSpeedMultiplier;

            if (IsCrouching && IsSprinting)
            {
                ignoreNextSprintInput = true;
                ChangeRunningState();
            }
            
            Crouch();
        }

        private void ChangeWalkingState ()
        {
            if (ignoreNextWalkingInput)
            {
                ignoreNextWalkingInput = false;
                return;
            }

            if (IsSprinting)
            {
                ignoreNextWalkingInput = true;
                return;
            }

            IsWalking = !IsWalking;

            if (IsWalking)
            {
                currentSpeed = movementSpeed[(int)MovementType.Walk];
                Walk();
            }
            else 
            {
                ChangeRunningState();
            }
        }

        private void ChangeRunningState ()
        {
            currentSpeed = movementSpeed[(int)MovementType.Run];
            Run();
        }

        private void ChangeSprintingState ()
        {
            if (ignoreNextSprintInput)
            {
                ignoreNextSprintInput = false;
                return;
            }

            if (IsCrouching)
            {
                ignoreNextSprintInput = true;
                return;
            }

            IsSprinting = !IsSprinting;

            if (IsSprinting)
            {
                currentSpeed = movementSpeed[(int)MovementType.Sprint];
                Sprint();
            }
            else 
            {
                ChangeRunningState();
            }
        }

    #endregion
}
