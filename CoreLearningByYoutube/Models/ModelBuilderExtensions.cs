using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreLearningByYoutube.Models
{
    /// <summary>
    /// 預設資料，程式撰寫處
    /// </summary>
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    Name = "Joy",
                    Email = "joy@sanrong.com.tw",
                    Department = Dept.IT
                },
                new Employee
                {
                    Id = 2,
                    Name = "Milly",
                    Email = "milly@sanrong.com.tw",
                    Department = Dept.HR
                }
                );
        }
    }
}
