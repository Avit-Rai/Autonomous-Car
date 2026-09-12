# Robotic Navigation in Unity

## Project Overview

This project implements an autonomous robotic navigation system in **Unity** using **C#**.

The objective is to program a simulated robot to autonomously navigate a predefined track containing a range of obstacles. The robot controller is designed to keep the robot on the road, avoid obstacles, maintain an appropriate speed, handle slopes, and stop when the track ends.

The project is results-based, with performance evaluated according to:

- Distance travelled around the course
- Time taken to complete the course
- Smoothness of the trajectory
- Ability to avoid obstacles
- Ability to remain on the track

## Assignment Constraint

The provided Unity project was used as the foundation for this implementation.

The assignment specifies that project parameters should not be changed and that the only project component edited for the navigation solution is:

`RobotController.cs`

The navigation behaviour is therefore implemented through the robot controller rather than modifying the provided track or environment.

## Technologies

- **Unity**
- **C#**
- **Unity Wheel Colliders**
- **Raycasting**
- **Rigidbody Physics**

## Navigation Approach

The robot uses three main raycast sensors:

- **Front sensor** – detects obstacles and checks the road ahead.
- **Left sensor** – detects obstacles and checks the left side of the track.
- **Right sensor** – detects obstacles and checks the right side of the track.

The sensors provide information about the surrounding environment, allowing the controller to make steering decisions automatically.

## Road Detection

The controller uses multiple raycasts at different angles and distances to determine whether the robot is positioned correctly on the track.

Short-range sensors detect the nearby road, while longer-angle raycasts provide information about the upcoming direction of the track.

If the road is detected primarily on one side, the robot adjusts its steering to move back towards the centre of the track.

## Obstacle Avoidance

The robot continuously checks for obstacles using its front, left, and right sensors.

When an obstacle is detected:

1. The controller determines its approximate position.
2. The robot steers away from the obstacle.
3. Braking can be applied when an obstacle is directly ahead.
4. The robot continues navigating once the obstacle is avoided.

This allows the robot to navigate around obstacles without manual control.

## Speed Control

The controller monitors the robot's velocity using its Rigidbody.

The motor force is adjusted according to the current speed:

- If the robot becomes too fast, motor force is reduced.
- If the robot becomes too slow, motor force is increased.

This helps maintain a controlled navigation speed.

## Slope Handling

The controller also monitors the robot's pitch using the slope sensor.

When the robot encounters a steep upward slope, additional motor force is applied to help maintain forward movement.

## Track-End Detection

The controller monitors whether the road is no longer detected.

If the road remains undetected for several consecutive physics frames, the controller interprets this as the end of the track.

The robot then:

- Stops applying motor torque.
- Applies braking force.
- Remains stationary.

## Steering

Only the front wheels are used for steering.

The steering angle is calculated from:

`steering angle = maximum steering angle × steering direction`

The steering direction is determined by the sensor information from the road and obstacles.

## Robot Controller Structure

The main components of `RobotController.cs` are:

| Component              | Purpose                        |
| ---------------------- | ------------------------------ |
| `Sense()`              | Detects roads and obstacles    |
| `HandleCarSteering()`  | Controls front-wheel steering  |
| `HandleCar()`          | Applies motor and brake torque |
| `StayingOnRoad()`      | Keeps the robot on the track   |
| `ForObstacles()`       | Avoids obstacles               |
| `CarSpeedController()` | Controls robot speed           |
| `ForSlope()`           | Handles uphill sections        |
| `TrackEnded()`         | Stops the robot at the end     |
| `WheelsUpdate()`       | Updates wheel visual positions |


## How to Run

1. Open the project in Unity.
2. Open the provided robotic navigation scene.
3. Ensure the `RobotController` component is attached to the robot.
4. Verify that the wheel colliders and sensors are assigned in the Unity Inspector.
5. Press **Play**.
6. Observe the robot navigating the track autonomously.

## Results

The controller is designed to maximise:

- Course completion distance
- Navigation speed
- Trajectory smoothness
- Obstacle avoidance
- Track stability

Performance can be evaluated by observing how far the robot travels, how quickly it completes the course, and how smoothly it navigates the track.

## Author

**Avit Rai Hodibu**

## Disclaimer

This project was developed for academic purposes as part of a robotic navigation assignment. The controller is designed specifically for the provided Unity simulation environment.
