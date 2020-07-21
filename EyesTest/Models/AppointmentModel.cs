using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EyesTest.Models
{
    public class AppointmentModel : DbModel
    {
        public int ID { get; set; }
        public int ClientID { get; set; }
        public DateTime Timestamp { get; set; }
        public ClientModel Client {get; set; }

        /// <summary>
        /// Method searches for appointments based on date
        /// </summary>
        /// <param name="date">appointment date in format yyyy-MM-dd</param>
        /// <returns>list of appointments objects</returns>
        public List<AppointmentModel> FindAppointments(string date)
        {
            string sql = "SELECT * FROM Appointments a JOIN Clients c ON a.ClientID = c.ID WHERE Timestamp LIKE @Timestamp ORDER BY Timestamp ASC";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Timestamp", date + "%");

            return Query<AppointmentModel,ClientModel>(sql, parameters).ToList();
        }

        /// <summary>
        /// Method deletes an appointment form database based on it's id
        /// </summary>
        public void Delete()
        {
            string sql = "DELETE FROM Appointments WHERE ID = @Id";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", ID);

            Execute(sql, parameters);
        }

        /// <summary>
        /// Method adds a new appointment to the database
        /// </summary>
        /// <returns>appointments id</returns>
        public int Add()
        {
            string sql = "INSERT INTO Appointments(ClientID,Timestamp) VALUES (@ClientId,@Timestamp)";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ClientID", ClientID);
            parameters.Add("@Timestamp", Timestamp);

            return Insert(sql, parameters);
        }

    }
}