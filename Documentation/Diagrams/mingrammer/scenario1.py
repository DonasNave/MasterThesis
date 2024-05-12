from diagrams import Cluster, Diagram
from diagrams.programming.language import Csharp
from diagrams.custom import Custom
from diagrams import Edge
from diagrams.programming.language import JavaScript

with Diagram(
    "DTA Stack - Scenario 1",
    show=False,
    direction="LR",
    strict=True,
    filename="Documentation/Diagrams/generated/stack-scenario1",
    outformat="png",
):
    k6 = Custom("Test runner", "../logos/k6.png")
    script = JavaScript("scenario1.js")
    config = JavaScript("config.js")

    with Cluster("Services"):
        fus = Csharp("FUS")
        srs = Csharp("SRS")
        bps = Csharp("BPS")
        eps = Csharp("EPS")

        req_edge = Edge(label="Request /health")

        script >> Edge(label="Load config") >> config
        script >> Edge(label="Load script") >> k6

        k6 >> req_edge >> srs
        k6 >> req_edge >> fus
        k6 >> req_edge >> bps
        k6 >> req_edge >> eps
