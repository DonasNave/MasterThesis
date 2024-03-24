#!/bin/bash

# Set the base directory of your Kubernetes manifests
K8S_DIR="./Stack/K8s"
BASE_DIR="$K8S_DIR/base"

CONFIG_DIR="$BASE_DIR/configmaps"
DEPLOYMENT_DIR="$BASE_DIR/deployments"
SERVICE_DIR="$BASE_DIR/services"
INGRESS_DIR="$BASE_DIR/ingress"
SECRETS_DIR="$BASE_DIR/secrets"
STORAGE_DIR="$K8S_DIR/persistent-storage/persistent-volume-claims"
TEST_DIR="$K8S_DIR/tests"

# Check if the base directory exists
if [ ! -d "$K8S_DIR" ]; then
  echo "Base directory does not exist: $K8S_DIR"
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

# Apply deployment
apply_manifests "$CONFIG_DIR"
apply_manifests "$SECRETS_DIR"
apply_manifests "$STORAGE_DIR"
apply_manifests "$INGRESS_DIR"
apply_manifests "$DEPLOYMENT_DIR"
apply_manifests "$SERVICE_DIR"

#apply_manifests "$TEST_DIR"

helm install dta-lgtm-release ./Stack/K8s/charts/lgtm

echo "Deployment complete."
