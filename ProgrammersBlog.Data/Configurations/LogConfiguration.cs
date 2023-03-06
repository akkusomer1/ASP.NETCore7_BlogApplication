using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgrammersBlog.Core.Concrete.Entities;

namespace ProgrammersBlog.Data.Configurations
{

    public class LogConfiguration:IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.MachineName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Logged).IsRequired();

            builder.Property(x => x.Level).IsRequired().HasMaxLength(50);

            builder.Property(x => x.Message).IsRequired().HasColumnType("NVARCHAR(MAX)");

            builder.Property(x => x.Logger).IsRequired(false).HasMaxLength(250);

            builder.Property(x => x.Callsite).IsRequired(false).HasColumnType("NVARCHAR(MAX)");

            builder.Property(x => x.Exception).IsRequired(false).HasColumnType("NVARCHAR(MAX)");

            builder.ToTable("Logs");

        }
    }
}
