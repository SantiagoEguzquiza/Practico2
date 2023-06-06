using Common;
using Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using UnitOfWork.Interfaces;

namespace Services
{
    public class InvoiceService
    {
        private IUnitOfWork _unitOfWork;

        public InvoiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Invoice> GetAll()
        {
            using (var context = _unitOfWork.Create())
            {
                var records = context.Repositories.InvoiceRepository.GetAll();

                foreach (var record in records)
                {
                    record.Client = context.Repositories.ClientRepository.Get(record.ClientId);
                    record.Detail = context.Repositories.InvoiceDetailRepository.GetAllByInvoiceId(record.Id);

                    foreach (var item in record.Detail)
                    {
                        item.Product = context.Repositories.ProductRepository.Get(item.ProductId);
                    }

                }
                return records;
            }
        }

        //public IEnumerable<Invoice> GetAll()
        //{
        //    var result = new List<Invoice>();

        //    using (var context = new SqlConnection(Parameters.ConnectionString))
        //    {
        //        context.Open();

        //        var command = new SqlCommand("SELECT * FROM Invoices", context);
        //        using (var reader = command.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                var invoice = new Invoice
        //                {
        //                    Id = Convert.ToInt32(reader["Id"]),
        //                    Iva = Convert.ToDecimal(reader["Iva"]),
        //                    SubTotal = Convert.ToDecimal(reader["SubTotal"]),
        //                    Total = Convert.ToDecimal(reader["Total"]),
        //                    ProductId = Convert.ToInt32(reader["ClientId"])
        //                };
        //                result.Add(invoice);
        //            }
        //        }



        //        // set aditional properties
        //        foreach (var invoice in result)
        //        {
        //            //Client
        //            SetClient(invoice, context);

        //            //Detail
        //            SetDetail(invoice, context);
        //        }
        //    }
        //    return result;
        //}

        public Invoice GetInvoice(int id)
        {


            //var result = new Invoice();

            using (var context = _unitOfWork.Create())
            {
                var result = context.Repositories.InvoiceRepository.Get(id);

                result.Client = context.Repositories.ClientRepository.Get(result.ClientId);
                result.Detail = context.Repositories.InvoiceDetailRepository.GetAllByInvoiceId(result.Id);

                foreach (var item in result.Detail)
                {
                    item.Product = context.Repositories.ProductRepository.Get(item.ProductId);
                }

                return result;
            }


            //using (var context = new SqlConnection(Parameters.ConnectionString))
            //{
            //    context.Open();

            //    var command = new SqlCommand("SELECT * FROM invoice WHERE id = @id", context);
            //    command.Parameters.AddWithValue("@id", id);

            //    using (var reader = command.ExecuteReader())
            //    {
            //        reader.Read();

            //        result.Id = Convert.ToInt32(reader["id"]);
            //        result.Iva = Convert.ToDecimal(reader["iva"]);
            //        result.SubTotal = Convert.ToDecimal(reader["subTotal"]);
            //        result.Total = Convert.ToDecimal(reader["total"]);
            //        result.ProductId = Convert.ToInt32(reader["clientId"]);

            //    }

            //    SetClient(result, context);

            //    SetDetail(result, context);
            //}

            //return result;
        }

        public void Create(Invoice model)
        {
            PrepareOrder(model);
            //using (var transaction = new TransactionScope())
            //{
            //    using (var context = new SqlConnection(Parameters.ConnectionString))
            //    {
            //        context.Open();

            //        Header
            //        AddHeader(model, context);

            //        Detail
            //        AddDetail(model, context);
            //    }

            //    transaction.Complete();
            //}

            using (var context = _unitOfWork.Create())
            {
                //Header
                context.Repositories.InvoiceRepository.Create(model);
                // detail
                context.Repositories.InvoiceDetailRepository.Create(model.Detail, model.Id);

                //confirm changes
                context.SaveChanges();
            }




        }
        public void Update(Invoice model)
        {
            PrepareOrder(model);


            //using (var transaction = new TransactionScope())



            //using (var context = new SqlConnection(Parameters.ConnectionString))
            //{


            //    //Header
            //    //UpdateHeader(model, context);

            //    //Remove
            //    //RemoveDetail(model.Id, context);

            //    //Detail
            //    //AddDetail(model, context);


            //}
            //transaction.Complete();

            using (var context = _unitOfWork.Create())
            {
                context.Repositories.InvoiceRepository.Update(model);

                context.Repositories.InvoiceDetailRepository.RemoveByInvoiceId(model.Id);
                context.Repositories.InvoiceDetailRepository.Create(model.Detail, model.Id);

                context.SaveChanges();
            }
        }
        public void Delete(int id)
        {
            //using (var context = new SqlConnection(Parameters.ConnectionString))
            //{
            //    context.Open();

            //    var command = new SqlCommand("DELETE FROM invoice WHERE id = @id", context);

            //    command.Parameters.AddWithValue("@id", id);

            //    command.ExecuteNonQuery();

            //}


            using (var context = _unitOfWork.Create())
            {
                context.Repositories.InvoiceRepository.Remove(id);
                context.SaveChanges();
            }
        }
        private void SetProduct(InvoiceDetail detail, SqlConnection context)
        {


            var command = new SqlCommand("SELECT * FROM product WHERE id = @productId", context);
            command.Parameters.AddWithValue("@productId", detail.ProductId);


            using (var reader = command.ExecuteReader())
            {

                reader.Read();

                detail.Product = new Product
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["name"].ToString(),
                    Price = Convert.ToDecimal(reader["precio"])
                };
            }
        }
        private void PrepareOrder(Invoice model)
        {
            foreach (var detail in model.Detail)
            {
                detail.Total = detail.Quantity * detail.Price;
                detail.Iva = detail.Total * Parameters.IvaRate;
                detail.SubTotal = detail.Total - detail.Iva;
            }

            model.Total = model.Detail.Sum(x => x.Total);
            model.SubTotal = model.Detail.Sum(x => x.SubTotal);
            model.Iva = model.Detail.Sum(x => x.Iva);
        }
        private void UpdateHeader(Invoice model, SqlConnection context)
        {
            var query = "update invoice set clientId = @clientId, iva = @iva, subTotal = @subTotal, total = @total WHERE id = @id";
            var command = new SqlCommand(query, context);

            command.Parameters.AddWithValue("@clientId", model.ProductId);
            command.Parameters.AddWithValue("@iva", model.Iva);
            command.Parameters.AddWithValue("@subTotal", model.SubTotal);
            command.Parameters.AddWithValue("@total", model.Total);
            command.Parameters.AddWithValue("@id", model.Id);

            command.ExecuteNonQuery();

        } // viejo
        private void RemoveDetail(int invoiceId, SqlConnection context)
        {
            var query = "delete from invoicesDetail where invoiceId = @invoiceId";

            var command = new SqlCommand(query, context);

            command.Parameters.AddWithValue("@invoiceId", invoiceId);

            command.ExecuteNonQuery();
        } // viejo
        private void AddHeader(Invoice model, SqlConnection context)
        {
            var query = " insert into invoice (clientId, iva, subTotal, total) output INSERTED.ID values(@clientId, @iva, @subTotal, @total)";
            var command = new SqlCommand(query, context);

            command.Parameters.AddWithValue("@clientId", model.ProductId);
            command.Parameters.AddWithValue("@iva", model.Iva);
            command.Parameters.AddWithValue("@subTotal", model.SubTotal);
            command.Parameters.AddWithValue("@total", model.Total);

            model.Id = Convert.ToInt32(command.ExecuteScalar());

        } // viejo
        private void AddDetail(Invoice model, SqlConnection context)
        {

            foreach (var detail in model.Detail)
            {
                var query = " insert into invoicesDetail (productId, invoiceId, quantity, precio, iva, subTotal, total) values(@productId, @invoiceId, @quantity, @precio, @iva, @subTotal, @total)";
                var command = new SqlCommand(query, context);

                command.Parameters.AddWithValue("@productId", detail.ProductId);
                command.Parameters.AddWithValue("@invoiceId", model.Id);
                command.Parameters.AddWithValue("@quantity", detail.Quantity);
                command.Parameters.AddWithValue("@precio", detail.Price);
                command.Parameters.AddWithValue("@iva", detail.Iva);
                command.Parameters.AddWithValue("@subTotal", detail.SubTotal);
                command.Parameters.AddWithValue("@total", detail.Total);

                command.ExecuteNonQuery();
            }



        } // viejo

        private void SetClient(Invoice invoice, SqlConnection context)
        {


            var command = new SqlCommand("SELECT * FROM client WHERE id = @clientId", context);
            command.Parameters.AddWithValue("@clientId", invoice.ProductId);


            using (var reader = command.ExecuteReader())
            {

                reader.Read();

                invoice.Client = new Client
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["name"].ToString()
                };
            }
        } // viejo
        private void SetDetail(Invoice invoice, SqlConnection context)
        {


            var command = new SqlCommand("SELECT * FROM invoicesDetail WHERE invoiceId = @invoiceId", context);
            command.Parameters.AddWithValue("@invoiceId", invoice.Id);


            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    //invoice.Detail.Add(new InvoiceDetail
                    //{
                    //    Id = Convert.ToInt32(reader["id"]),
                    //    ProductId = Convert.ToInt32(reader["productId"]),
                    //    Quantity = Convert.ToInt32(reader["quantity"]),
                    //    Iva = Convert.ToDecimal(reader["iva"]),
                    //    SubTotal = Convert.ToDecimal(reader["subTotal"]),
                    //    Total = Convert.ToDecimal(reader["total"]),
                    //    Invoice = invoice

                    //});
                }

            }
            foreach (var item in invoice.Detail)
            {
                //Product
                SetProduct(item, context);
            }
        } // viejo

    }
}


 