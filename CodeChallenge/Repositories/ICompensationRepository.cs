using CodeChallenge.Models;
using System;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories
{
    public interface ICompensationRepository
    {
        Compensation GetByEmployeeId(String employeeId);
        Compensation Add(Compensation compensation);
        Task SaveAsync();
    }
}
