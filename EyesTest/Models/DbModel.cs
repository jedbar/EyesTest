using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace EyesTest.Models
{
    public class DbModel
    {
        protected IDbConnection DbConnection = null;
        private readonly string connectionString;


        /// <summary>
        /// Default controller, sets the connection string used for database access
        /// </summary>
        public DbModel()
        {
            connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
        }

        /// <summary>
        /// Method inserts data into the database
        /// </summary>
        /// <param name="sql">sql insert query</param>
        /// <param name="parameters">parameters used to bind the sql query</param>
        /// <returns>last inserted row id</returns>
        protected int Insert(string sql, DynamicParameters parameters = null)
        {
            using (DbConnection = new SQLiteConnection(connectionString))
            {
                sql += "; SELECT last_insert_rowid();";

                if (parameters == null)
                {
                    parameters = new DynamicParameters();
                }

                return DbConnection.QuerySingle<int>(sql, parameters);
            }
        }

        /// <summary>
        /// Method executes an sql query 
        /// </summary>
        /// <param name="sql">sql query</param>
        /// <param name="parameters">parameters used to bind the sql query</param>
        protected void Execute(string sql, DynamicParameters parameters = null)
        {
            using (DbConnection = new SQLiteConnection(connectionString))
            {
                if (parameters == null)
                {
                    parameters = new DynamicParameters();
                }

                DbConnection.Execute(sql, parameters);
            }
        }

        /// <summary>
        /// Method queries the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql query</param>
        /// <param name="parameters">parameters used to bind the sql query</param>
        /// <param name="limit">results limit number</param>
        /// <param name="offset">results offset number</param>
        /// <param name="order">results order descending or ascending</param>
        /// <returns></returns>
        protected IEnumerable<T> Query<T>(string sql, DynamicParameters parameters = null, int limit = 0, int offset = 0, string order = null)
        {
            using (DbConnection = new SQLiteConnection(connectionString))
            {
                if (parameters == null)
                {
                    parameters = new DynamicParameters();
                }

                if (limit > 0)
                {
                    sql += " LIMIT @Limit OFFSET @Offset";
                    parameters.Add("@Limit", limit);
                    parameters.Add("@Offset", offset);
                }

                if (order != null)
                {
                    sql += " ORDER BY @Order";
                    parameters.Add("@Order", order);
                }

                return DbConnection.Query<T>(sql, parameters);
            }
        }

        /// <summary>
        /// Method queries the database with a join table statement
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">sql query</param>
        /// <param name="parameters">parameters used to bind the sql query</param>
        /// <param name="limit">results limit number</param>
        /// <param name="offset">results offset number</param>
        /// <param name="order">results order descending or ascending</param>
        /// <returns></returns>
        protected IEnumerable<T> Query<T,T2>(string sql, DynamicParameters parameters = null, int limit = 0, int offset = 0, string order = null)
        {
            using (DbConnection = new SQLiteConnection(connectionString))
            {
                if (parameters == null)
                {
                    parameters = new DynamicParameters();
                }

                if (limit > 0)
                {
                    sql += " LIMIT @Limit OFFSET @Offset";
                    parameters.Add("@Limit", limit);
                    parameters.Add("@Offset", offset);
                }

                if (order != null)
                {
                    sql += " ORDER BY @Order";
                    parameters.Add("@Order", order);
                }

                return DbConnection.Query<T, T2, T>(sql, (t, t2) =>
                {
                    var properties = t.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    foreach (var prop in properties)
                    {
                        if (prop.PropertyType == typeof(T2))
                        {
                            prop.SetValue(t, t2);
                        }
                    }
                    return t;
                }, parameters, splitOn: "ID").ToList();
            }
        }

    }

}
