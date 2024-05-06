using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BeetleX.MQTT.Server.Storages
{
    [Table("options")]
    public class Options
    {
        [Key]
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
