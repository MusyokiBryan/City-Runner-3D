using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    //Movement controls
    private const float LANE_DISTANCE = 3.0f;
    private const float TURN_SPEED = 0.08f;

    //Jumping Animations
    private Animator anim;

    private CharacterController controller;
    private float jumpForce = 6.0f;
    private float gravity = 12.0f;
    private float verticalVelocity;
    private float speed = 7.0f;
    private int desiredLane = 1; //Contorl the lane of character 0=left 1= middle 2=right 

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        //Inputs on which lane we should go
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            MoveLane(false);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            MoveLane(true);

        //calculate where we should be in future
        Vector3 targetPosition = transform.position.z * Vector3.forward;
        if (desiredLane == 0)
            targetPosition += Vector3.left * LANE_DISTANCE;
        else if (desiredLane == 2)
            targetPosition += Vector3.right * LANE_DISTANCE;

        // Calculating move delta
        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).normalized.x * speed;

        bool isGrounded = IsGrounded();

        // Calculate gravity
        if (isGrounded)
        {
            verticalVelocity = -0.1f;
            anim.SetBool("Grounded", isGrounded);




            if (Input.GetKeyDown(KeyCode.Space))
            {
                //jumping
                anim.SetTrigger("Jump");
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            verticalVelocity -= (gravity * Time.deltaTime);
            //falling faster if you are in air and slide
            if (Input.GetKeyDown(KeyCode.Space))
            {
                verticalVelocity = -jumpForce;
            }
        }

        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        //move player
        controller.Move(moveVector * Time.deltaTime);

        //rotate directions
        Vector3 direction = controller.velocity;
        if (direction != Vector3.zero)
        {
            direction.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, direction, TURN_SPEED);
        }

    }
    private void MoveLane(bool goingRight)
    {
        //moving left logic
        /*if (!goingRight)
        {
            desiredLane--;
            if (desiredLane == -1)
                desiredLane = 0;
        }
        else
        {
            desiredLane++;
            if (desiredLane == 3)
                desiredLane = 2;
        }*/
        desiredLane += (goingRight) ? 1 : -1;
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);
    }

    private bool IsGrounded()
    {
        Ray groundRay = new Ray(new Vector3(controller.bounds.center.x, (controller.bounds.center.y - controller.bounds.extents.y) + 0.2f, controller.bounds.center.z), Vector3.down);

        return Physics.Raycast(groundRay, 0.2f + 0.1f);

    }

}
