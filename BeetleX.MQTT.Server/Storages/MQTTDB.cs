using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeetleX.MQTT.Server.Storages
{
    public class MQTTDB : DbContext
    {
        public DbSet<User> User { get; set; }

        public DbSet<Options> Options { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=mqttdb.db;");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<MQTTDriver>()
            //    .Property(e => e.DataType)
            //    .HasConversion(
            //    v => v.ToString(),
            //    v => (DataType)Enum.Parse(typeof(DataType), v)
            //);
        }
    }

    public class User
    {
        [Key]
        public string Name { get; set; }

        public string PWD { get; set; }

        public string Category { get; set; }

        public bool IsNodeClient { get; set; }

        public bool Enabled { get; set; }

        public string Remark { get; set; }
    }
}
