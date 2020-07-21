using Microsoft.VisualStudio.TestTools.UnitTesting;
using EyesTest.Models;
using System.Data.SQLite;

namespace EyesTest.Tests
{
    [TestClass()]
    public class ClientModelTests
    {
        [TestMethod()]
        public void Test1()
        {
            var client = new ClientModel();

            client.FirstName = "Adam";
            client.LastName = "Kowalski";
            client.Pesel = "12345678101";

            bool isValid = client.Validate();
            int id = client.Add(client);

            Assert.AreEqual(true, isValid);
            Assert.IsTrue(id > 0);

        }

        [TestMethod()]
        public void Test2()
        {
            var client = new ClientModel();

            client.FirstName = "Marcin";
            client.LastName = "Duda";
            client.Pesel = "12345678101";

            bool isValid = client.Validate();

            Assert.AreEqual(true, isValid);
            Assert.ThrowsException<SQLiteException>(() => client.Add(client)); //duplicate pesel exception
        }

    }
}