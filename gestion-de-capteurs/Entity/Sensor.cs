namespace gestion_de_capteurs.Models
{
    public class Sensor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public DateTime InstalledDate { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
