using gestion_de_capteurs.Context;
using gestion_de_capteurs.Dto;
using gestion_de_capteurs.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gestion_de_capteurs.Service
{
    public class SensorsService
    {
        private readonly AppDbContext _context;

        public SensorsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Sensor>> GetSensorsAsync()
        {
            return await _context.Sensors.ToListAsync();
        }

        public async Task<Sensor> GetSensorByIdAsync(int id)
        {
            return await _context.Sensors.FindAsync(id);
        }

        public async Task<(Sensor sensor, string message)> CreateSensorAsync(SensorDto sensorDto)
        {
            var sensor = new Sensor
            {
                Name = sensorDto.Name,
                Location = sensorDto.Location,
                Type = sensorDto.Type,
                InstalledDate = System.DateTime.Now
            };

            _context.Sensors.Add(sensor);
            await _context.SaveChangesAsync();
            return (sensor, "Sensor created successfully.");
        }

        public async Task<(bool success, string message)> UpdateSensorAsync(int id, SensorDto sensorDto)
        {
            var sensor = await _context.Sensors.FindAsync(id);
            if (sensor == null) return (false, $"Sensor with ID {id} not found.");

            sensor.Name = sensorDto.Name;
            sensor.Location = sensorDto.Location;
            sensor.Type = sensorDto.Type;
            sensor.DateUpdated = System.DateTime.Now;

            _context.Entry(sensor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SensorExists(id)) return (false, $"Sensor with ID {id} not found.");
                throw;
            }

            return (true, "Sensor updated successfully.");
        }

        public async Task<(bool success, string message)> DeleteSensorAsync(int id)
        {
            var sensor = await _context.Sensors.FindAsync(id);
            if (sensor == null) return (false, $"Sensor with ID {id} not found.");

            _context.Sensors.Remove(sensor);
            await _context.SaveChangesAsync();
            return (true, "Sensor deleted successfully.");
        }

        private bool SensorExists(int id)
        {
            return _context.Sensors.Any(e => e.Id == id);
        }
    }
}
