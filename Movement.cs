using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script requires you to have setup your animator with 3 parameters, "InputMagnitude", "InputX", "InputZ"
//With a blend tree to control the inputmagnitude and allow blending between animations.
[RequireComponent(typeof(CharacterController))]
public class Movement: MonoBehaviour
{

    public float InputX;
    public float InputY;
    public Animator anim;
    public float movementSpeed = 7;
    public float distanceToGround;
    public bool willGroundIdle = false;
    public bool willGroundRun = false;
    public bool isgrounded = false;
    public float VerticalSpeed;
    public Rigidbody rb;
    public float momentum;
    bool idle;
    bool run;



    // Use this for initialization
    void Start()
    {
        anim = this.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("isRunning", false);
        anim.ResetTrigger("isJumping");
        VerticalSpeed = -rb.velocity.y;
        distanceToGround = 1f;
        if (VerticalSpeed > 12f)
        {
            distanceToGround = 7f;
        }
        else
        {
            distanceToGround = VerticalSpeed*0.5f;
        }
       
        anim.SetBool("willGroundIdle", willGroundIdle);
        anim.SetBool("willGroundRun", willGroundRun);
        anim.SetBool("isGrounded", isgrounded);
        Vector3 pos = transform.position;
        pos.z = 0;
        transform.position = pos;

        if (isgrounded)
        { 

            InputX = Input.GetAxis("Horizontal");
            InputY = Input.GetAxis("Vertical");
            anim.SetFloat("InputX", InputX);
            anim.SetFloat("InputY", InputY);
            if (Input.GetKey(KeyCode.Space))
            {
                anim.SetTrigger("isJumping");
                momentum = rb.velocity.x;
            }
        }
        else
        {
            anim.SetFloat("InputX", 0f);
            anim.SetFloat("InputY", 0f);
            anim.ResetTrigger("isJumping");
            Debug.Log(momentum);
            rb.AddForce(Vector3.right * momentum*30);
        }

        if (!isgrounded && willGroundIdle) {
            anim.SetBool("willGround", true);
        }
    }

    private void FixedUpdate()
    {
        idle = Physics.Raycast(transform.position, -Vector3.up, distanceToGround);
        run = Physics.Raycast(transform.position, -Vector3.up, distanceToGround*0.5f);
        if (idle)
        {
            willGroundIdle = true;
        }
        else
        {
            willGroundIdle = false;
        }
        if (run)
        {
            willGroundRun = true;
        }
        else
        {
            willGroundRun = false;
        }
        
    }
    void OnCollisionEnter(Collision theCollision)
    {
        if (theCollision.gameObject.name == "floor")
        {
            isgrounded = true;
        }
    }

    //consider when character is jumping .. it will exit collision.
    void OnCollisionExit(Collision theCollision)
    {
        if (theCollision.gameObject.name == "floor")
        {
            isgrounded = false;
        }
    }
}




//make sure u replace "floor" with your gameobject name.on which player is standing
