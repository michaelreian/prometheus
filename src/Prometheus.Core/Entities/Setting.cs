using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prometheus.Core.Entities
{
    [Table("setting", Schema = "public")]
    public class Setting
    {
        [StringLength(256)]
        public string Name { get; set; }
        [StringLength(2048)]
        public string Value { get; set; }
    }
}