#!/bin/bash

# Function to check service health
check_health() {
    local health_url=$1"health"
    local max_wait=${2:-30}
    local check_interval=${3:-0.01}

    echo "Waiting for service to start"

    local interval_ms=$(echo "$check_interval*1000/1" | bc)
    local max_attempts=$(echo "$max_wait/$check_interval" | bc)

    for ((i=0; i<=$max_attempts; i++)); do
        if curl -f $health_url >/dev/null 2>&1; then
            local seconds_up=$(echo "scale=2; $i*$check_interval" | bc)
            echo "Service is up after $seconds_up seconds"
            return 0
        fi
        sleep $check_interval
    done

    echo "Service did not become healthy in time"
    return 1
}
