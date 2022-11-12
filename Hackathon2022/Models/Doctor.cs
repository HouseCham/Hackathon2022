namespace Hackathon2022.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Token { get; set; }

        public Doctor(int id, string name, string surname, string token)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Token = token;
        }
    }
}
