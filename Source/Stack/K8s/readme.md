# DTA Stack

```mermaid
graph TD;
    subgraph DB [Database]
        Postgres
    end

    subgraph JIT_Services [JIT Services]
        JIT_SRS[SRS]
        JIT_FUS[FUS]
    end

    subgraph AoT_Services [AoT Services]
        AOT_FUS[FUS]
        AOT_SRS[SRS]
    end

    subgraph Monitoring [LGTM - Monitoring]
        Prometheus
        Grafana
        Loki
        Tempo
        OtelCollector[OpenTelemetry Collector]
    end

    subgraph Ingress [Ingress]
        Nginx
    end

    subgraph Tests [Tests]
        K6
    end

    JIT_FUS --> Postgres
    AOT_FUS --> Postgres

    K6 --> JIT_Services
    K6 --> AoT_Services

    JIT_Services --> OtelCollector
    AoT_Services --> OtelCollector
    Nginx --> Grafana

    OtelCollector --> Prometheus
    Prometheus --> Grafana
    Loki --> Grafana
    Tempo --> Grafana
    OtelCollector --> Loki
    OtelCollector --> Tempo

```

## Services

- LGTM (Liveness, Giveness, Traffic, Monitoring)
  - Prometheus
  - Grafana
  - Loki
  - Tempo
  - OpenTelemetry Collector
- Postgres
- Nginx
- DTA Services (JIT)
  - FUS
  - SRS
- DTA Services (AoT)
  - FUS
  - SRS

## Description

## Pre-requisites

## Usage

## Plugins

KNative

Kourier
