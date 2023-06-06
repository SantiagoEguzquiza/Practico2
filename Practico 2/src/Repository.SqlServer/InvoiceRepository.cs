using Models;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace Repository.SqlServer
{
    public class InvoiceRepository : Repository, IInvoiceRepository
    {


        public InvoiceRepository(SqlConnection context, SqlTransaction transaction) 
        { 
            this._transaction = transaction;
            this._context = context;
        }
        public void Create(global::Models.Invoice model)
        {
            var query = " insert into invoice (clientId, iva, subTotal, total) output INSERTED.ID values(@clientId, @iva, @subTotal, @total)";
            var command =CreateCommand(query);

            command.Parameters.AddWithValue("@clientId", model.ProductId);
            command.Parameters.AddWithValue("@iva", model.Iva);
            command.Parameters.AddWithValue("@subTotal", model.SubTotal);
            command.Parameters.AddWithValue("@total", model.Total);

            model.Id = Convert.ToInt32(command.ExecuteScalar());
        }

        public global::Models.Invoice Get(int id)
        {

            var result = new Invoice();

            var command = CreateCommand("SELECT * FROM invoice WHERE id = @id");
            command.Parameters.AddWithValue("@id", id);

            using (var reader = command.ExecuteReader())
            {
                reader.Read();

                result.Id = Convert.ToInt32(reader["id"]);
                result.Iva = Convert.ToDecimal(reader["iva"]);
                result.SubTotal = Convert.ToDecimal(reader["subTotal"]);
                result.Total = Convert.ToDecimal(reader["total"]);
                result.ProductId = Convert.ToInt32(reader["clientId"]);

            }

            return result;
        }

        public IEnumerable<global::Models.Invoice> GetAll()
        {
            var result = new List<Invoice>();

            var command = CreateCommand("Select * from invoices WITH(NOLOCK)");

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new Invoice
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Iva = Convert.ToDecimal(reader["iva"]),
                        SubTotal = Convert.ToDecimal(reader["subTotal"]),
                        Total = Convert.ToDecimal(reader["total"]),
                        ClientId = Convert.ToInt32(reader["clientId"])
                    });
                }

                return result;
            }
        }

        public void Remove(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(global::Models.Invoice t)
        {
            throw new NotImplementedException();
        }
    }
}
