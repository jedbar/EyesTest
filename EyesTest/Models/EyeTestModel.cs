using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EyesTest.Models
{
    public class EyeTestModel : DbModel
    {
		public int ID { get; set; }
		public int ClientID { get; set; }
		public int ExaminationID { get; set; }
		public string Vision { get; set; }
		public string Eye { get; set; }
		public float Sph { get; set; }
		public float Cyl { get; set; }
		public int Ax { get; set; }
		public int Pd { get; set; }
		public string Timestamp { get; set; }

		/// <summary>
		/// Method validates the eyetest object
		/// </summary>
		/// <returns>true on success or throws on error</returns>
		public bool Validate()
		{
			if (Sph < -10.0 || Sph > 10.0)
			{
				throw new ArgumentException("Sfera musi być pomiędzy -10 a 10");
			}

			if (Cyl < -10.0 || Cyl > 10.0)
			{
				throw new ArgumentException("Cylinder musi być pomiędzy -10 a 10");
			}

			if (Ax < 0 || Ax > 180)
			{
				throw new ArgumentException("Oś musi być pomiędzy 0 a 180");
			}
			
			if (Pd < 1)
			{
				throw new ArgumentException("Odległość źrenicy musi być większa od 0");
			}
			return true;
		}


		/// <summary>
		/// Method inserts eyetest data into the database
		/// </summary>
		public void Save()
		{
			string sql = "INSERT INTO EyeTests(ClientID,ExaminationID,Vision,Eye,Sph,Cyl,Ax,Pd) VALUES (@ClientId,@ExaminationId,@Vision,@Eye,@Sph,@Cyl,@Ax,@Pd)";

			DynamicParameters parameters = new DynamicParameters();
			parameters.Add("@ClientID", ClientID);
			parameters.Add("@ExaminationID", ExaminationID);
			parameters.Add("@Vision", Vision);
			parameters.Add("@Eye", Eye);
			parameters.Add("@Sph", Sph);
			parameters.Add("@Cyl", Cyl);
			parameters.Add("@Ax", Ax);
			parameters.Add("@Pd", Pd);

			Execute(sql, parameters);
		}


	}
}
