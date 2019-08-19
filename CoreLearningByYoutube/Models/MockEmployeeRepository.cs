using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreLearningByYoutube.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {
        private List<Employee> _employeeList;

        public MockEmployeeRepository()
        {
            _employeeList = new List<Employee>
            {
                new Employee{ Id=1,Name="Mary",Department=Dept.HR,Email="mary@gmail.com"},
                new Employee{ Id=2,Name="John",Department=Dept.IT,Email="john@gmail.com"},
                new Employee{ Id=3,Name="Tom",Department=Dept.Payroll,Email="tom@gmail.com"}
            };
        }

        public Employee Add(Employee employee)
        {
            employee.Id = _employeeList.Max(e => e.Id) + 1;
            _employeeList.Add(employee);
            return employee;
        }

        public Employee Delete(int id)
        {
            var employee = _employeeList.FirstOrDefault(e => e.Id == id);
            if(employee != null)
            {
                _employeeList.Remove(employee);
            }
            return employee;
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return _employeeList;
        }

        public Employee GetEmployee(int id)
        {
            return _employeeList.FirstOrDefault(e => e.Id == id);
        }

        public Employee Update(Employee employeeChange)
        {
            var employee = _employeeList.FirstOrDefault(e => e.Id == employeeChange.Id);
            if (employee != null)
            {
                employee.Name = employeeChange.Name;
                employee.Email = employeeChange.Email;
                employee.Department = employeeChange.Department;
            }
            return employee;
        }
    }
}
