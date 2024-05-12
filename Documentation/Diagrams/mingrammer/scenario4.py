from diagrams import Cluster, Diagram
from diagrams.programming.language import Csharp
from diagrams.custom import Custom
from diagrams import Edge
from diagrams.onprem.queue import Rabbitmq
from diagrams.programming.language import JavaScript
from diagrams.onprem.database import Postgresql

with Diagram(
    "DTA Stack - Scenario 4",
    show=False,
    direction="LR",
    strict=False,
    filename="Documentation/Diagrams/generated/stack-scenario4",
    outformat="png",
):
    k6 = Custom("Test runner", "../logos/k6.png")
    script = JavaScript("scenario4.js")
    config = JavaScript("config.js")
    database = Postgresql("PostgreSQL")
    rabbit = Rabbitmq("Message Broker")

    with Cluster("Services"):
        fus = Csharp("FUS")
        eps = Csharp("EPS")
        bps = Csharp("BPS")

        download_edge = Edge(label="Request /api/uploadFile")

        upload_edge = Edge(label="Request /api/downloadFile")

        request_publish_edge = Edge(label="Request /api/simulateEvent")

        publish_edge = Edge(label="Publish Event")
        consume_edge = Edge(label="Consume Event")

        get_file_edge = Edge(label="RPC GetFile")

        query_edge = Edge(label="Query File")

        script >> Edge(label="Load config") >> config
        script >> Edge(label="Load script") >> k6

        k6 >> request_publish_edge >> eps

        eps >> publish_edge >> rabbit
        bps << consume_edge << rabbit

        bps << get_file_edge << fus

        fus << query_edge << database
