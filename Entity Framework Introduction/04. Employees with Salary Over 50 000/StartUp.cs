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
            Console.WriteLine(GetEmployeesWithSalaryOver50000(context));
        }
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var sb = new StringBuilder();
            var list = context.Employees.OrderBy(e => e.FirstName).Where(e => e.Salary > 50000).Select(e => new
            {
                firstName = e.FirstName,
                salary = e.Salary
            }).ToList() ;
            foreach (var item in list)
            {
                sb.Append($"{item.firstName} - {item.salary:f2}");
                sb.AppendLine();
            }
            return sb.ToString().Trim();
        }
    }
}