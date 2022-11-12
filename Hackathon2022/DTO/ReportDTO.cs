using Hackathon2022.Models;

namespace Hackathon2022.DTO
{
    public class ReportDTO
    {
        public int Id { get; set; }
        public string PharmaceuticName { get; set; }
        public Doctor doctor { get; set; }
        public User user { get; set; }
        public DateTime Created { get; set; }
        public List<Sympton> SymptonsReported { get; set; }
        public List<Drug> DrugsReported { get; set; }
    }
}