using CodeChallenge.Models;
using CodeChallenge.Repositories;
using System;
using System.Collections.Generic;

namespace CodeChallenge.Services
{
    public class ReportingStructureService : IReportingStructureService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private int _numberOfReports;
        
        public ReportingStructureService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public ReportingStructure GetNumberOfReports(string employeeId)
        {
            if(!String.IsNullOrEmpty(employeeId))
            {
                Employee employee = _employeeRepository.GetById(employeeId);
                _numberOfReports = 0;
                if(employee != null)
                {
                    helperMethodToCalculateNumberOfReports(employee);
                }
                
                return new ReportingStructure(employee, _numberOfReports);
            }

            return null;
        }

        /// <summary>
        /// This helper method is used to apply DFS algorithm to calculate the total number of reports under a given employee
        /// </summary>
        /// <param name="employee"></param>
        public void helperMethodToCalculateNumberOfReports(Employee employee)
        {
            List<Employee> directReports = employee.DirectReports;
            if(directReports == null)
            {
                return;
            }

            _numberOfReports += directReports.Count;

            foreach(Employee emp in directReports)
            {
                helperMethodToCalculateNumberOfReports(emp);
            }

        }
    }
}
