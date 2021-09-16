using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMovement : MonoBehaviour {

    Rigidbody ourDrone;

    void Awake()
    {
        ourDrone = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        MovementUpDown();
        MovementFoward();
        Rotation();
        ClampingSpeedValues();
        Leaning();

        ourDrone.AddRelativeForce(Vector3.up * upForce);
        ourDrone.rotation = Quaternion.Euler(
            new Vector3(tiltAmountFoward, currentYRotation, tiltAmountSideway)
            );
    }

    public float upForce;
    void MovementUpDown()
    {
        if((Mathf.Abs(Input.GetAxis("Vertical")) > 0.2f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f))
        {
            if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftControl))
            {
                ourDrone.velocity = ourDrone.velocity;
            }
            if(!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.Q) && !Input.GetKey(KeyCode.E))
            {
                ourDrone.velocity = new Vector3(ourDrone.velocity.x, Mathf.Lerp(ourDrone.velocity.y, 0, Time.deltaTime * 5), ourDrone.velocity.z);
                upForce = 140;
            }
            if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl) && (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E)))
            {
                ourDrone.velocity = new Vector3(ourDrone.velocity.x, Mathf.Lerp(ourDrone.velocity.y, 0, Time.deltaTime * 5), ourDrone.velocity.z);
                upForce = 110;
            }
            if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
            {
                upForce = 200;
            }
        }

        if(Mathf.Abs(Input.GetAxis("Vertical")) < 0.2f && Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f)
        {
            upForce = 135;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            upForce = 450;
            if(Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f)
            {
                upForce = 500;
            }
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            upForce = -200;
        }
        else if(!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl) && (Mathf.Abs(Input.GetAxis("Vertical")) < 0.2f && Mathf.Abs(Input.GetAxis("Horizontal")) < 0.2f))
        {
            upForce = 98.1f;
        }
    }

    private float movementFowardSpeed = 500.0f;
    private float tiltAmountFoward = 0;
    private float tiltVelocityFoward;

    void MovementFoward()
    {
        if(Input.GetAxis("Vertical") != 0)
        {
            ourDrone.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical") * movementFowardSpeed);
            tiltAmountFoward = Mathf.SmoothDamp(tiltAmountFoward, 4 * Input.GetAxis("Vertical"), ref tiltVelocityFoward, 0.1f);
        }
    }

    // For Rotation
    private float wantedYRotation;
    private float currentYRotation;
    private float rotationYVelocity;
    private float rotateAmountByKeys = .5f;

    void Rotation()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            wantedYRotation -= rotateAmountByKeys;
        }
        if (Input.GetKey(KeyCode.E))
        {
            wantedYRotation += rotateAmountByKeys;
        }

        currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, 0.25f);
    }

    // For limiting movment speed of drone
    private Vector3 velocityToSmoothDampToZero;
    void ClampingSpeedValues()
    {
        if(Mathf.Abs(Input.GetAxis("Vertical")) > 0.2f && Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f)
        {
            ourDrone.velocity = Vector3.ClampMagnitude(ourDrone.velocity, Mathf.Lerp(ourDrone.velocity.magnitude, 10.0f, Time.deltaTime * 5f));
        }
        if(Mathf.Abs(Input.GetAxis("Vertical")) > 0.2f && Mathf.Abs(Input.GetAxis("Horizontal")) < 0.2f)
        {
            ourDrone.velocity = Vector3.ClampMagnitude(ourDrone.velocity, Mathf.Lerp(ourDrone.velocity.magnitude, 10.0f, Time.deltaTime * 5f));
        }
        if(Mathf.Abs(Input.GetAxis("Vertical")) < 0.2f && Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f)
        {
            ourDrone.velocity = Vector3.ClampMagnitude(ourDrone.velocity, Mathf.Lerp(ourDrone.velocity.magnitude, 5.0f, Time.deltaTime * 5f));
        }
        if (Mathf.Abs(Input.GetAxis("Vertical")) < 0.2f && Mathf.Abs(Input.GetAxis("Horizontal")) < 0.2f)
        {
            ourDrone.velocity = Vector3.SmoothDamp(ourDrone.velocity, Vector3.zero, ref velocityToSmoothDampToZero, 0.95f);
        }
    }

    private float sideMovementAmount = 300.0f;
    private float tiltAmountSideway;
    private float tiltAmountVelocity;
    void Leaning()
    {
        if(Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f)
        {
            ourDrone.AddRelativeForce(Vector3.right * Input.GetAxis("Horizontal") * sideMovementAmount);
            tiltAmountSideway = Mathf.SmoothDamp(tiltAmountSideway, -4 * Input.GetAxis("Horizontal"), ref tiltAmountVelocity, 0.1f);
        }
        else
        {
            tiltAmountSideway = Mathf.SmoothDamp(tiltAmountSideway, 0, ref tiltAmountVelocity, 0.1f);
        }
    }
}