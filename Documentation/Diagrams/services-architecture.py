from diagrams import Cluster, Diagram
from diagrams.onprem.database import Postgresql
from diagrams.onprem.queue import Rabbitmq
from diagrams.programming.language import Csharp
from diagrams.custom import Custom

with Diagram(
    "DTA Stack - Services Architecture",
    show=False,
    direction="LR",
    strict=True,
    filename="Documentation/Diagrams/generated/services-architecture",
    outformat="png",
):
    rabbit = Rabbitmq("Message Broker")
    database = Postgresql("PostgreSQL")

    services = Cluster("Services")
    aot = Cluster("AOT")
    jit = Cluster("JIT")

    otel = Custom("OTel", "../logos/otel.png")

    with services:
        fus = Csharp("FUS")
        srs = Csharp("SRS")
        bps = Csharp("BPS")
        eps = Csharp("EPS")

        eps >> rabbit >> bps
        bps >> fus

    fus >> database

    fus >> otel
    srs >> otel
    bps >> otel
    eps >> otel
