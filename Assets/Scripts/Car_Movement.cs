using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Movement : MonoBehaviour
{
    public WheelCollider FRWheel;
    public WheelCollider FLWheel;
    public WheelCollider BRWheel;
    public WheelCollider BLWheel;

    public Transform FLwheelTransform;
    public Transform FRwheelTransform;
    public Transform BLwheelTransform;
    public Transform BRwheelTransform;

    public float Motorforce = 4f;
    public float SteeringAngle = 30f;
    

    float HorizontalInput;
    float VerticalInput;



    void Start()
    {
        
    }

    void Update()
    {
        MotorForce();
        UpdateWheels();
        GetInput();
        Steering();
    }

    void GetInput()
    {
        HorizontalInput=Input.GetAxis("Horizontal");
        VerticalInput=Input.GetAxis("Vertical");

    }

    void Steering()
    {
        FLWheel.steerAngle = SteeringAngle * HorizontalInput;
        FRWheel.steerAngle = SteeringAngle * HorizontalInput;
    }

    void MotorForce()
    {
        FLWheel.motorTorque = Motorforce * VerticalInput;
        FRWheel.motorTorque = Motorforce * VerticalInput;
        BLWheel.motorTorque = Motorforce * VerticalInput;
        BRWheel.motorTorque = Motorforce * VerticalInput;

    }

    void UpdateWheels()
    {
        RotateWheel(FLWheel,FLwheelTransform);
        RotateWheel(FRWheel,FRwheelTransform);
        RotateWheel(BLWheel,BLwheelTransform);
        RotateWheel(BRWheel,BRwheelTransform);

    }

    void RotateWheel(WheelCollider wheelcollider,Transform transform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelcollider.GetWorldPose(out pos,out rot);
        transform.position = pos;
        Debug.Log(rot);
        transform.rotation = rot;
    }
}
