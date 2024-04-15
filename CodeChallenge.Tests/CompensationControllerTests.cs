using System;
using System.Net;
using System.Net.Http;
using System.Text;

using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeChallenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f"
            };
            var compensation = new Compensation()
            {
                Employee = employee,
                Salary = 90000,
                EffectiveDate = new DateTime(2023, 09, 04)
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newCompensation.Employee.EmployeeId);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
        }

        [TestMethod]
        public void CreateCompensation_Returns_InternalServerError()
        {
            // Arrange

            // Compensation for this EmployeeId already exists and hence will throw an internal server error
            var employee = new Employee()
            {
                EmployeeId = "62c1084e-6e34-4630-93fd-9153afb65309"  
            };
            var compensation = new Compensation()
            {
                Employee = employee,
                Salary = 90000,
                EffectiveDate = new DateTime(2023, 09, 04)
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            HttpResponseMessage response = null;
            try
            {
                var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
                response = postRequestTask.Result;
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected exception type thrown");
            }

            // Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);

            var responseContent = response.Content.ReadAsStringAsync().Result;
            Assert.IsTrue(responseContent.Contains("An item with the same key has already been added"));
        }

        [TestMethod]
        public void GetCompensationByEmployeeId_Returns_Ok()
        {
            // Arrange
            var employeeId = "62c1084e-6e34-4630-93fd-9153afb65309";
            var expectedSalary = 90000;
            var expectedEffectiveDate = new DateTime(2023, 09, 04);

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var compensation = response.DeserializeContent<Compensation>();
            Assert.AreEqual(expectedSalary, compensation.Salary);
            Assert.AreEqual(expectedEffectiveDate, compensation.EffectiveDate);
            Assert.AreEqual(employeeId, compensation.Employee.EmployeeId);
        }

        [TestMethod]
        public void GetCompensationByEmployeeId_Returns_CompensationNotFound()
        {
            // Arrange
            var employeeId = "Invalid_Id";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var response = getRequestTask.Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual($"Compensation for the employee with {employeeId} could not be found", responseContent);
        }
    }
}
