using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script requires you to have setup your animator with 3 parameters, "InputMagnitude", "InputX", "InputZ"
//With a blend tree to control the inputmagnitude and allow blending between animations.
[RequireComponent(typeof(CharacterController))]
public class Movement: MonoBehaviour
{
    public float maxJumpHeight;
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
    bool topPlayerCol;
    bool midPlayerCol;
    bool lowPlayerCol;
    Vector3 topRay;
    Vector3 midRay;
    Vector3 lowRay;
    float maxTimeidle=0.5f;
    float maxTimeRunning = 0.2f;
    float timer;
    bool jumpPressed;
    bool isRunning;



    // Use this for initialization
    void Start()
    {
        anim = this.GetComponent<Animator>();     
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {   
        anim.SetBool("isClimbable", false);
        anim.ResetTrigger("flip");
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
        
        if(jumpPressed)
            timer += Time.deltaTime;
        if (timer >= maxTimeidle && !isRunning)
        {
            rb.AddForce(new Vector3(0, 5, 0), ForceMode.Impulse);
            timer = 0;
            jumpPressed = false;
        }
        else if(timer>=maxTimeidle)
        {
            timer = 0;
            jumpPressed = false;
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
            if (InputX > 0)
                isRunning = true;
            else
                isRunning = false;
            anim.SetFloat("InputY", InputY);
            if (Input.GetKey(KeyCode.Space))
            {
                anim.SetTrigger("isJumping");
                momentum = rb.velocity.x;
                jumpPressed = true;
            }
            if (Input.GetKey(KeyCode.F))
            {
                anim.SetTrigger("flip");
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
            anim.SetBool("willGroundIdle", true);
        }
    }

    private void FixedUpdate()
    {
        //gets the rays positions
        topRay = transform.position + transform.up*2;
        midRay = transform.position + transform.up;
        lowRay = transform.position;
        
        //draws rays with no collisions
        Debug.DrawRay(topRay, transform.TransformDirection(Quaternion.AngleAxis(30, transform.forward) * transform.up), Color.green);
        Debug.DrawRay(midRay, transform.TransformDirection(Vector3.forward), Color.green);
        Debug.DrawRay(lowRay, transform.TransformDirection(Vector3.forward), Color.green);
        //verifies if there is collision in front or bottom
        topPlayerCol = Physics.Raycast(topRay, transform.TransformDirection(Quaternion.AngleAxis(30, transform.forward) * transform.up), 2);
        midPlayerCol = Physics.Raycast(midRay, transform.TransformDirection(Vector3.forward), 1);
        lowPlayerCol = Physics.Raycast(lowRay, transform.TransformDirection(Vector3.forward), 1);
        idle = Physics.Raycast(transform.position, -Vector3.up, distanceToGround);
        run = Physics.Raycast(transform.position, -Vector3.up, distanceToGround*0.5f);
        //draws rays with collisions
        if (topPlayerCol)
        {
            Debug.DrawRay(topRay, transform.TransformDirection(Vector3.forward), Color.red);
            topPlayerCol = true;
        }
        if (midPlayerCol)
        {
            Debug.DrawRay(midRay, transform.TransformDirection(Vector3.forward), Color.red);
            midPlayerCol = true;
        }        
        if (lowPlayerCol)
        {
            Debug.DrawRay(lowRay, transform.TransformDirection(Vector3.forward), Color.red);
            lowPlayerCol = true;
        }
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
