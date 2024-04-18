#!/bin/bash

# The name of the compose file without the file extension, e.g., "docker-compose"
COMPOSE_FILE_NAME="../compose"

# The ID of the test run, you can use this to tag your test runs
TEST_ID=$1

# Base command for docker-compose if you have a single compose file or a standardized naming convention
COMPOSE_CMD="docker-compose -f ${COMPOSE_FILE_NAME}.yaml"

# Define a function to run k6 test
run_k6_test() {
    local service_name=$1
    local url=$2
    local compilation=$3

    echo "Starting k6 test for $service_name"
    k6 run --out influxdb=http://localhost:8086/k6 scripts/scenario3.js -e SERVICE_URL=$url -e TEST_ID=$TEST_ID -e COMPILATION_MODE=$compilation
}

SERVICES=$(yq e '.services[] | select(.name == "BPS")' config.yaml)

# Loop over services and perform actions
for service in $SERVICES; do
    name=$(echo $service | yq e '.name' -)
    compilation=$(echo $service | yq e '.compilation' -)
    url=$(echo $service | yq e '.url' -)
    protocol=$(echo $service | yq e '.protocol' -)

    # Standardize service name for docker-compose
    standardized_name=$(echo "${service_name}-${compilation}-service" | tr '[:upper:]' '[:lower:]')

    # Start the service
    $COMPOSE_CMD up -d $standardized_name

    # Run k6 test
    run_k6_test $name $url $compilation

    # Stop the service
    $COMPOSE_CMD down $standardized_name
done

echo "All tests completed."
