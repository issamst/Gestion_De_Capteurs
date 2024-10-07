using gestion_de_capteurs.Dto;
using gestion_de_capteurs.Models;
using gestion_de_capteurs.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace gestion_de_capteurs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private readonly SensorsService _sensorsService;
        private readonly IConfiguration _configuration; 

        public SensorsController(SensorsService sensorsService, IConfiguration configuration) 
        {
            _sensorsService = sensorsService;
            _configuration = configuration; 
        }

        // GET: api/Sensors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sensor>>> GetSensors()
        {
            var sensors = await _sensorsService.GetSensorsAsync();
            if (sensors == null || sensors.Count == 0)
            {
                return NoContent(); // Return 204 if no sensors are found
            }
            return Ok(new { Message = "Retrieved sensors successfully.", Data = sensors }); // Return 200 with the list of sensors
        }

        // GET: api/Sensors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sensor>> GetSensor(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid sensor ID."); // Return 400 for bad requests
            }

            var sensor = await _sensorsService.GetSensorByIdAsync(id);

            if (sensor == null)
            {
                return NotFound($"Sensor with ID {id} not found."); // Return 404 if sensor not found
            }

            return Ok(new { Message = "Sensor retrieved successfully.", Data = sensor }); // Return 200 with the sensor details
        }

        // POST: api/Sensors
        [HttpPost]
        public async Task<ActionResult<SensorResponseDto>> PostSensor(SensorDto sensorDto)
        {
            if (sensorDto == null || string.IsNullOrEmpty(sensorDto.Name))
            {
                return BadRequest("Sensor data is invalid."); // Return 400 if request body is invalid
            }

            var (sensor, message) = await _sensorsService.CreateSensorAsync(sensorDto);

            var response = new SensorResponseDto
            {
                Message = message,
                Data = sensor
            };

            return Ok(response); // Return the response DTO
        }


        // PUT: api/Sensors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSensor(int id, SensorDto sensorDto)
        {
            if (id <= 0 || sensorDto == null)
            {
                return BadRequest("Invalid sensor data or ID.");
            }

            var (updated, message) = await _sensorsService.UpdateSensorAsync(id, sensorDto);
            if (!updated)
            {
                return NotFound(message);
            }

            return Ok(new { Message = message }); // Return 200 with a success message
        }

        // DELETE: api/Sensors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSensor(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid sensor ID.");
            }

            var (deleted, message) = await _sensorsService.DeleteSensorAsync(id);
            if (!deleted)
            {
                return NotFound(message);
            }

            return Ok(new { Message = message }); // Return 200 with a success message
        }


        private bool ValidateToken(string token)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey));
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero // Remove delay of token when expire
                }, out SecurityToken validatedToken);
                return true; // Token is valid
            }
            catch
            {
                return false; // Token is not valid
            }
        }

    }
}
