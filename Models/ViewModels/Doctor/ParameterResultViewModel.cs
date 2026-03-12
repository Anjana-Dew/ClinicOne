namespace ClinicOne.Models.ViewModels.Doctor
{
    public class ParameterResultViewModel
    {
        public string ParameterName { get; set; }
        public decimal TestValue { get; set; }
        public string Unit { get; set; }
        public string ResultStatus { get; set; }
    }
}
