using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Objects;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Extentions.Objects;
using EzDbSchema.Core.Extentions.Strings;
using EzDbSchema.Core.Enums;
using System.Text;

namespace EzDbSchema.MsSql
{
	public class ConnectionParameters : IConnectionParameters
    {
        /// <summary>
        /// Test the connection string to determine if it is valid and we can connect to the data source 
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            try
            {
                var Errors = new List<string>();
                if (string.IsNullOrEmpty(Database)) Errors.Add("Database is missing or Null.");
                if (string.IsNullOrEmpty(Server)) Errors.Add("Server is missing or Null.");
                if (!Trusted)
                {
                    if (string.IsNullOrEmpty(UserName)) Errors.Add("UserName is missing or Null.");
                    if (string.IsNullOrEmpty(Password)) Errors.Add("Password is missing or Null.");
                }
                if (Errors.Count>0) throw new Exception("Database Connection Error. " + string.Join(" ", Errors.ToList()));
                using (SqlConnection connection = new SqlConnection(this.ConnectionString))
                {
                    if (connection.State == ConnectionState.Closed) connection.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT 1", connection))
                    {
                        cmd.CommandTimeout = 300;
                        cmd.CommandType = CommandType.Text;
                        var rs = cmd.ExecuteReader();
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private SqlConnectionStringBuilder _builder = new SqlConnectionStringBuilder();
        public string Database { get=> _builder.DataSource; set => _builder.DataSource=value; }
        public string Server { get => _builder.InitialCatalog; set => _builder.InitialCatalog = value; }
        /// <summary>
        /// If you pass a UserName of "TRUSTED" and a password of NULL, then this will assume a Trusted=true
        /// </summary>
        public string UserName { get => _builder.UserID; set => _builder.UserID = value; }
        public string Password { get => _builder.Password; set => _builder.Password = value; }
        public bool Trusted { get => _builder.IntegratedSecurity; set => _builder.IntegratedSecurity = value; }
        public string ConnectionString { get => _builder.ConnectionString; set => _builder.ConnectionString = value; }
    }
}
