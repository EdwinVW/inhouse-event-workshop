# Info Support in-house event workshop

This repo contains a workshop that teaches you how to apply Dapr to a microservices application. This repository contains the source-code that forms the starting point of the workshop. During the workshop you will have to add new services to the solution and integrate these into the solution using Dapr.

## Case Setup

This is an overview of the fictitious setup I'm simulating in this sample:

<img src="img/setup-overview.png" alt="Setup overview" style="zoom: 25%;" />

There is a stretch of highway that is equipped with camera's. There's 1 entry-camera and 1 exit-camera per lane. When a vehicle passes an entry-camera, the license-number of the vehicle and the timestamp is registered.

When the car passes an exit-camera, this timestamp is also registered and the average speed of the vehicle based on the entry- and exit-timestamp is calculated.

Next to the highway, there is a CO2 sensor. This sensor will measure the amount of CO2 in the air surrounding the highway once every minute.

Using the overall average speed and CO2 levels over time, we can get investigate whether the average speed on a highway is correlated to the CO2 level  in the air. 

## Simulation

In order to simulate this in code, I created the following services:

<img src="./img/services.png" alt="Services" style="zoom:25%;" />

- The **Camera Simulation** is a .NET Core console application that will simulate passing cars.
- The **CO2 Sensor Simulation** is a .NET Core console application that will simulate a CO2 level sensor.
- The **Traffic Control Service** is an ASP.NET Core WebAPI application that offers 2 endpoints: `/entrycam` and `/exitcam`.
- The **Measurements Service** is an ASP.NET Core WebAPI application that offers 3 endpoints: `/averageSpeed`, `/co2Level` and `/stats`.
- The **Measurements Dashboard** is an ASP.NET Core Web application that shows a dashboard with the measurements.

The Dashboard will show 2 synchronized graphs with the metrics:

<img src="img/graphs.png" style="zoom:25%" />

The way the simulation works is depicted in the sequence diagram below:

<img src="./img/sequence.png" alt="Sequence diagram" style="zoom:25%;" />

1. The **Camera Simulation** generates a random license-number and sends a *VehicleRegistered* message (containing this license-number, a random entry-lane (1-3) and the timestamp) to the `/entrycam` endpoint of the **TrafficControl Service**.
1. The **TrafficControl Service** stores the VehicleState (license-number and entry-timestamp).
1. After some random interval, the **Camera Simulation** sends a *VehicleRegistered* message to the `/exitcam` endpoint of the **TrafficControl Service** (containing the license-number generated in step 1, a random exit-lane (1-3) and the exit timestamp).
1. The **TrafficControl Service** retrieves the VehicleState that was stored at vehicle entry.
1. The **TrafficControl Service** calculates the average speed of the vehicle using the entry- and exit-timestamp. It also stores the VehicleState with the exit timestamp for audit purposes, but this is left out of the sequence diagram for clarity.
1. The average speed is sent to the `/averageSpeed` endpoint of the **Measurements Service**. The request payload will be an *AverageSpeedmeasured* message containing the average speed in KMh of a vehicle and the timestamp of the measurement.
1. The **Measurements Service** calculates the overall average speed over time since the last sample point (see 9) and stores this.
1. Every minute, the **CO2 Sensor Simulation** reads the current level of CO2 in the air and sends this to the `/co2Level` endpoint of the **Measurements Service**. The request payload will be a *CO2LevelMeasured* message containing the the amount of CO2 in the air expressed in parts per million (ppm) and the timestamp of the measurement. 
1. The **Measurements Service** generates a *sample point*. A sample point is a measurement at a point in time that contains a timestamp, the average speed over time since the last sample point and the CO2 level at that point in time.
1. The **Measurement Dashboard** gets all the sample points by calling the metrics endpoint of the of the **Measurements Service** and renders 2 graphs. The **Measurements Service** returns the last 50 sample points.  

## Hands-on Workshop



### Assignment 1

### Assignment 2

### Assignment 3

