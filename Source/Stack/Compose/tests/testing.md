# DTA Scenarios tests

Within this folder, you will find the tests for the DTA Scenarios. The tests are written in js and are run using k6.

## Running the tests

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
