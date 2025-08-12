import grpc
   import customer_pb2
   import customer_pb2_grpc
   import asyncio

   async def unary_call(stub):
       print("Unary RPC Testi:")
       try:
           response = await stub.GetCustomer(customer_pb2.CustomerRequest(id=1))
           print(f"Müşteri: ID={response.id}, İsim={response.name}, E-posta={response.email}, Kayıt={response.registration_date}")
       except grpc.RpcError as e:
           print(f"Hata: {e.details()}")

   async def server_streaming_call(stub):
       print("\nServer Streaming RPC Testi (keyword='example'):")
       request = customer_pb2.CustomerFilterRequest(keyword="example")
       async for response in stub.GetCustomerList(request):
           print(f"Müşteri: ID={response.id}, İsim={response.name}, E-posta={response.email}, Kayıt={response.registration_date}")

   async def server_streaming_all_call(stub):
       print("\nServer Streaming RPC Testi (Tüm Müşteriler):")
       request = customer_pb2.EmptyRequest()
       async for response in stub.GetAllCustomers(request):
           print(f"Müşteri: ID={response.id}, İsim={response.name}, E-posta={response.email}, Kayıt={response.registration_date}")

   async def client_streaming_call(stub):
       print("\nClient Streaming RPC Testi:")
       async def customer_stream():
           customers = [
               customer_pb2.CustomerResponse(id=4, name="Zeynep Çelik", email="zeynep.celik@example.com", registration_date="2025-01-01"),
               customer_pb2.CustomerResponse(id=5, name="Can Öztürk", email="can.ozturk@example.com", registration_date="2025-02-01")
           ]
           for customer in customers:
               yield customer
               await asyncio.sleep(0.5)
       
       response = await stub.UploadCustomers(customer_stream())
       print(f"Yükleme sonucu: {response.message}")

   async def bidirectional_streaming_call(stub):
       print("\nBidirectional Streaming RPC Testi:")
       async def customer_stream():
           customers = [
               customer_pb2.CustomerResponse(id=6, name="Elif Aydın", email="elif.aydin@example.com", registration_date="2025-03-01"),
               customer_pb2.CustomerResponse(id=7, name="Burak Şahin", email="invalid-email", registration_date="2025-04-01")
           ]
           for customer in customers:
               yield customer
               await asyncio.sleep(1)
       
       response_stream = stub.ProcessCustomerStream(customer_stream())
       async for response in response_stream:
           print(f"Doğrulama sonucu: ID={response.id}, İsim={response.name}, Geçerli={response.is_valid}, Mesaj={response.message}")

   async def main():
       async with grpc.aio.insecure_channel('localhost:50051') as channel:
           stub = customer_pb2_grpc.CustomerServiceStub(channel)
           
           await unary_call(stub)
           await server_streaming_call(stub)
           await server_streaming_all_call(stub)
           await client_streaming_call(stub)
           await bidirectional_streaming_call(stub)

   if __name__ == '__main__':
       asyncio.run(main())