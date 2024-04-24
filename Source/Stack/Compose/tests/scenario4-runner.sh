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
    local fus_url=$4

    echo "Starting k6 test for $service_name"
    k6 run --out influxdb=http://dta:dtapass@localhost:8086/k6 scripts/scenario4.js -e SERVICE_URL=$url -e TEST_ID=$TEST_ID -e COMPILATION_MODE=$compilation -e FUS_URL=$fus_url
}

echo "Starting tests for services"

# Loop over services and perform actions
jq -c '.services[] | select((.protocol == "http") and (.name == "EPS"))' config.json | while IFS= read -r service; do
    name=$(echo "$service" | jq -r '.name')
    url=$(echo "$service" | jq -r '.url')
    compilation=$(echo "$service" | jq -r '.compilation')
    protocol=$(echo "$service" | jq -r '.protocol')

    fus_url=$(jq -r '.services[] | select((.compilation == "'$compilation'") and (.protocol == "http") and (.name == "FUS")).url' config.json)
    fus_service_name=$(echo "fus-${compilation}-service" | tr '[:upper:]' '[:lower:]')
    bps_service_name=$(echo "bps-${compilation}-service" | tr '[:upper:]' '[:lower:]')


    echo "Service data loaded: $name, $compilation, $url, $protocol, $fus_url"

    # Standardize service name for docker-compose
    standardized_name=$(echo "${name}-${compilation}-service" | tr '[:upper:]' '[:lower:]')

    echo "Starting service $standardized_name"

    # Start the service
    $COMPOSE_CMD up -d $bps_service_name
    $COMPOSE_CMD up -d $standardized_name

    echo "Waiting for service to start"

    # Run k6 test
    run_k6_test $name $url $compilation $fus_url

    # Stop the service
    $COMPOSE_CMD down $standardized_name
    $COMPOSE_CMD down $fus_service_name
    $COMPOSE_CMD down $bps_service_name
done

echo "All tests completed."
