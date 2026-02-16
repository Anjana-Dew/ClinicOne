using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOne.Models.Entities
{
    public class TestRange
    {
        [Key]
        public int RangeID { get; set; }

        [Required]
        public int TestTypeID { get; set; }
        public decimal? NormalMin { get; set; }
        public decimal? NormalMax { get; set; }
        public decimal? HighMin { get; set; }
        public decimal? HighMax { get; set; }
        public decimal? RiskMin { get; set; }
        public decimal? RiskMax { get; set; }


        [ForeignKey("TestTypeID")]
        public TestType TestType { get; set; }
    }
}
