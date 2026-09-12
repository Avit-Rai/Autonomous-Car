using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    [SerializeField] private WheelCollider WheelColliderFrontLeft;
    [SerializeField] private WheelCollider WheelColliderFrontRight;
    [SerializeField] private WheelCollider WheelColliderBackLeft;
    [SerializeField] private WheelCollider WheelColliderBackRight;

    [SerializeField] private Transform TransformWheelFrontLeft;
    [SerializeField] private Transform TransformWheelFrontRight;
    [SerializeField] private Transform TransformWheelBackLeft;
    [SerializeField] private Transform TransformWheelBackRight;

    [SerializeField] private Transform RaycastSensorFront;
    [SerializeField] private Transform RaycastSensorLeft;
    [SerializeField] private Transform RaycastSensorRight;
    [SerializeField] private Transform EulerAnglesSensor;

    // Car Attributes
    [SerializeField] private float maxSteerAngle = 45f;
    [SerializeField] private float motorForce = 400f;
    [SerializeField] private float brakeForce = 0f;

    // Sensor Distances
    private float obsInLength = 10f;  // Obstacle distance
    private float nearRoadRayLength = 12f;   // Road Short distance
    private float farRoadRayLength = 12f;  // Road Long distance
    private float frontRoadRayLengtht = 12f; // Front Road distance

    private Rigidbody rb;
    private bool TheEnd = false;
    private int endOfcenterRoad = 0;
    private float velocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if(RaycastSensorFront) RaycastSensorFront.localRotation = Quaternion.identity;
        if(RaycastSensorLeft) RaycastSensorLeft.localRotation = Quaternion.identity;
        if(RaycastSensorRight) RaycastSensorRight.localRotation = Quaternion.identity;
    }
    private void FixedUpdate()
    {
        velocity = rb.linearVelocity.magnitude;
        brakeForce = 0f;

        StayingOnRoad();
        ForObstacles(); 
        CarSpeedController();
        ForSlope();
        TrackEnded();
        HandleCar();
        WheelsUpdate();
    }

    // Sense function
    private bool Sense(Transform sensor, float angleX, float angleY, float dist, string LookFor)
    {
        Quaternion rotation = Quaternion.Euler(angleX, angleY, 0);
        Vector3 Directions = sensor.TransformDirection(rotation * Vector3.forward);

        RaycastHit hit;
        if (Physics.Raycast(sensor.position, Directions, out hit, dist))
        {
            string objectName = hit.collider.name;
            //Check only track
            if (LookFor == "Road" && objectName.StartsWith("MT_"))
            {
                Debug.DrawRay(sensor.position, Directions * hit.distance, Color.green);
                return true;
            }
            // Check only obstacles
            if (LookFor == "Obs" && objectName.StartsWith("Cube"))
            {
                Debug.DrawRay(sensor.position, Directions * hit.distance, Color.yellow);
                return true;
            }
        }

        Debug.DrawRay(sensor.position, Directions * dist, Color.red);
        return false;
    }


    // sterring handler
    private void HandleCarSteering(float direction)
    {
        float steer = maxSteerAngle * direction;
        // steer only front wheels
        WheelColliderFrontLeft.steerAngle = steer;
        WheelColliderFrontRight.steerAngle = steer;
    }
    // motor and brake handler
    private void HandleCar()
    {
        WheelColliderFrontLeft.motorTorque = motorForce;
        WheelColliderFrontRight.motorTorque = motorForce;
        WheelColliderBackLeft.motorTorque = motorForce;
        WheelColliderBackRight.motorTorque = motorForce;
        
        WheelColliderFrontLeft.brakeTorque = brakeForce;
        WheelColliderFrontRight.brakeTorque = brakeForce;
        WheelColliderBackLeft.brakeTorque = brakeForce;
        WheelColliderBackRight.brakeTorque = brakeForce;
    }

    private void UpdateWheel(WheelCollider wc, Transform wt)
    {
        Vector3 pos; Quaternion rot;
        wc.GetWorldPose(out pos, out rot);
        wt.position = pos;
        wt.rotation = rot;
    }

    private void WheelsUpdate()
    {
        UpdateWheel(WheelColliderFrontLeft, TransformWheelFrontLeft);
        UpdateWheel(WheelColliderFrontRight, TransformWheelFrontRight);
        UpdateWheel(WheelColliderBackLeft, TransformWheelBackLeft);
        UpdateWheel(WheelColliderBackRight, TransformWheelBackRight);
    }

    // Be on track
    private void StayingOnRoad()
    {
        // check the sensor for road
        bool shortLeftSense = Sense(RaycastSensorLeft,15f, -15f, nearRoadRayLength, "Road");
        bool shortRightSense = Sense(RaycastSensorRight,15f, 15f, nearRoadRayLength, "Road");
        bool longLeftSense  = Sense(RaycastSensorLeft,15f, -35f, farRoadRayLength, "Road");
        bool longRightSense  = Sense(RaycastSensorRight,15f, 35f, farRoadRayLength, "Road");
        bool frontSense = Sense(RaycastSensorFront,15f, 0f, frontRoadRayLengtht, "Road");

        if (!frontSense && !shortLeftSense && !shortRightSense)
        {
            // mark the end of the road
            endOfcenterRoad++;
            if (endOfcenterRoad > 8) TheEnd = true;
        }
        else
        {
            endOfcenterRoad = 0;
            TheEnd = false;

            // not senseing road 
            if (!longRightSense)
            {
                HandleCarSteering(-1.2f);
            }
            else if (!longLeftSense)
            {
                HandleCarSteering(1.2f);
            }
            else
            {
                HandleCarSteering(0);
            }
        }
    }
    // avoid obstacle
    private void ForObstacles()
    {
        // check the sensor for obstacles
        bool hitRightObstacles = Sense(RaycastSensorRight,0f, 15f, obsInLength, "Obs");
        bool hitLeftObstacles = Sense(RaycastSensorLeft,0f, -15f, obsInLength, "Obs");
        bool hitFrontObstacles = Sense(RaycastSensorFront,0f, 0f, obsInLength, "Obs");

        if (hitFrontObstacles)
        {
            HandleCarSteering(hitLeftObstacles ? 1 : -1); // Steering away
            brakeForce = 100f; // breaking
        }
        else if (hitRightObstacles)
        {
            HandleCarSteering(-0.5f); // Steering left
        }
        else if (hitLeftObstacles)
        {
            HandleCarSteering(0.5f);  // Steering right
        }
    }
    // speed controller
    private void CarSpeedController()
    {
        if (velocity > 6) motorForce = 0;
        else if (velocity < 4) motorForce = 450; 
    }
    // handle high hill
    private void ForSlope()
    {
        if (EulerAnglesSensor != null) {
            float pitch = EulerAnglesSensor.eulerAngles.x;
            if (pitch > 180f) pitch -= 360f;
            
            //pitch up
            if (pitch > 20f) motorForce = 400f;
        }
    }
    // end
    private void TrackEnded()
    {
        if (TheEnd)
        {
            brakeForce = 2000f;
            motorForce = 0f;
        }
    }
}