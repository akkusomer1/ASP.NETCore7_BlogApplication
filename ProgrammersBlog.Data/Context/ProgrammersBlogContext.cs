using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProgrammersBlog.Core.Concrete.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Data.Context
{
    //Tamamıyla hepsini vermiş olduk.
    //Eğer projelerimizde özelleştirme gerekmiyorsa User,Role,int tanımlamamızda yeterli olabilir.
    public class ProgrammersBlogContext:IdentityDbContext<User,Role,int,UserClaim,UserRole, UserLogin,RoleClaim,UserToken>
    {
        public ProgrammersBlogContext(DbContextOptions<ProgrammersBlogContext> options):base(options)
        {
        }

 
        public DbSet<Category> Categories { get; set; }
 
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Log> Logs { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
