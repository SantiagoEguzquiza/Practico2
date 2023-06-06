using Models;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Repository.SqlServer
{
    public class ClientRepository : Repository, IClientRepository
    {


        public ClientRepository(SqlConnection context, SqlTransaction transaction)
        {
            this._transaction = transaction;
            this._context = context;
        }

        public global::Models.Client Get(int id)
        {

            var command = CreateCommand("SELECT * FROM client WITH(NOLOCK) WHERE id = @clientId");
            command.Parameters.AddWithValue("@clientId", id);


            using (var reader = command.ExecuteReader())
            {

                reader.Read();

                return new Client
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["name"].ToString()
                };
            }

        }

        public IEnumerable<Client> GetAll()
        {
            throw new System.NotImplementedException();
        }

    }
}