# Plugin architecture

## Overview

## How plugin deploys

- Get project name as parameter
- Using framework API build a project:
  1. Get project
     - get project's code
     - get project configuration
     - get build commands and entrypoint
  2. Configure
     - set project's configuration in accordance to the target deployment's configuration (environmental values, rewrite configuration files, etc.)
     - set project's build commands and entrypoint in accordance to the target deployment's configuration
  3. Build
     - build project using specified frameworks (dotnet, nodejs, etc.)
     - build specified deployment artifacts (docker image, kubernetes YML, NuGet package, etc.)
     - configure deployment artifacts (set environmental values, rewrite configuration files, etc.)
     - push deployment artifacts to the target deployment (docker registry, kubernetes cluster, etc.)