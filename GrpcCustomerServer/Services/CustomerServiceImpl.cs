using Grpc.Core;
using GrpcCustomerServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcCustomerServer
{
    public class CustomerServiceImpl : CustomerService.CustomerServiceBase
    {
        private readonly List<CustomerResponse> customers = new()
           {
               new CustomerResponse { Id = 1, Name = "Ali Y�lmaz", Email = "ali.yilmaz@example.com", RegistrationDate = "2023-01-15" },
               new CustomerResponse { Id = 2, Name = "Ay�e Kaya", Email = "ayse.kaya@example.com", RegistrationDate = "2023-06-20" },
               new CustomerResponse { Id = 3, Name = "Mehmet Demir", Email = "mehmet.demir@example.com", RegistrationDate = "2024-02-10" }
           };

        public override Task<CustomerResponse> GetCustomer(CustomerRequest request, ServerCallContext context)
        {
            var customer = customers.FirstOrDefault(c => c.Id == request.Id);
            if (customer == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "M��teri bulunamad�."));
            }
            return Task.FromResult(customer);
        }

        public override async Task GetCustomerList(CustomerFilterRequest request, IServerStreamWriter<CustomerResponse> responseStream, ServerCallContext context)
        {
            var filteredCustomers = customers.Where(c =>
                c.Name.Contains(request.Keyword, StringComparison.OrdinalIgnoreCase) ||
                c.Email.Contains(request.Keyword, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var customer in filteredCustomers)
            {
                await responseStream.WriteAsync(customer);
                await Task.Delay(1000);
            }
        }

        public override async Task GetAllCustomers(EmptyRequest request, IServerStreamWriter<CustomerResponse> responseStream, ServerCallContext context)
        {
            foreach (var customer in customers)
            {
                await responseStream.WriteAsync(customer);
                await Task.Delay(1000);
            }
        }

        public override async Task<UploadResponse> UploadCustomers(IAsyncStreamReader<CustomerResponse> requestStream, ServerCallContext context)
        {
            int count = 0;
            while (await requestStream.MoveNext())
            {
                var customer = requestStream.Current;
                Console.WriteLine($"Sunucu: Yeni m��teri al�nd�: {customer.Name} ({customer.Email})");
                customers.Add(customer);
                count++;
            }
            return new UploadResponse
            {
                TotalUploaded = count,
                Message = $"{count} m��teri ba�ar�yla y�klendi."
            };
        }

        public override async Task ProcessCustomerStream(IAsyncStreamReader<CustomerResponse> requestStream, IServerStreamWriter<ValidationResponse> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var customer = requestStream.Current;
                Console.WriteLine($"Sunucu: M��teri al�nd�: {customer.Name} ({customer.Email})");

                bool isValid = customer.Email.Contains("@");
                string message = isValid ? "M��teri bilgisi ge�erli." : "Ge�ersiz e-posta adresi.";

                await responseStream.WriteAsync(new ValidationResponse
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    IsValid = isValid,
                    Message = message
                });
            }
        }
    }
}
