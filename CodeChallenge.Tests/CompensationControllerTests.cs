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
        public void CreateCompensation_Returns_CreatedNewCompensation()
        {
            // Arrange
            string employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";

            var compensation = new Compensation()
            {
                EmployeeId = employeeId,
                Salary = 90000,
                EffectiveDate = new DateTime(2023, 09, 04)
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;
            var newCompensation = response.DeserializeContent<Compensation>();

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsNotNull(newCompensation.EmployeeId);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
        }

         [TestMethod]
        public void CreateCompensation_Returns_UpdatedExistingCompensation()
        {
            // Arrange
            string employeeId = "62c1084e-6e34-4630-93fd-9153afb65309"; //This id is present in the CompensationSeedData.json file 

            var compensation = new Compensation()
            {
                EmployeeId = employeeId,
                Salary = 80000,
                EffectiveDate = new DateTime(2024, 01, 02)
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;
            var newCompensation = response.DeserializeContent<Compensation>();

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsNotNull(newCompensation.EmployeeId);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
        }

        [TestMethod]
        public void CreateCompensation_Returns_EmployeeIdIsNull()
        {
            // Arrange
            var compensation = new Compensation()
            {
                Salary = 90000,
                EffectiveDate = new DateTime(2023, 09, 04)
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.AreEqual($"Employee Id is null", responseContent);
        }

        [TestMethod]
        public void CreateCompensation_Throws_EmployeeNotFoundException()
        {
            // Arrange
            var compensation = new Compensation()
            {
                EmployeeId = "Invalid_Id",
                Salary = 90000,
                EffectiveDate = new DateTime(2023, 09, 04)
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.IsTrue(responseContent.Contains("No employee found with ID"));
            Assert.IsTrue(responseContent.Contains("KeyNotFoundException"));
        }


        [TestMethod]
        public void GetCompensationByEmployeeId_Returns_Ok()
        {
            // Arrange
            var employeeId = "c0c2293d-16bd-4603-8e08-638a9d18b22c";
            var expectedSalary = 80000;
            var expectedEffectiveDate = new DateTime(2022, 03, 04);

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var response = getRequestTask.Result;
            var compensation = response.DeserializeContent<Compensation>();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(expectedSalary, compensation.Salary);
            Assert.AreEqual(expectedEffectiveDate, compensation.EffectiveDate);
            Assert.AreEqual(employeeId, compensation.EmployeeId);
        }

        [TestMethod]
        public void GetCompensationByEmployeeId_Returns_CompensationForEmployeeNotFound()
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
