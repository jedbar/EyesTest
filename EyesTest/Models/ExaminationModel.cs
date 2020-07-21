using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
namespace EyesTest.Models
{
    public class ExaminationModel : DbModel
    {
		public int ID { get; set; }
		public int ClientID { get; set; }
		public string Timestamp { get; set; }
		public string Note { get; set; }
        public List<EyeTestModel> Tests = new List<EyeTestModel>();

        /// <summary>
        /// Method adds as examination into the database
        /// </summary>
        /// <param name="clientId">client's id</param>
        /// <returns>examination id</returns>
        public int Add(int clientId)
        {
            string sql = "INSERT INTO Examinations(ClientID) VALUES (@ClientId)";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ClientId", clientId);

            return Insert(sql, parameters);
        }

        /// <summary>
        /// Method deletes an examination from the database based on it's id
        /// </summary>
        public void Delete()
        {
            string sql = "DELETE FROM Examinations WHERE ID = @Id";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", ID);

            Execute(sql, parameters);
        }

        /// <summary>
        /// Method gets an examinations based on client id
        /// </summary>
        /// <param name="clientId">client id</param>
        /// <returns>list of examination objects</returns>
        public List<ExaminationModel> GetHistory(int clientId)
        {
            string sql = "SELECT * FROM Examinations WHERE ClientID = @ClientId";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ClientId", clientId);

            return Query<ExaminationModel>(sql, parameters).ToList();
        }

        /// <summary>
        /// Method gets eye tests for an examination
        /// </summary>
        /// <returns>list of eye tests objects</returns>
        public List<EyeTestModel> GetTests()
        {
            string sql = "SELECT * FROM EyeTests WHERE ClientID = @ClientId AND ExaminationID = @ExaminationId";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ClientId", ClientID);
            parameters.Add("@ExaminationId", ID);

            return Query<EyeTestModel>(sql, parameters).ToList();
        }
    }
}
