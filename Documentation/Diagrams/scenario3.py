from diagrams import Cluster, Diagram
from diagrams.programming.language import Csharp
from diagrams.custom import Custom
from diagrams import Edge
from diagrams.programming.language import JavaScript

with Diagram(
    "DTA Stack - Scenario 3",
    show=False,
    direction="LR",
    strict=False,
    filename="Documentation/Diagrams/generated/stack-scenario3",
    outformat="png",
):
    k6 = Custom("Test runner", "../logos/k6.png")
    script = JavaScript("scenario3.js")
    config = JavaScript("config.js")

    with Cluster("Services"):
        fus = Csharp("BPS")

        fib_edge = Edge(label="Request /api/processFibonacci")

        fact_edge = Edge(label="Request /api/processFactorial")

        query_edge = Edge(label="Query File")

        insert_edge = Edge(label="Insert File")

        script >> Edge(label="Load config") >> config
        script >> Edge(label="Load script") >> k6

        k6 >> fib_edge >> fus
        k6 >> fact_edge >> fus
