using FaxServer.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestFaxcom
{
    [TestClass]
    public class TestParameters
    {
        [TestMethod]
        public void TestUser()
        {
            var user = new User {Password = "test!"};
            Assert.IsFalse(user.Validate().IsValid);
            Assert.IsTrue(user.LastError == "Username is missing.");
            user.Username = "Shoemaker";
            Assert.IsTrue(user.Validate().IsValid);
            Assert.IsTrue(string.IsNullOrEmpty(user.LastError));
            user.Username = null;
            Assert.IsFalse(user.Validate().IsValid);
            Assert.IsTrue(user.LastError == "Username is missing.");
        }

        [TestMethod]
        public void TestSender()
        {
            var sendr = new Sender(); 
            Assert.IsFalse(sendr.Validate().IsValid);
            sendr.Name = "Shoemaker";
            Assert.IsFalse(sendr.Validate().IsValid);
            sendr.Company = "SedgwickCMS";
            Assert.IsFalse(sendr.Validate().IsValid);
            sendr.FaxNumber = "6178554454";
            Assert.IsTrue(sendr.Validate().IsValid);
            Assert.IsTrue(sendr.LastError == string.Empty);
        }

        [TestMethod]
        public void TestQueue()
        {
            var queue = new FaxQueue() {Queue = @"\\ltr1fx03\FaxcomQ_SMTPOPMEDIFaxQ" };
            Assert.IsFalse(queue.Validate().IsValid);
            queue.Username = "Administrator";
            Assert.IsTrue(queue.Validate().IsValid);
            Assert.IsTrue(queue.LastError == string.Empty);
        }
    }
}
