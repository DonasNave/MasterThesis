# Analysis of microservice architecture containing dotnet AoT compiled services

## Introduction

With the introduction of AoT compilation in dotnet 7, questions about it's use in microservice architecture have arisen. This thesis will analyze the use of AoT compiled dotnet services in microservice architecture. This will by done by two mirror architectures, one using AoT compiled while other using JIT compiled services. The analysis will be done by comparing the two architectures in terms of:

- language support
- performance
- memory usage
- deployment
- development
- maintenance

## How JIT and AoT compilation works

JIT compilation is a process of compiling source code into machine code during runtime. This is done by the JIT compiler, which is part of the runtime environment. The JIT compiler compiles the source code into machine code, which is then executed by the CPU. This is however different for dotnet, where the source code is first compiled into IL code, which is then compiled into machine code by the JIT compiler. This is done to allow for cross platform support, reflection and other features.

AoT compilation is a process of compiling source code into machine code before runtime. This is done by the AoT compiler, which is part of the dotnet SDK. The AoT compiler compiles the source code into machine code, which is then executed by the CPU. Not having to rely on JIT compiler during runtime allows for faster startup times and smaller memory footprint. This is however done at the cost of cross platform support, reflection and other major features.

## What steps are needed

1. Study of documentation
   - dotnet (AoT and JIT compilation)
   - microk8s (kubernetes)
   - monitoring tools
2. Design of the architecture
   - Architecture of the application, it should consist of
     - Services
       - dotnet AoT and JIT compiled
       - minimal apis (dotnet 8.0)
       - sample services
         - file upload service (sporadic use)
         - weather service (constant use)
         - stock market scraper (periodic use)
       - Request runner (management service)
         - runs requests on the services in specified time intervals
     - Client
       - Blazor SPA (single page application)
         - Integrate dashboards for monitoring?
     - Database
       - PostgreSQL
     - Monitoring
       - Grafana
       - Prometheus
       - dotnet monitor
       - edgeshark
     - Auth provider?
       - Keycloak
     - Message broker?
       - Kafka
     - Reverse proxy (Load balancer)?
       - nginx
   - Deployment
     - MicroK8s (Kubernetes)
   - There will be 2 deployments
     - JIT stack
     - AoT stack
3. Implementation of the architecture
   - configuration of deploy environment and 3rd party tools
   - implementation of services
   - implementation of client
   - configuration of deployment
4. Testing of the architecture
   - performance testing
     - upper limit of the architecture
   - reliability testing
     - lower limit of the architecture
     - stress testing
5. Analysis of the architecture
   - current language support
   - deployment performance
   - economic performance
6. Conclusion
   - comparison of the two architectures
   - recommendation for future use
   - thesis completion

## Expected result

- two mirror architectures
  - stored as OS code with proper docs and ci/cd (github)
- analysis of the two architectures via:
  - grafana dashboards + prometheus metrics
  - documentation/thesis text and graphs

### Documentation

Master's thesis will be written in latex. It will contain:

- introduction
- theory
  - dotnet compilation techniques
    - current language support
    - plus and cons of each technique
  - architectures
    - monolithic architecture
    - microservice architecture
  - deployment techniques and tools
    - monitoring tools (grafana, prometheus, loki, dotnet monitor, edgeshark)
    - networking tools (nginx, envoy)
    - deployment tools (microk8s, kubernetes, docker swarm)
    - message brokers (rabbitmq, kafka)
- practical part
  - design of the architecture
  - implementation of the architecture
  - testing of the architecture
  - analysis of the architecture
  - conclusion
  - testing
- results
  - description of the results
  - comparison of the two architectures
- conclusion

## Proposed tools

- versioning
  - git
  - hosting on github
- code
  - dotnet
    - framework 8.0
    - nuget
    - testing
      - XUnit
      - Moq
- deployment
  - docker
    - docker images
  - kubernetes
    - microk8s
- message broker
  - kafka
- database
  - postgresql
- networking
  - nginx/envoy
- monitoring
  - grafana
  - prometheus
  - loki
  - dotnet monitor
  - edgeshark
- CI/CD
  - github actions
- feedback collection
  - github issues
- documentation
  - github wiki (markdown) for code
  - latex for thesis

## Related work

Reporting

- supervisor (professor)
- company

## Possible extension

- create a serverless architecture using AoT dotnet services

## Supporting material

### Literature

#### Books

#### Articles

### Software

- [Docker](https://www.docker.com/)
- [MicroK8s](https://microk8s.io/)
- [Kubernetes](https://kubernetes.io/)
- [dotnet](https://dotnet.microsoft.com/)
- [aot-dotnet](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/?tabs=net7%2Cwindows)
- [NuGet](https://www.nuget.org/)
- [XUnit](https://xunit.net/)
- [Kafta](https://kafka.apache.org/)
- [PostgreSQL](https://www.postgresql.org/)
- [nginx](https://www.nginx.com/)
- [grafana](https://grafana.com/)
- [prometheus](https://prometheus.io/)
- [dotnet monitor](https://github.com/dotnet/dotnet-monitor)
- [keycloak](https://www.keycloak.org/)
