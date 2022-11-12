namespace Hackathon2022.Models
{
    public class Report
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PharmaceuticId { get; set; }
        public DateTime DateReported { get; set; }
    }
}
