# .NET Microservices Sample Application

Sample .NET Core reference application, powered by Microsoft, based on clean architecture and Docker containers.

## Overview ###
 
The application manages 100 robots. It gives their current position on an xy-plane along with their battery life. 

The /v1/robots/closest endpoint (HTTPPost) accepts a payload with a load which needs to be moved including its identifier and current x,y coordinates and return the robot which is the best to transport the load based on which one is closest the load's location. 

If there is more than 1 robot within 10 distance units of the load, the one with the most battery remaining is returned.

The distance between two points is found with the following formula:

![distance formula](https://user-images.githubusercontent.com/7139741/122107356-f915e300-cde8-11eb-8699-f87b50046350.png)

## Getting Started

Make sure you have [installed](https://docs.docker.com/docker-for-windows/install/) and [configured](https://github.com/dotnet-architecture/eShopOnContainers/wiki/Windows-setup#configure-docker) docker in your environment. After that, you can run the below commands from the **/src/** directory and get started with `robots` immediately.

```powershell
docker-compose build
docker-compose up
```

Access the application by navigating to [http://localhost:5001/swagger](http://localhost:5001/swagger)

#### Endpoints
There are 3 endpoints available
* GetRobots - displays the robots
* GetNearestRobot - Returns the robot which is the best to transport the load based on which one is closest the load's location 
* GetJobs - Returns a list of previously submitted jobs and their results

## Coming Soon! ##

* SPA to visualize the robot's locations and payloads
* Faster Computations! [Nearest neighbor search](https://en.wikipedia.org/wiki/Nearest_neighbor_search#Approximate_nearest_neighbor) implementation ([Supercluster.KD-Tree](https://github.com/ericreg/Supercluster.KDTree/wiki/Tutorial:-Nearest-Neighbor-Search)) and benmark results ([BenchmarkDontNet](https://benchmarkdotnet.org/articles/overview.html))
* **Better Robots!** Variances in the robots battery capacity, battery usage/per load weight, battery degradation
* Robots will move to payload location when performing a job (for subsequent requests)
* Battery levels persist through subsequent payload requests and the ability to send robots to recharge when their battery level is low

## Related documentation and guidance

You can find a related reference **Guide/eBook** focusing on **architecting and developing containerized and microservice-based .NET Applications** (download link available below) which explains in detail how to develop this kind of architectural style (microservices, Docker containers, Domain-Driven Design for certain microservices).

[![](img/architecture-book-cover-large-we.png)](https://aka.ms/microservicesebook)