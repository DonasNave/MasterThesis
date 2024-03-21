#!/bin/bash

# Set the base directory of your Kubernetes manifests
BASE_DIR="./Stack/K8s"
STORAGE_DIR="./Stack/persistent-storage"
TEST_DIR="$BASE_DIR/tests"

# Check if the base directory exists
if [ ! -d "$BASE_DIR" ]; then
  echo "Base directory does not exist: $BASE_DIR"
  exit 1
fi

# Function to apply Kubernetes manifests
apply_manifests() {
  local dir=$1
  echo "Applying manifests in directory: $dir"
  find $dir -name '*.yaml' -o -name '*.yml' | while read -r file; do
    echo "Applying $file"
    kubectl apply -f "$file"
  done
  echo "--------------------------------"
}

# Apply base manifests
apply_manifests "$BASE_DIR/base"
apply_manifests "$STORAGE_DIR"
apply_manifests "$TEST_DIR"

echo "Deployment complete."
