#!/bin/bash

# The name of the compose file without the file extension, e.g., "docker-compose"
COMPOSE_FILE_NAME="../compose"
CHECK_INTERVAL=0.01  # Check every 10ms
MAX_WAIT=30  # Maximum number of seconds to wait for the service to become healthy

# The ID of the test run, you can use this to tag your test runs
TEST_ID=$1

# Base command for docker-compose if you have a single compose file or a standardized naming convention
COMPOSE_CMD="docker-compose -f ${COMPOSE_FILE_NAME}.yaml"

# InfluxDB configuration
INFLUXDB_WRITE_URL="http://dta:dtapass@localhost:8086/write?db=k6"

MEASUREMENT="http_req_duration"

# Function to check service health
check_health() {
    local health_url=$1
    for ((i=0; i<=$MAX_WAIT; i++)); do
        if curl -f $health_url >/dev/null 2>&1; then
            echo "Service is up after $(echo "$i * $CHECK_INTERVAL" | bc) seconds"
            return 0
        fi
        sleep $CHECK_INTERVAL
    done
    echo "Service did not become healthy in time"
    return 1
}

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
