using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main()
        {
            var context = new SoftUniContext();
            Console.WriteLine(GetEmployeesFullInformation(context));
        }
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var sb = new StringBuilder();
            var list = context.Employees.OrderBy(e => e.EmployeeId).Select(x => new
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                MiddleName = x.MiddleName,
                Salary = x.Salary,
                JobTitle = x.JobTitle,
            }).ToList();
            foreach (var employee in list)
            {
                sb.Append($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:F2}");
                sb.Append(Environment.NewLine);
            }
            return sb.ToString().Trim();
        }
    }
}