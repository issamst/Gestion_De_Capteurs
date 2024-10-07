using gestion_de_capteurs.Models;

namespace gestion_de_capteurs.Dto
{
    public class SensorResponseDto
    {
        public string Message { get; set; }
        public Sensor Data { get; set; }
    }

}
