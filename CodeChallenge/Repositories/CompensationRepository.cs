using CodeChallenge.Data;
using CodeChallenge.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CodeChallenge.Repositories
{
    public class CompensationRepository : ICompensationRepository
    {
        public readonly EmployeeContext _employeeContext;

        public CompensationRepository(EmployeeContext employeeContext) 
        {
             _employeeContext = employeeContext;
        }


        /// <summary>
        /// I have assumed that there will be one compensation per employee
        /// So this is kind of an upsert operation
        /// </summary>
        /// <param name="compensation"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public Compensation AddOrUpdate(Compensation compensation)
        {
            if(!_employeeContext.Employees.Any(e => e.EmployeeId == compensation.EmployeeId))
            {
                throw new KeyNotFoundException($"No employee found with ID {compensation.EmployeeId}.");
            }

            var existingCompensation = _employeeContext.Compensations.FirstOrDefault(c => c.EmployeeId == compensation.EmployeeId);

            if (existingCompensation != null)
            {
                existingCompensation.Salary = compensation.Salary;
                existingCompensation.EffectiveDate = compensation.EffectiveDate;
            }
            else
            {
                compensation.CompensationId = Guid.NewGuid().ToString();
                _employeeContext.Compensations.Add(compensation);
                existingCompensation = compensation;
            }

            return existingCompensation;
        }

        public Compensation GetByEmployeeId(String employeeId)
        {
            return _employeeContext.Compensations.Include(c => c.Employee).FirstOrDefault(c => c.EmployeeId == employeeId);
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }
    }
}
