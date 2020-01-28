# Email Router Service

The prototype can be launched with `make build && make up`. To view logging output stream, use `make logs`.

Problem Breakdown
---------------------------

- High-level characteristics of email delivery
	- Critical in nature, failure is unacceptable (particularly for Transactional)
	- Bulk email will cause very spiky load (customers may send thousands/millions of emails at once)
	- System must be able to respond to load spikes without manual intervention

- Needs/expectations of customers using the service
	- Must be fast to respond (ensure customer's infrastructure is not burdened down waiting for sending)
	- Delivery will be asynchronous (an accepted convention when dealing with email sending)
	- Should communicate errors clearly (asychronously)
		- Reporting via customer's account UI
		- Email alerting opt-in?

- Functionality required of the service
	- Receive email send request
	- Perform validation
	- Perform categorization
	- Send to delivery pipeline

Implementation Ideas
---------------------------

- Message Routing Service (MessageRouter)
	- Background worker microservice, operating on the competing consumer pattern
	- Completely separate from public API service
		- Public API pushes send requests to the routing service via message queue
		- Allows for optimal responsiveness for customer requests
		- Can scale routing workers independently
		- Isolate failure from customer-facing

- Public API Service (PublicAPI):
	- Performs basic request model validation, customer authentication
	- On success, publish message(s) to a message queue exchange
		- Queue/consumers could be partitioned based on customer id, location, etc.
		- Routing key could be a simple queue name for direct exchange, or more complicated routing if necessary

- MessageRouter will be hosted as a containerized service, orchestrated via Kubernetes
	- Horizontal scaling is very simple, autoscale based on message queue length
	- Add external metrics for message queue to k8s cluster, setup HorizontalPodAutoscaler with replica specs based on these metrics (described in more detail later on)

- MessageRouter processing pipeline:
	- Receive message from queue
	- Process email validation rules
		- On failure, publish to rejected queue for further processing? ie. notify customer, etc
	- Process email categorization
	- Publish message to delivery queue (or rejected queue?)


Work Outline
---------------------------

The following is a brief outline of the steps I would take to fully complete this work. As part of this exercise I have only implemented the first stages of the MessageRouter service (step 2).

1) Clarify requirements, define domain language
	- Have a discussion with the product owner to ensure requirements are clearly understood, ask questions to clear up ambiguity
	- Discuss with team members and ensure domain concepts and language to be used is clear and accurate - critical for clear and correct code
	- This is always important, but would be particularly important for this project, being the first time I have worked on this system

2) Start building MessageRouter service
	- Define message schema this service is expected to receive
	- Sketch out validation, categorization, delivery steps
	- Start writing some unit tests for the various steps
	- Ensure a test message can go from end to end through the service (possibly with some functionality mocked out for now)

3) Make changes to Public API service
	- Implement changes to the service required for it to publish email requests to the message queue

4) Setup the two services so they can be run locally, test end-to-end by making requests to the Public API service
	- Consider writing or planning integration tests at this point

5) Continue fleshing out MessageRouter service
	- Implement all valiation rules, categorization rules
	- Implement message publishing to delivery pipelines
	- Implement failure handling for rejected messages
		- Write failures to database?
		- Should we notify the user?
		- Handled by another consumer? Perhaps publish a message to another queue ...
	- Write more unit tests / integration tests
  - Consider load testing with production workloads


Performance & HA (Autoscaling w/ Kubernetes)
---------------------------
- Kubernetes makes it easy to scale and distribute services according to our needs
- By using HorizontalPodAutoscaler and k8s metrics API we can configure our service deployment to add/remove replicas based on message queue length (or other metrics ie. CPU utilization) - see example below.
- By setting the minReplicas count to a sufficiently high value, and by configuring Kubernetes to distribute our pods across the cluster, we provide redundancy via multiple replicas across multiple host nodes. Additionally our cluster nodes can be distributed across multiple availability zones, giving us protection against data center failure.

HPA configuration:
---------------------------
- Requirements (assuming RabbitMQ):
	- RabbitMQ metrics exporter - https://github.com/kbudde/rabbitmq_exporter
	- Metrics adapter to expose via k8s metrics API - https://github.com/zalando-incubator/kube-metrics-adapter

- The below HPA targets our MessageRouter service, scaling between 5-20 replicas as needed to achieve a ready queue length of 30. Obviously these numbers would need to be tested and tweaked based on production usage requirements ...

apiVersion: autoscaling/v2beta1
kind: HorizontalPodAutoscaler
metadata:
  name: message-router-hpa
  annotations:
  	metric-config.external.prometheus-query.prometheus/prometheus-server: http://prometheus.default.svc
  	metric-config.external.prometheus-query.prometheus/message-queue-length: |
      rabbitmq_queue_messages_ready{queue="email-send-queue"}
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: message-router
  minReplicas: 5
  maxReplicas: 20
  metrics:
  - type: External
	  external:
	  	metricName: prometheus-query
      metricSelector:
        matchLabels:
          query-name: message-queue-length
      targetAverageValue: 30


NOTES/TODO
---------------------------
- Concept of "sending pipelines" and what that entails is not really fleshed out. Currently just publishing a message based on the determined category
- I would move message definitions out into a separate class library, as these would need to be shared by other services that are publishing messages (ie. Public API Service)
- Message schema versioning would also be a consideration
- Implement basic instrumentation for message queue processing, load test
- Setup k8s manifests for deployment
- Write unit tests
