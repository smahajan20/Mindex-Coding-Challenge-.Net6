using CodeChallenge.Models;
using CodeChallenge.Repositories;
using System;

namespace CodeChallenge.Services
{
    public class CompensationService : ICompensationService
    {
        public readonly ICompensationRepository _compensationRepository;
        public readonly IEmployeeService _employeeService;

        public CompensationService(ICompensationRepository compensationRepository, IEmployeeService employeeService) 
        {
            _compensationRepository = compensationRepository;
            _employeeService = employeeService;
        }

        public Compensation Create(Compensation compensation)
        {
            if(compensation != null)
            {
                _compensationRepository.Add(compensation);
                _compensationRepository.SaveAsync().Wait();
            }
            return compensation;
        }

        public Compensation GetByEmployeeId(string employeeId)
        {
            if(!String.IsNullOrEmpty(employeeId))
            {
                return _compensationRepository.GetByEmployeeId(employeeId);
            }
            return null;
        }
    }
}
