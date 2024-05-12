# DTA

Application for testing services. Allows creation of identical services in native AOT and JIT modes.

## Usage

Each service contains a `Dockerfile` that can be used to build the service. The `Dockerfile` is specified by prefix and allows creation of the service in AOT, JIT or JIT.Trimmed mode.

## Source structure

- `/Common` - Contains projects that are shared between services.
- `/Service` - Contains projects that are used to create services.
- `/SolutionItems` - Contains files that are used to create the solution.
