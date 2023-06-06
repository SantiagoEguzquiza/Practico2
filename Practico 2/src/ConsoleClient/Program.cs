using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitOfWork.SqlServer;

namespace ConsoleClient
{
     class Program
    {

        static void Main(string[] args)
        {
            var unitOfWork = new UnitOfWorkSqlServer();

            var invoiceService = new InvoiceService(unitOfWork);

            var result = invoiceService.GetInvoice(2);

            var invoice = new Invoice
            {
                ProductId = 1,
                Detail = new List<InvoiceDetail>
                {
                    new InvoiceDetail
                    {
                        ProductId = 1,
                        Quantity = 3,
                        Price = 100,
                    },
                    new InvoiceDetail
                    {
                        ProductId = 2,
                        Quantity = 15,
                        Price = 200,
                    }
                }
            };

            //invoiceService.Create(invoice);


            //var invoice = new Invoice
            //{
            //    Id = 6,
            //    ProductId = 1,
            //    Detail = new List<InvoiceDetail>
            //    {
            //        new InvoiceDetail
            //        {
            //            ProductId = 1,
            //            Quantity = 5,
            //            Price = 100,
            //        },
            //        new InvoiceDetail
            //        {
            //            ProductId = 2,
            //            Quantity = 5,
            //            Price = 200,
            //        }
            //    }
            //};

            //invoiceService.Update(invoice);


            invoiceService.Delete(6);

            Console.Read();
        }
    }
}
