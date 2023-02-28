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

            string result = RemoveTown(dbContext);
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


        //7.	Employees and Projects
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var emploeeWithProject = context.Employees
                //.Where(e => e.EmployeesProjects
                //.Any(ep => ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003))
                .Take(10)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    ManagerFirstName = e.Manager.FirstName,
                    ManagerLastName = e.Manager.LastName,
                    Projects = e.EmployeesProjects.Where(ep => ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003)
                    .Select(ep => new
                    {
                        ProjectName = ep.Project.Name,
                        StartDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt"),
                        EndDate = ep.Project.EndDate.HasValue ? ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt") : "not finished"
                    }).ToArray()
                }).ToArray();

            foreach (var e in emploeeWithProject)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");
                foreach (var p in e.Projects)
                {
                    sb.AppendLine($"--{p.ProjectName} - {p.StartDate} - {p.EndDate}");
                }
            }
            return sb.ToString().TrimEnd();   
        }



        //8.	Addresses by Town
        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var allAddresses = context
                .Addresses
                .Select(a => new
                {
                    a.AddressText,
                    AddressTown = a.Town.Name,
                    CountEmployees = a.Employees.Count()
                })
                .OrderByDescending(a => a.CountEmployees)
                .ThenBy(a => a.AddressTown)
                .ThenBy(a => a.AddressText)
                .Take(10)
                .ToArray();

            foreach (var a in allAddresses)
            {
                output.AppendLine($"{a.AddressText}, {a.AddressTown} - {a.CountEmployees} employees");
            }

            return output.ToString().TrimEnd();
        }


        //9.	Employee 147
        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e => new
                {   
                       e.FirstName,
                       e.LastName,
                       e.JobTitle,
                       Projects = e.EmployeesProjects.Select(ep => new { ep.Project.Name })
                    .OrderBy(ep => ep.Name)
                    .ToArray()
                }).ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                foreach (var p in e.Projects)
                {
                    sb.AppendLine(p.Name);
                }
            }
            return sb.ToString().TrimEnd();
        }


        //10.	Departments with More Than 5 Employees
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var departmentsWithMoreThan5Em = context.Departments
                .Where(d=>d.Employees.Count>5)
                .Select(d => new
                {
                    DepartmentName = d.Name, 
                    ManagerFirstName = d.Manager.FirstName,
                    ManagerLastName = d.Manager.LastName,
                    CountEmployees = d.Employees.Count(),
                    Employees =d.Employees.Select(e => new
                    {
                        EmployeeFirstName = e.FirstName,
                        EmployeeLastName = e.LastName,
                        JobTitle = e.JobTitle
                    }).OrderBy(e=>e.EmployeeFirstName)
                      .ThenBy(e=>e.EmployeeLastName)
                      .ToArray()
                }).OrderBy(d=>d.CountEmployees)
                  .ThenBy(d=>d.DepartmentName)
                  .ToArray();


                  foreach (var d in departmentsWithMoreThan5Em)
            {
                sb.AppendLine($"{d.DepartmentName} – {d.ManagerFirstName} {d.ManagerLastName}");
                foreach (var e in d.Employees)
                {
                    sb.AppendLine($" {e.EmployeeFirstName} {e.EmployeeLastName} - {e.JobTitle} ");
                }
            }
            return sb.ToString().TrimEnd();
        }


        //11.	Find Latest 10 Projects
        public static string GetLatestProjects(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var latestProjects = context.Projects
                
                .OrderByDescending(p => p.StartDate)
                .Select(p => new
                {  
                    p.Name,
                    p.Description,
                    StartDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt")
                })
                .Take(10)
                .ToArray()
                .OrderBy(p => p.Name);


            foreach (var item in latestProjects)
            {
                sb.AppendLine(item.Name);
                sb.AppendLine(item.Description);
                sb.AppendLine(item.StartDate);
            }
            return sb.ToString().TrimEnd();
        }


        //12.	Increase Salaries
        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var employeesForSalaryIncrease = context
                .Employees
                .Where(employee => employee.Department.Name == "Engineering" ||
                                   employee.Department.Name == "Tool Design" ||
                                   employee.Department.Name == "Marketing" ||
                                   employee.Department.Name == "Information Services")
                .OrderBy(employee => employee.FirstName)
                .ThenBy(employee => employee.LastName)
                .ToList();

            foreach (var em in employeesForSalaryIncrease)
            {
                em.Salary += em.Salary * 0.12m;
            }

            context.SaveChanges();


            foreach (var em in employeesForSalaryIncrease)
            {
                output.AppendLine($"{em.FirstName} {em.LastName} (${em.Salary:F2})");
            }

            return output.ToString().TrimEnd();
        }


        //13.	Find Employees by First Name Starting with "Sa"
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var employees = context
                .Employees
                .Where(e => e.FirstName.StartsWith("Sa"))
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToArray();

            foreach (var e in employees)
            {
                output.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})");
            }

            return output.ToString().TrimEnd();
        }


        //14.	Delete Project by Id
        public static string DeleteProjectById(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            Project projectToDelete = context.Projects.Find(2)!;

            var referencedProjects = context
                .EmployeesProjects
                .Where(ep => ep.ProjectId == projectToDelete.ProjectId)
                .ToArray();

            context.EmployeesProjects.RemoveRange(referencedProjects);
            context.Projects.Remove(projectToDelete);
            context.SaveChanges();

            string[] projectNames = context
                .Projects
                .Take(10)
                .Select(p => p.Name)
                .ToArray();

            foreach (var p in projectNames)
            {
                output.AppendLine(p);
            }

            return output.ToString().TrimEnd();
        }


        //15.	Remove Town
        public static string RemoveTown(SoftUniContext context)
        {
            Town townToDelete = context
                .Towns
                .FirstOrDefault(t => t.Name == "Seattle")!;

            Address[] referencedAddresses = context
                .Addresses
                .Where(a => a.TownId == townToDelete.TownId)
                .ToArray();

            foreach (var e in context.Employees)
            {
                if (referencedAddresses.Any(a => a.AddressId == e.AddressId))
                {
                    e.AddressId = null;
                }
            }

            int numberOfDeletedAddresses = referencedAddresses.Length;

            context.Addresses.RemoveRange(referencedAddresses);
            context.Towns.Remove(townToDelete);

            context.SaveChanges();

            return $"{numberOfDeletedAddresses} addresses in Seattle were deleted";
        }
    }
}