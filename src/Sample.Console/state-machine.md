### Sample.Consumer.StateMachine.OrderingStateMachine

``` mermaid
flowchart TB;
    0(["Initial"]) --> 4["OrderCreated«OrderCreated»"];
    1(["CreatingPayment"]) --> 5["PaymentCreated«PaymentCreated»"];
    2(["Paying"]) --> 6["PaymentDone«PaymentDone»"];
    2(["Paying"]) --> 7["PaymentFailed«PaymentFailed»"];
    4["OrderCreated«OrderCreated»"] --> 1(["CreatingPayment"]);
    5["PaymentCreated«PaymentCreated»"] --> 2(["Paying"]);
    6["PaymentDone«PaymentDone»"] --> 3(["Final"]);
    7["PaymentFailed«PaymentFailed»"] --> 3(["Final"]);
```
