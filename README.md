# gRPC Customer Service Example

This project demonstrates a gRPC application showcasing four communication patterns: Unary, Server Streaming, Client Streaming, and Bidirectional Streaming using a customer service example.

## gRPC Methods

- Unary: Retrieve a single customer by ID (GetCustomer)

- Server Streaming: Stream filtered customers (GetCustomerList) or all customers (GetAllCustomers)

- Client Streaming: Upload multiple customers (UploadCustomers)

- Bidirectional Streaming: Send customers and receive real-time validation (ProcessCustomerStream)

  ## Technologies

- Server: .NET (6.0+), Grpc.Tools (2.66.0), Google.Protobuf (3.27.2)

- Client: Python (3.7+), grpcio, grpcio-tools

- Protocol: gRPC with Protocol Buffers (customer.proto)
