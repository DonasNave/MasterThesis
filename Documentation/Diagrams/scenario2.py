from diagrams import Cluster, Diagram
from diagrams.programming.language import Csharp
from diagrams.custom import Custom
from diagrams import Edge
from diagrams.programming.language import JavaScript
from diagrams.onprem.database import Postgresql

with Diagram(
    "DTA Stack - Scenario 2",
    show=False,
    direction="LR",
    strict=False,
    filename="Documentation/Diagrams/generated/stack-scenario2",
    outformat="png",
):
    k6 = Custom("Test runner", "../logos/k6.png")
    script = JavaScript("scenario2.js")
    config = JavaScript("config.js")
    database = Postgresql("PostgreSQL")

    with Cluster("Services"):
        fus = Csharp("FUS")

        download_edge = Edge(label="Request /api/uploadFile")

        upload_edge = Edge(label="Request /api/downloadFile")

        query_edge = Edge(label="Query File")

        insert_edge = Edge(label="Insert File")

        script >> Edge(label="Load config") >> config
        script >> Edge(label="Load script") >> k6

        k6 << download_edge << fus
        k6 >> upload_edge >> fus

        database >> query_edge >> fus
        database << insert_edge << fus
