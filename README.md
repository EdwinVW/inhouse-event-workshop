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

<img src="img/services.png" style="zoom: 33%;" />

- The **Simulation** is a .NET console application that will simulate the 3 entry- and exit-camera's with cars passing under them and the CO2 level sensor.
- The **Traffic Control Service** is an ASP.NET WebAPI application that offers 2 endpoints: `/entrycam` and `/exitcam`.
- The **Measurements Service** is an ASP.NET Core WebAPI application that offers 3 endpoints: `/averageSpeed`, `/co2Level` and `/metrics`.

The way the simulation works is depicted in the sequence diagram below:

<img src="./img/sequence.png" alt="Sequence diagram" style="zoom:25%;" />

1. The **Camera Simulation** generates a random license-number and sends a *VehicleRegistered* message (containing this license-number, a random entry-lane (1-3) and the timestamp) to the `/entrycam` endpoint of the **TrafficControl Service**.
1. The **TrafficControl Service** stores the VehicleState (license-number and entry-timestamp).
1. After some random interval, the **Camera Simulation** sends a *VehicleRegistered* message to the `/exitcam` endpoint of the **TrafficControl Service** (containing the license-number generated in step 1, a random exit-lane (1-3) and the exit timestamp).
1. The **TrafficControl Service** retrieves the VehicleState that was stored at vehicle entry.
1. The **TrafficControl Service** calculates the average speed of the vehicle using the entry- and exit-timestamp. It also stores the VehicleState with the exit timestamp for audit purposes, but this is left out of the sequence diagram for clarity.
1. The average speed is sent to the `/averageSpeed` endpoint of the **Measurements Service**. The request payload will be an *AverageSpeedmeasured* message containing the average speed in KMh of a vehicle and the timestamp of the measurement.
1. The **Measurements Service** calculates the overall average speed over time since the last sample point (see 9) and stores this.
1. Every 15 seconds, the **CO2 Sensor Simulation** reads the current level of CO2 in the air and sends this to the `/co2Level` endpoint of the **Measurements Service**. The request payload will be a *CO2LevelMeasured* message containing the the amount of CO2 in the air expressed in parts per million (ppm) and the timestamp of the measurement. 
1. The **Measurements Service** generates a *sample point*. A sample point is a measurement at a point in time that contains a timestamp, the average speed over time since the last sample point and the CO2 level at that point in time.
1. The **Dashboard\*** gets all the sample points by calling the metrics endpoint of the of the **Measurements Service**. The **Measurements Service** returns the last 50 sample points. This data can be used to create a dashboard that will plot the average speed and CO2 level in a graph.

**\*** The Dashboard is not implemented in the sample solution. But it could look something like this:

<img src="img/graphs.png" style="zoom:25%" />

## Running the services

You can start the services using the `dotnet run` command. Follow the instructions below to get the sample application up & running:

1. Open a terminal window. You could use the terminal window in Visual Studio Code.  

1. Go into the `src/MeasurementsService` folder.

1. Start the service by typing `dotnet run`. You should see output like this:
   ```PowerShell
   ❯ dotnet run
   info: Microsoft.Hosting.Lifetime[14]
         Now listening on: http://localhost:6003
   info: Microsoft.Hosting.Lifetime[0]
         Application started. Press Ctrl+C to shut down.
   info: Microsoft.Hosting.Lifetime[0]
         Hosting environment: Development
   info: Microsoft.Hosting.Lifetime[0]
         Content root path: D:\dev\inhouse-event-workshop\MeasurementsService
   ```

1. Open a new terminal window.

1. Go into the `src/TrafficCOntrolService` folder.

1. Start the service by typing `dotnet run`.

1. Open a new terminal window.

1. Go into the `src/Simulation` folder.

1. Start the simulation by typing `dotnet run`.

You can see in the logging of the Measurements service what the average speed and CO2 level in the air is:

![image-20230302131559093](img/measurementsservice-logging.png)

Now that you have the application running, you will start adding Dapr to it.

## Hands-on assignments

You've just heard about how Dapr can help with implementing microservices. In this hand-on part of the workshop, you will add Dapr building-blocks to the solution described above. The end result should look like this:

![](img/services-dapr.png)

The workshop will not tell you exactly what to do. It's up to you to add the Dapr building-blocks. You can use the [Dapr Documentation](https://docs.dapr.io/) to get more information on how to implement the different Building Blocks. 

Make sure you've installed the prerequisites listed below, and you can dive right in!

## Prerequisites

In order to get most value out of the workshop, make sure you have the prerequisites installed on your machine before the workshop starts. Install the General prerequisites first. Then, select the technology stack you are going to use for executing the workshop assignments and install the prerequisites for that technology stack.

#### General

- Git ([download](https://git-scm.com/))
- Visual Studio Code ([download](https://code.visualstudio.com/download)) with at least the following extensions installed:
  - [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client)
- Docker for desktop ([download](https://www.docker.com/products/docker-desktop))
- [Install the Dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/) and [initialize Dapr locally](https://docs.dapr.io/getting-started/install-dapr-selfhost/)

All scripts in the instructions are PowerShell scripts. If you're working on a Mac, it is recommended to install PowerShell for Mac:

- PowerShell for Mac ([instructions](https://docs.microsoft.com/nl-nl/powershell/scripting/install/installing-powershell-core-on-macos?view=powershell-7.1))

#### .NET

- .NET 7 SDK ([download](https://dotnet.microsoft.com/download/dotnet/7.0))
- [C# extension for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)

---

### Assignment 1

**Goal:** make sure all communication between the Simulation and the TrafficControl service is done using Dapr service invocation. 

Replace the default HTTP Proxy implementation and use the Dapr API (or de the .NET SDK for Dapr) to call the Traffic Control service using service invocation. 

---

### Assignment 2

**Goal:** make sure the TrafficControl service stores its state in Redis through the Dapr state-management building block.

Currently, the state is stored in memory. Replace the current Repository implementation with one that uses Dapr state-management.

---

### Assignment 3

Goal: make sure all communication with the Measurements service is done using asynchronous messaging. You will use the Redis server that is installed by default with Dapr as the message broker.

---

### Stretch Goal 1

> This assignment is for people that have finished all the other assignments within the timespan of te workshop. 

**Goal:** change the message-broker used in assignment 3 from Redis to RabbitMQ. 

In the `Infrastructure` folder in the repository you can find scripts to start and stop RabbitMQ as a Docker container on your local machine. Once started, you can access the RabbitMQ management console by browsing to [https://localhost:15672](https://localhost:15672). The default login credentials are: 

- Username:`guest`
- Password: `guest`

### Stretch Goal 2

> This assignment is for people that have finished all the other assignments within the timespan of te workshop. 

**Goal:** create a cool web-based Measurements DashBoard that shows the metrics. 
