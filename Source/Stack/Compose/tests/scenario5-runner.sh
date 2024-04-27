#!/bin/bash

source common_lib.sh

TEST_ID=${1:-test-$(date +%s)}

INFLUXDB_WRITE_URL="http://dta:dtapass@localhost:8086/write?db=k6"

COMPOSE_FILE_NAME="../compose"
COMPOSE_CMD="docker-compose -f ${COMPOSE_FILE_NAME}.yaml"

MEASUREMENT="http_req_duration"

# Function to perform HTTP request and log time
run_http_request() {
    local service_name=$1
    local url=$2
    local compilation=$3

    echo "Starting service $service_name"

    $COMPOSE_CMD up -d $service_name

    # Wait for the service to be fully up by checking health endpoint
    if ! check_health $url; then
        echo "Error: Service failed to start"
        $COMPOSE_CMD down $service_name
        continue  # Skip this iteration
    fi

    signals_endpoint="${url}api/signals/3"

    # Perform HTTP request
    curl -s -o /dev/null -w "Request response code: %{http_code}\n" $signals_endpoint
}

echo "Starting tests for services"

# Loop over services and perform actions
jq -c '.services[] | select((.protocol == "http") and (.name == "SRS"))' config.json | while IFS= read -r service; do
    name=$(echo "$service" | jq -r '.name')
    url=$(echo "$service" | jq -r '.url')
    compilation=$(echo "$service" | jq -r '.compilation')
    protocol=$(echo "$service" | jq -r '.protocol')

    echo "Service data loaded: $name, $compilation, $url, $protocol"

    # Standardize service name for docker-compose
    standardized_name=$(echo "${name}-${compilation}-service" | tr '[:upper:]' '[:lower:]')

    echo "Running HTTP request with startup test for $standardized_name"

    # Repeat the test 10 times
    for i in {1..10}; do
        REAL_TIME=$( { time run_http_request $standardized_name $url $compilation; } 2>&1 | awk '/real/ {print $2}' )
        REAL_TIME_MS=$(echo $REAL_TIME | awk -F'[ms]' '{ print ($1 * 60 * 1000) + ($2 * 1000) }')

        echo "Test completed in $REAL_TIME_MS ms"

        # Log to InfluxDB
        curl -i -XPOST "$INFLUXDB_WRITE_URL" \
            --data-binary "${MEASUREMENT},dta_service=SRS-${compilation},test_scenario=scenario5,test_id=${TEST_ID} value=${REAL_TIME_MS} ${current_time_ns}"
    done

    echo "Stopping service $standardized_name"
    $COMPOSE_CMD down $standardized_name
done

echo "All tests completed."
