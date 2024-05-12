from diagrams import Cluster, Diagram
from diagrams.onprem.logging import Loki
from diagrams.onprem.tracing import Tempo
from diagrams.onprem.database import Postgresql, Influxdb
from diagrams.onprem.monitoring import Grafana, Prometheus
from diagrams.onprem.network import Nginx
from diagrams.onprem.queue import Rabbitmq
from diagrams.programming.language import Csharp
from diagrams.custom import Custom

with Diagram(
    "DTA Stack - Data Flow",
    show=False,
    direction="LR",
    strict=True,
    filename="Documentation/Diagrams/generated/stack-data-flow",
    outformat="png",
):
    influx = Influxdb("InfluxDB")
    k6 = Custom("Testing", "../logos/k6.png")

    lgtm = Cluster("LGTM")
    services = Cluster("Services")
    aot = Cluster("AOT")
    jit = Cluster("JIT")

    otel = Custom("OTel", "../logos/otel.png")
    node = Prometheus("Node Exporter")
    cadvisor = Custom("Cadvisor", "../logos/cadvisor.png")

    with lgtm:
        grafana = Grafana("Monitoring")
        prom = Prometheus("Metrics")
        tempo = Tempo("Tracing")
        loki = Loki("Logging")
        otel >> prom >> grafana
        otel >> tempo >> grafana
        otel >> loki >> grafana
        k6 >> influx >> grafana

        node >> prom
        cadvisor >> prom

    with aot:
        fus_aot = Csharp("FUS")
        srs_aot = Csharp("SRS")
        bps_aot = Csharp("BPS")
        eps_aot = Csharp("EPS")

        bps_aot >> fus_aot

    with jit:
        fus_jit = Csharp("FUS")
        srs_jit = Csharp("SRS")
        bps_jit = Csharp("BPS")
        eps_jit = Csharp("EPS")

        bps_jit >> fus_jit

    fus_aot >> otel
    fus_jit >> otel
    srs_aot >> otel
    srs_jit >> otel
    bps_aot >> otel
    bps_jit >> otel
    eps_aot >> otel
    eps_jit >> otel

    k6 >> services
