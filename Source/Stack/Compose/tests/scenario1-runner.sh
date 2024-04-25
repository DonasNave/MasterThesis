#!/bin/bash

source common_lib.sh

TEST_ID=${1:-test-$(date +%s)}

SCENARIO_FILE="scripts/scenario1.js"
INFLUXDB_WRITE_URL="http://dta:dtapass@localhost:8086/k6"

COMPOSE_FILE_NAME="../compose"
COMPOSE_CMD="docker-compose -f ${COMPOSE_FILE_NAME}.yaml"

# Define a function to run k6 test
run_k6_test() {
    local service_name=$1
    local url=$2
    local compilation=$3

    echo "Starting k6 test for $service_name"
    k6 run --out influxdb=$INFLUXDB_WRITE_URL $SCENARIO_FILE -e SERVICE_URL=$url -e SERVICE_NAME=$service_name -e TEST_ID=$TEST_ID -e COMPILATION_MODE=$compilation
}

echo "Starting tests for services"

# Loop over services and perform actions
jq -c '.services[] | select(.protocol == "http")' config.json | while IFS= read -r service; do
    name=$(echo "$service" | jq -r '.name')
    url=$(echo "$service" | jq -r '.url')
    compilation=$(echo "$service" | jq -r '.compilation')
    protocol=$(echo "$service" | jq -r '.protocol')

    echo "Service data loaded: $name, $compilation, $url, $protocol"

    # Standardize service name for docker-compose
    standardized_name=$(echo "${name}-${compilation}-service" | tr '[:upper:]' '[:lower:]')

    echo "Starting service $standardized_name"

    # Start the service
    $COMPOSE_CMD up -d $standardized_name

    # Wait for the service to be fully up by checking health endpoint
    if ! check_health $url; then
        echo "Error: Service failed to start"
        $COMPOSE_CMD down $standardized_name
        continue  # Skip this iteration
    fi

    # Run k6 test
    run_k6_test $name $url $compilation

    # Stop the service
    $COMPOSE_CMD down $standardized_name
done

echo "All tests completed."
