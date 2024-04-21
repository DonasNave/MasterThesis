# DTA Scenarios tests

Within this folder, you will find the tests for the DTA Scenarios. The tests are written in js and are run using k6.

## Running the tests

Running the tests requires bash and docker to be installed on your machine. Additional requirement is jq library/tool.

Running test scenarios is as simple as launching scenarioX-test.sh script. The script will load the required environment variables, launches services and tests as necessary

```bash
sh ./scenario1-runner.sh <test-id>
```

Where `<test-id>` is name of test instance for further identification within Grafana.

## Running raw scripts

### Local

To run the tests, you will need to have k6 installed. You can find the installation instructions [here](https://k6.io/docs/getting-started/installation/).

Once you have k6 installed, you can run the tests by running the following command:

```bash
k6 run --out influxdb=http://dta:dtapass@localhost:8086/k6 <test-file>
```

Where `<test-file>` is the path to the test file you want to run.

### Docker

You can also run the tests using docker. To do so, you can run the following command:

```bash
docker run --network compose_stack-network -v ${PWD}/scripts:/scripts -i grafana/k6 run /scripts/<test-file>
```

Where `<test-file>` is the path to the test file you want to run.

## Scenario 1

This scenario tests the performance of the DTA API when creating a new user.

## Scenario 2

This scenario tests the performance of the DTA API when creating a new user and then logging in.

## Scenario 3

This scenario tests the performance of the DTA API when creating a new user, logging in, and then creating a new project.

## Scenario 4

This scenario tests the performance of the DTA API when creating a new user, logging in, creating a new project, and then creating a new task.

## Scenario 5

This scenario tests the performance of the DTA API when creating a new user, logging in, creating a new project, creating a new task, and then updating the task.
