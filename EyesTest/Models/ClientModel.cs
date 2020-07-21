using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Automation.Peers;

namespace EyesTest.Models
{
    public class ClientModel : DbModel
    {
		public int ID { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Pesel { get; set; }
		public string Street { get; set; }
		public string City { get; set; }
		public string ZipCode { get; set; }
		public string PhoneNumber { get; set; }


        /// <summary>
        /// Method validates required attributes
        /// </summary>
        /// <returns>true on success or throws an argument exception</returns>
        public bool Validate()
        {
            if (!Regex.IsMatch(FirstName, "^[A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ]+$"))
            {
                throw new ArgumentException("Imię musi składać się wyłącznie z liter");
            }

            if (!Regex.IsMatch(LastName, "^[A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ]+$"))
            {
                throw new ArgumentException("Nazwisko musi składać się wyłącznie z liter.");
            }

            if (!Regex.IsMatch(Pesel, "^[0-9]{11}$"))
            {
                throw new ArgumentException("Pesel musi zawierać 11 cyfr.");
            }
            return true;
        }

        /// <summary>
        /// Method gets patients data by it's id number
        /// </summary>
        /// <param name="id">patients id number</param>
        /// <returns>patient object</returns>
        public ClientModel GetById(int id)
        {
            string sql = "SELECT * FROM Clients WHERE ID = @Id";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            return Query<ClientModel>(sql, parameters).FirstOrDefault();
        }

        /// <summary>
        /// Method searches for patients based on their id
        /// </summary>
        /// <param name="id">patients id number</param>
        /// <returns>list of patient objects</returns>
        public List<ClientModel> FindById(string id)
        {
            string sql = "SELECT * FROM Clients WHERE ID = @Id";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            return Query<ClientModel>(sql, parameters).ToList();
        }

        /// <summary>
        /// Method searches for patients based on their last name
        /// </summary>
        /// <param name="lastName">patients last name</param>
        /// <returns>list of patient objects</returns>
        public List<ClientModel> FindByLastName(string lastName)
        {
            string sql = "SELECT * FROM Clients WHERE LastName LIKE @LastName";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@LastName", lastName + "%");

            return Query<ClientModel>(sql, parameters).ToList();
        }

        /// <summary>
        /// Method searches for patients based on their pesel number
        /// </summary>
        /// <param name="pesel">patients pesel number</param>
        /// <returns>list of patient objects</returns>
        public List<ClientModel> FindByPesel(string pesel)
        {
            string sql = "SELECT * FROM Clients WHERE Pesel LIKE @Pesel";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Pesel", pesel + "%");

            return Query<ClientModel>(sql, parameters).ToList();
        }


        /// <summary>
        /// Method adds a new patient to the database
        /// </summary>
        /// <param name="client"></param>
        /// <returns>last inserted row id</returns>
        public int Add(ClientModel client)
        {
            string sql = "INSERT INTO Clients(FirstName,LastName,Pesel,Street,City,ZipCode,PhoneNumber) VALUES (@FirstName,@LastName,@Pesel,@Street,@City,@ZipCode,@PhoneNumber)";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@FirstName", client.FirstName);
            parameters.Add("@LastName", client.LastName);
            parameters.Add("@Pesel", client.Pesel);
            parameters.Add("@Street", client.Street);
            parameters.Add("@City", client.City);
            parameters.Add("@ZipCode", client.ZipCode);
            parameters.Add("@PhoneNumber", client.PhoneNumber);

            return Insert(sql, parameters);
        }

        /// <summary>
        /// Method updates the database with new patient data
        /// </summary>
        public void Update()
        {
            string sql = "UPDATE Clients SET FirstName = @FirstName ,LastName = @LastName, Pesel = @Pesel, Street = @Street, City = @City, ZipCode = @ZipCode, PhoneNumber = @PhoneNumber WHERE ID = @Id";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", ID);
            parameters.Add("@FirstName", FirstName);
            parameters.Add("@LastName", LastName);
            parameters.Add("@Pesel", Pesel);
            parameters.Add("@Street", Street);
            parameters.Add("@City", City);
            parameters.Add("@ZipCode", ZipCode);
            parameters.Add("@PhoneNumber", PhoneNumber);

            Execute(sql, parameters);
        }


        /// <summary>
        /// Method deletes a patient form database based on their id
        /// </summary>
        public void Delete()
        {
            string sql = "DELETE FROM Clients WHERE ID = @Id";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", ID);

            Execute(sql, parameters);
        }

    }

}