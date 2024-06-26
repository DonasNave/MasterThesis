FROM redhat/ubi8:8.9

ENV GRAFANA_VERSION="10.4.1"
ENV PROMETHEUS_VERSION="2.51.0"
ENV TEMPO_VERSION="2.4.1"
ENV LOKI_VERSION="2.9.6"
ENV OPENTELEMETRY_COLLECTOR_VERSION="0.96.0"

ARG TARGETARCH
ENV TARGETARCH=${TARGETARCH}

RUN mkdir /otel-lgtm
WORKDIR /otel-lgtm

RUN yum install -y unzip jq

RUN curl -sOL https://dl.grafana.com/oss/release/grafana-$GRAFANA_VERSION.linux-${TARGETARCH}.tar.gz && \
    tar xfz grafana-$GRAFANA_VERSION.linux-${TARGETARCH}.tar.gz && \
    rm grafana-$GRAFANA_VERSION.linux-${TARGETARCH}.tar.gz

RUN curl -sOL https://github.com/prometheus/prometheus/releases/download/v$PROMETHEUS_VERSION/prometheus-$PROMETHEUS_VERSION.linux-${TARGETARCH}.tar.gz && \
    tar xfz prometheus-$PROMETHEUS_VERSION.linux-${TARGETARCH}.tar.gz && \
    mv prometheus-$PROMETHEUS_VERSION.linux-${TARGETARCH} prometheus-$PROMETHEUS_VERSION && \
    rm prometheus-$PROMETHEUS_VERSION.linux-${TARGETARCH}.tar.gz

RUN curl -sOL https://github.com/grafana/tempo/releases/download/v$TEMPO_VERSION/tempo_${TEMPO_VERSION}_linux_${TARGETARCH}.tar.gz && \
    mkdir tempo-$TEMPO_VERSION/ && \
    tar xfz tempo_${TEMPO_VERSION}_linux_${TARGETARCH}.tar.gz -C tempo-$TEMPO_VERSION/ && \
    rm tempo_${TEMPO_VERSION}_linux_${TARGETARCH}.tar.gz

RUN curl -sOL https://github.com/grafana/loki/releases/download/v$LOKI_VERSION/loki-linux-${TARGETARCH}.zip && \
    mkdir loki-$LOKI_VERSION && \
    unzip loki-linux-${TARGETARCH} -d loki-$LOKI_VERSION/ && \
    rm loki-linux-${TARGETARCH}.zip

RUN curl -sOL https://github.com/open-telemetry/opentelemetry-collector-releases/releases/download/v$OPENTELEMETRY_COLLECTOR_VERSION/otelcol-contrib_${OPENTELEMETRY_COLLECTOR_VERSION}_linux_${TARGETARCH}.tar.gz && \
    mkdir otelcol-contrib-$OPENTELEMETRY_COLLECTOR_VERSION && \
    tar xfz otelcol-contrib_${OPENTELEMETRY_COLLECTOR_VERSION}_linux_${TARGETARCH}.tar.gz -C otelcol-contrib-$OPENTELEMETRY_COLLECTOR_VERSION/ && \
    rm otelcol-contrib_${OPENTELEMETRY_COLLECTOR_VERSION}_linux_${TARGETARCH}.tar.gz

COPY prometheus.yaml .
COPY run-prometheus.sh .
COPY grafana-datasources.yaml ./grafana-v$GRAFANA_VERSION/conf/provisioning/datasources/
COPY grafana-dashboards.yaml grafana-v$GRAFANA_VERSION/conf/provisioning/dashboards/
# copy custom dashboards
COPY dashboards dashboards/
COPY run-grafana.sh .
COPY loki-config.yaml .
COPY run-loki.sh .
COPY tempo-config.yaml .
COPY run-tempo.sh .
COPY otelcol-config.yaml .
COPY run-otelcol.sh .
COPY run-all.sh .

CMD ./run-all.sh
