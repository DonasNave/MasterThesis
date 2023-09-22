# Project deployment and configuration tool

## Introduction

Working in train development company presents unique software challenges.

Lifetime of a train is 30-40 years. During this time, the train is maintained and repaired. Software and hardware relating to the train (vehicle unit) is expected to stay mostly the same. However supporting server systems might change. Even when software stays the same, current deployment and configuration tools used might not have required life expectancy. Being capable of abstracting crucial logic behind deployment and configuration from the tools used is necessary for the company.

Another challenge is development standardization and norms. Developing software that accesses certain critical systems (like brakes, acceleration, signaling etc.) requires legal norms and standards to be followed. This is to ensure that the software is safe to use and does not cause any harm to the passengers or the train itself. Those standards are set on a EU level* and present a set of rules and methodologies that need to be followed. This might result in a software not being able to be used in a train, because it was not developed using those standards.

\* Currently there is no EU standardization for Server systems, but it is expected to be introduced in near future.

## How do current systems work

Server systems consist of microservices. They are developed with combination of dotnet framework and Angular. Each microservice is developed as a separate project.

Current deployment is done via building each project into a docker image. Those images are deployed to a server using docker swarm. Docker swarm uses YML files to configure the deployment.

## What is needed

These challenges present a need for tool that:

- abstracts logic behind configuration and deployment from the tools currently used (docker swarm/kubernetes) Such tool's code must be
- is available to the company (open source, or company owned) and modifiable to suit the company's needs
- is developed in accordance with the standards set by the company (and by proxy EU legislation)
- is easily modifiable without need for extensive knowledge of the tool's codebase (framework that supports plugins with adequate api)
- usable in a CI/CD pipeline (must be able to be run from command line)

## Desired result

Custom OS dotnet CLI tool, that is composed of core framework that maintains abstracted configuration and deployment logic, a set of rules that are used to validate the configuration and a set of rules that are used to deploy the configuration. This framework will support plugins that will use it's api to attain project/application data and transform it into specific configuration and deployment files.

This tool will be used in at least development environment of my company. In the remaining time before finishing the thesis, feedback and issues will be collected and addressed.

Simply put, if there is a decision to take all those services and move them to kubernetes, the only thing that would need to be done is to write a plugin that would transform the configuration and deployment rules to kubernetes YML files. (and so on for other deployment options)

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

- company
- supervisor (professor)

## Supporting material

### Literature

### Software
