using SoftUni.Data;
using SoftUni.Models;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext dbContext = new SoftUniContext();

            string result = AddNewAddressToEmployee(dbContext);
            Console.WriteLine(result);
        }


        //3.	Employees Full Information
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var allEmployees = context
                .Employees
                .OrderBy(e => e.EmployeeId)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.Salary

                })
                .ToList();

            foreach (var employee in allEmployees)
            {
                output.AppendLine(
                    $"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");
            }

            return output.ToString().TrimEnd();
        }


        //4.	Employees with Salary Over 50 000
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var highSalaryEmployees = context
                .Employees
                .Where(e => e.Salary > 50000)
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ToArray();

            foreach (var e in highSalaryEmployees)
            {
                output.AppendLine($"{e.FirstName} - {e.Salary:f2}");
            }

            return output.ToString().TrimEnd();
        }


        //5.	Employees from Research and Development
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var emploeeRND = context.Employees
               .Where(e => e.Department.Name == "Research and Development").Select(
                e => new
                {
                    e.FirstName,
                    e.LastName,
                    departmentName = e.Department.Name,
                    e.Salary
                }).OrderBy(e => e.Salary).ThenByDescending(e => e.FirstName).ToArray();

            foreach (var item in emploeeRND)
            {
                output.AppendLine($"{item.FirstName} {item.LastName} from {item.departmentName} - ${item.Salary:f2}");
            }
            return output.ToString().TrimEnd();
        }

        //6.	Adding a New Address and Updating Employee
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address newAddress = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            Employee employee = context.Employees.FirstOrDefault(e => e.LastName == "Nakov");
            employee.Address=newAddress;

            context.SaveChanges();

            var emploeeAddress = context.Employees.OrderByDescending(e => e.AddressId).Take(10)
                .Select(e => e.Address.AddressText).ToArray();

            return String.Join(Environment.NewLine, emploeeAddress);
        }
    }
}