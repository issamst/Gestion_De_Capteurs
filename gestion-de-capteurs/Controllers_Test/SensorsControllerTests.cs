using gestion_de_capteurs.Context;
using gestion_de_capteurs.Controllers;
using gestion_de_capteurs.Dto;
using gestion_de_capteurs.Models;
using gestion_de_capteurs.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace gestion_de_capteurs.Controllers_Test
{
    public class SensorsControllerTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            var context = new AppDbContext(options);
            return context;
        }

        private IConfiguration GetConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            return configurationBuilder.Build();
        }
        [Fact]
        public async Task PostSensor_ShouldReturnCreatedResult()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var sensorsService = new SensorsService(context);
            var configuration = GetConfiguration();
            var controller = new SensorsController(sensorsService, configuration);
            var sensorDto = new SensorDto { Name = "Temperature Sensor", Location = "Building A", Type = "Temperature" };

            // Act
            var result = await controller.PostSensor(sensorDto);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<SensorResponseDto>(okObjectResult.Value); // Use the new response type

            Assert.NotNull(response); // Ensure response is not null
            Assert.Equal("Temperature Sensor", response.Data.Name); // Check the sensor's properties
            Assert.Equal("Building A", response.Data.Location);
            Assert.Equal("Temperature", response.Data.Type);
        }



        [Fact]
        public async Task PutSensor_ShouldReturnOk()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var sensorsService = new SensorsService(context);
            var configuration = GetConfiguration();
            var controller = new SensorsController(sensorsService, configuration);
            var sensor = new Sensor { Name = "Initial Sensor", Location = "Room 101", Type = "Humidity" };
            context.Sensors.Add(sensor);
            await context.SaveChangesAsync();

            var sensorDto = new SensorDto { Name = "Updated Sensor", Location = "Room 102", Type = "Temperature" };

            // Act
            var result = await controller.PutSensor(sensor.Id, sensorDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseMessage = okResult.Value; // Since it returns an anonymous object, you can just get the value directly
            Assert.Contains("Sensor updated successfully.", responseMessage.ToString());
        }

        [Fact]
        public async Task DeleteSensor_ShouldReturnOk()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var sensorsService = new SensorsService(context);
            var configuration = GetConfiguration();
            var controller = new SensorsController(sensorsService, configuration);
            var sensor = new Sensor { Name = "Sensor to Delete", Location = "Room 103", Type = "Pressure" };
            context.Sensors.Add(sensor);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.DeleteSensor(sensor.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseMessage = okResult.Value; 
            Assert.Contains("Sensor deleted successfully.", responseMessage.ToString());

            var deletedSensor = await context.Sensors.FindAsync(sensor.Id);
            Assert.Null(deletedSensor);
        }
    }
}
