using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Movement Settings")]
    // Speed
    [SerializeField] private float playerDefaultSpeed = 8f;
    [SerializeField] private float playerSprintingSpeed = 12f;

    [Header ("Player State")]
    private Vector3 velocity;
    private bool isSprinting;

    [Header ("Object References")]
    [SerializeField] private CharacterController controller;

    private void Update()
    {
        if (controller.isGrounded && velocity.y < 0f){
            velocity.y = -2f;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 movementVector = transform.right * x + transform.forward * z;

        if (movementVector != Vector3.zero){
            isSprinting = Input.GetKey(KeyCode.LeftShift);

            if (isSprinting){
                controller.Move(movementVector * playerSprintingSpeed * Time.deltaTime);
            }else{
                controller.Move(movementVector * playerDefaultSpeed * Time.deltaTime);
            }
        }else{
            isSprinting = false;
        }   

        velocity.y += Physics.gravity.y * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
