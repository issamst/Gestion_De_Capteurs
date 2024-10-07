using gestion_de_capteurs.Context;
using gestion_de_capteurs.Controllers;
using gestion_de_capteurs.Dto;
using gestion_de_capteurs.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace gestion_de_capteurs.Controllers_Test
{
    public class AuthentificationControllerTest
    {
        private readonly AuthentificationController _controller;
        private readonly AuthentificationService _service;
        private readonly AppDbContext _context;

        public AuthentificationControllerTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") 
                .Options;

            _context = new AppDbContext(options);
            _service = new AuthentificationService(_context);
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Jwt:Key"]).Returns("a-very-long-secret-key-that-is-at-least-32-bytes-long!");

            _controller = new AuthentificationController(_service, mockConfig.Object);
        }

        [Fact]
        public async Task Register_UserAlreadyExists_ReturnsBadRequest()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Fullname = "Test User",
                Username = "testuser",
                Password = "Password123"
            };
            await _service.Register(registerDto); // Register first user

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User already exists", badRequestResult.Value);
        }

        [Fact]
        public async Task Register_NewUser_ReturnsOk()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Fullname = "New User",
                Username = "newuser",
                Password = "Password123"
            };

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User registered successfully", okResult.Value);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = "invaliduser",
                Password = "wrongpassword"
            };

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Invalid username or password", unauthorizedResult.Value);
        }
        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Fullname = "Valid User",
                Username = "validuser",
                Password = "Password123"
            };
            await _service.Register(registerDto); // Register user

            var loginDto = new LoginDto
            {
                Username = "validuser",
                Password = "Password123"
            };

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<LoginResponse>(okResult.Value); // Change to the concrete type
            Assert.Equal("Login successful! You can now access the SensorsController.", response.Message);
            Assert.NotNull(response.Token); // Ensure token is not null
        }

    }
}
