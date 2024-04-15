﻿using CodeChallenge.Data;
using CodeChallenge.Models;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace CodeChallenge.Repositories
{
    public class CompensationRepository : ICompensationRepository
    {
        public readonly CompensationContext _compensationContext;

        public CompensationRepository(CompensationContext compensationContext) 
        {
             _compensationContext = compensationContext;
        }

        public Compensation Add(Compensation compensation)
        {
            compensation.CompensationId = Guid.NewGuid().ToString();
            _compensationContext.Compensations.Add(compensation);
            return compensation;
        }

        public Compensation GetByEmployeeId(String employeeId)
        {
            return _compensationContext.Compensations.SingleOrDefault(c => c.Employee.EmployeeId == employeeId);
        }

        public Task SaveAsync()
        {
            return _compensationContext.SaveChangesAsync();
        }
    }
}
