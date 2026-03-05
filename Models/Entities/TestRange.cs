using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOne.Models.Entities
{
    public class TestRange
    {
        [Key]
        public int RangeID { get; set; }

        [Required]
        public int ParameterID { get; set; }

        public char? Gender { get; set; }

        public decimal? ReferenceMin { get; set; }
        public decimal? ReferenceMax { get; set; }
        public decimal? CriticalLow { get; set; }
        public decimal? CriticalHigh { get; set; }

        [ForeignKey("ParameterID")]
        public TestParameter TestParameter { get; set; }
    }
}
