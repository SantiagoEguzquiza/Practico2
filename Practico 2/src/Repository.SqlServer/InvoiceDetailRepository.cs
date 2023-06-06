using Models;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.Remoting.Contexts;

namespace Repository.SqlServer
{
    public class InvoiceDetailRepository : Repository, IInvoiceDetailRepository
    {


        public InvoiceDetailRepository(SqlConnection context, SqlTransaction transaction)
        {
            this._transaction = transaction;
            this._context = context;
        }
        public InvoiceDetail Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<InvoiceDetail> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Create(IEnumerable<InvoiceDetail> model, int invoiceId)
        {
            foreach (var detail in model)
            {
                var query = " insert into invoicesDetail (productId, invoiceId, quantity, precio, iva, subTotal, total) values(@productId, @invoiceId, @quantity, @precio, @iva, @subTotal, @total)";
                var command = CreateCommand(query);

                command.Parameters.AddWithValue("@productId", detail.ProductId);
                command.Parameters.AddWithValue("@invoiceId", invoiceId);
                command.Parameters.AddWithValue("@quantity", detail.Quantity);
                command.Parameters.AddWithValue("@precio", detail.Price);
                command.Parameters.AddWithValue("@iva", detail.Iva);
                command.Parameters.AddWithValue("@subTotal", detail.SubTotal);
                command.Parameters.AddWithValue("@total", detail.Total);

                command.ExecuteNonQuery();
            }
        }

        public IEnumerable<InvoiceDetail> GetAllByInvoiceId(int invoiceId)
        {
            var result = new List<InvoiceDetail>();
            var command = CreateCommand("SELECT * FROM invoicesDetail WITH(NOLOCK) WHERE invoiceId = @invoiceId");
            command.Parameters.AddWithValue("@invoiceId", invoiceId);


            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new InvoiceDetail
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        ProductId = Convert.ToInt32(reader["productId"]),
                        Quantity = Convert.ToInt32(reader["quantity"]),
                        Iva = Convert.ToDecimal(reader["iva"]),
                        SubTotal = Convert.ToDecimal(reader["subTotal"]),
                        Total = Convert.ToDecimal(reader["total"]),


                    });
                }

            }
            return result;
        }

        public void RemoveByInvoiceId(int invoiceId)
        {
            var command = CreateCommand("DELETE FROM invoicedetail WHERE invoiceId = @invoiceId");
            command.Parameters.AddWithValue("@invoiceId", invoiceId);

            command.ExecuteNonQuery();
        }
    }
}