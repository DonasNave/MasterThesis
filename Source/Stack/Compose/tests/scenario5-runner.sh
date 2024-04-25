#!/bin/bash

source common_lib.sh

TEST_ID=${1:-test-$(date +%s)}

INFLUXDB_WRITE_URL="http://dta:dtapass@localhost:8086/write?db=k6"

COMPOSE_FILE_NAME="../compose"
COMPOSE_CMD="docker-compose -f ${COMPOSE_FILE_NAME}.yaml"

MEASUREMENT="http_req_duration"
CHECK_INTERVAL=0.01  # Check every 10ms
MAX_WAIT=30  # Maximum number of seconds to wait for the service to become healthy

# Function to perform HTTP request and log time
run_http_request() {
    local service_name=$1
    local url=$2
    local compilation=$3

    # Start time is recorded before attempting to start the service
    START_TIME=$(date +%s%N)  # Start time in nanoseconds

    echo "Starting service $service_name"

    $COMPOSE_CMD up -d $service_name

    # Wait for the service to be fully up by checking health endpoint
    if ! check_health "$url/health"; then
        echo "Error: Service failed to start"
        $COMPOSE_CMD down
        continue  # Skip this iteration
    fi

    # Perform HTTP request
    curl -s -o /dev/null -w "%{http_code}" $url/api/signals/3

    # Measure end time
    END_TIME=$(date +%s%N)  # End time in nanoseconds

    # Calculate duration in milliseconds
    DURATION=$((($END_TIME - $START_TIME)/1000000))

    # Log to InfluxDB
    curl -i -XPOST "$INFLUXDB_WRITE_URL" \
        --data-binary "$MEASUREMENT,dta_service=SRS-$compilation,test_scenario=scenario5,test_id=$TEST_ID, duration=$DURATION"

    echo "Stopping service $service_name"
    $COMPOSE_CMD down
}

echo "Starting tests for services"

# Loop over services and perform actions
jq -c '.services[] | select((.protocol == "http") and (.name == "SRS"))' config.json | while IFS= read -r service; do
    name=$(echo "$service" | jq -r '.name')
    url=$(echo "$service" | jq -r '.url')
    compilation=$(echo "$service" | jq -r '.compilation')
    protocol=$(echo "$service" | jq -r '.protocol')

    # Standardize service name for docker-compose
    standardized_name=$(echo "${name}-${compilation}-service" | tr '[:upper:]' '[:lower:]')

    echo "Running HTTP request test for $standardized_name"

    run_http_request $standardized_name $url $compilation

done

echo "All tests completed."
