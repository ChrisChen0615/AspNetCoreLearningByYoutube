using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreLearningByYoutube.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>//DbContext
    {
        /// <summary>
        /// to pass configuration information to the DbContext use DbContextOptions instance
        /// (configuration:connection string,database provider)
        /// DbContextOptions<AppDbContext>:泛型，將資料應用至泛型的型別
        /// 
        /// The DbContext class includes a DbSet<TEntity> property for each entity in the model
        /// </summary>
        /// <param name="options">連線字串等相關設定資料</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        public DbSet<Employee> Employees { get; set; }

        /// <summary>
        /// 初始seed資料
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //建立seed方式有兩種
            //1.直接寫在model creating內
            //modelBuilder.Entity<Employee>().HasData(
            //    new Employee
            //    {
            //        Id = 1,
            //        Name = "Joy",
            //        Email = "joy@sanrong.com.tw",
            //        Department = Dept.IT
            //    },
            //    new Employee
            //    {
            //        Id = 2,
            //        Name = "Milly",
            //        Email = "milly@sanrong.com.tw",
            //        Department = Dept.HR
            //    }
            //    );

            //2.為了版面2，將seed資料搬移至其他地方，寫ModelBuilder extension
            //IdentityDbContext，需先create identityuser,identityrole資料表
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();
        }
    }
}
