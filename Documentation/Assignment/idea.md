# Project deployment and configuration tool

## Introduction

Working in train development company presents unique software challenges.

Lifetime of a train is 30-40 years. During this time, the train is maintained and repaired. Software and hardware relating to the train (vehicle unit) is expected to stay mostly the same. However supporting server systems might change. Even when software stays the same, current deployment and configuration tools used might not have required life expectancy. Being capable of abstracting crucial logic behind deployment and configuration from the tools used is necessary for the company.

Another challenge is development standardization and norms. Developing software that accesses certain critical systems (like brakes, acceleration, signaling etc.) requires legal norms and standards to be followed. This is to ensure that the software is safe to use and does not cause any harm to the passengers or the train itself. Those standards are set on a EU level* and present a set of rules and methodologies that need to be followed. This might result in a software not being able to be used in a train, because it was not developed using those standards.

\* Currently there is no EU standardization for Server systems, but it is expected to be introduced in near future.

## How current systems work

Our application consist of microservices. They are developed with combination of dotnet framework, Angular and React. Each microservice is developed as a separate project. Each project is built into a docker image. Those images are deployed to a server using docker swarm. Docker swarm uses YML files to configure the deployment.

## What is needed

Supporting server software for train vehicles and it's challenges present a need for tool that:

- abstracts logic behind build, configuration and deployment of application from the tools currently used (docker swarm/kubernetes)
- has code available to the company (open source, or company owned) and is modifiable to suit the company's needs
- is developed in accordance with the standards set by the company (and by proxy EU legislation)
- is easily modifiable without need for extensive knowledge of the tool's codebase (framework that supports plugins with adequate api)
- usable in a CI/CD pipeline (must be able to be run from command line)

## Desired result

Custom OS dotnet CLI tool, that is composed of core framework and plugins. Framework provides via API configuration and deployment logic from abstracted config files. Specific plugin implementation provides implementation logic to be used by framework when deploying application.

This tool will be used in at least development environment of my company. In the remaining time before finishing the thesis, feedback and issues will be collected and addressed.

Simply put, if there is a decision to take application deployment and move it to kubernetes, the only thing that need to be done is to write a plugin that would transform the configuration and deployment rules to kubernetes YML files. (and so on for other deployment options).

Tool will use new human readable configuration files (likely json, abstracted from specific tool) related to:

- Service
  - Build description
  - Appsettings (existed before)
  - Deployment specifics
    - Networking
    - Resources
    - Service discovery
    - Dependencies (other services/versions)
- Application
  - Service list
  - Deployment specifics
    - Actions
    - Service coupling
    - Networking
    - Resources
    - Service discovery
- Server specifics
  - Networking
  - Resources
    - CPU, RAM, Memory
  - Service discovery
  - Dependencies (other services/versions)

*Sidenote: This is a very rough idea of what the configuration files will look like. It is expected that the configuration files will be changed and refined during the development of the tool.*

*Sidenote: Some presumptions are currently made about the tool's implementation as well as target environment. They are subject to change.*

### Functionality

- Building of service using abstract/primitive information into required format (docker image, podman, simple dlls/app) and it's configuration
- Creating configuration for application environment
- Creating configuration for server environment
- Running build and deploy actions *
- Running tests *

*Sidenote: Tool will be likely tightly integrated into CI/CD pipelines, thus it's functions will border with pipelines actions. Currently it is hard to predict where will the fine line in responsibility distribution will be drawn. This is subject to change.*

### Expected workflow

- User provides required abstracted configuration and plugin to the tool
- Tool validates the configuration
- Tool builds and configures the services
- Tool validates the services
- Tool implements/translates the configuration to the required format for application and server environment
- Tool prepares the server environment
- Tool deploys the services to the server environment

### Documentation

Master's thesis will be written in latex. It will contain:

- introduction
- theory
  - related work
  - methodology
- implementation
- testing
- results
- conclusion

Tool itself will be documented in a markdown files. It will contain:

- overview
- how to use
- how to develop plugins

## Proposed tools

- versioning
  - git
  - hosting on github
- code
  - dotnet
    - framework 8.0
    - dotnet tools CLI
    - nuget packaging for framework and plugin management
    - testing
      - NUnit?
      - Moq?
- abstracted configuration and deployment logic
  - JSON
  - NoSQL database
- implemented configuration and deployment logic
  - Docker images
  - Docker swarm (docker swarm plugin)
    - YML
    - build actions
    - deploy actions
  - Kubernetes (kubernetes plugin)
    - YML
    - build actions
    - deploy actions
- CI/CD
  - github actions
- feedback collection
  - github issues
- documentation
  - github wiki (markdown) for code
  - latex for thesis

## Expected steps

1. Definition of requirements (partially done)
2. Research existing tools and frameworks (partially done, reason for this thesis)
   1. Explain why they are not suitable for this project
3. Design of the tool
   1. Design of the core framework
      1. Design data structures
   2. Design of the plugin system
   3. Design of the configuration and deployment rules
   4. Documentation of the design
4. Implementation of the tool
    - Implementation of the core framework
      - Implementation of the configuration and deployment rules
    - Implementation of the plugin system
    - Documentation of the implementation
5. Testing of the tool
   1. Unit testing
   2. Integration testing
   3. Documentation of the testing
6. Deployment of the tool
   1. Deployment to the development environment
   2. Feedback collection
   3. Refinement of the tool
7. Finalization of the thesis
   1. Finalization thesis' text
   2. Presentation of the thesis

## Related work

Reporting

- supervisor (professor)
- company

## Supporting material

### Literature

#### Books

#### Articles

### Software

- [Docker](https://www.docker.com/)
- [Docker swarm](https://docs.docker.com/engine/swarm/)
- [Kubernetes](https://kubernetes.io/)
- [dotnet](https://dotnet.microsoft.com/)
- [dotnet tools CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools)
- [NuGet](https://www.nuget.org/)
