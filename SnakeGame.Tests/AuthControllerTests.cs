using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using SnakeGame.Controllers;
using SnakeGame.Data;
using SnakeGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame.Tests
{
	public class AuthControllerTests
	{
		private readonly AuthController _controller;
		private readonly Mock<IConfiguration> _mockConfiguration;
		private readonly Mock<IDataContext> _mockDataContext;

		public AuthControllerTests()
		{
			_mockConfiguration = new Mock<IConfiguration>();
			_mockDataContext = new Mock<IDataContext>();
			_controller = new AuthController(_mockConfiguration.Object, _mockDataContext.Object);
		}

		[Fact]
		public async Task Register_ReturnsBadRequest_WhenEmailIsInvalid()
		{
			var userDto = new UserRegisterDto
			{
				Email = "invalid email",
				Username = "test",
				Password = "test",
				PasswordConfirmation = "test"
			};

			var users = new List<User>().AsQueryable();

			var mockSet = new Mock<DbSet<User>>();
			mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
			mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
			mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
			mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

			_mockDataContext.Setup(c => c.Users).Returns(mockSet.Object);

			var result = await _controller.Register(userDto);
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result); ;
		}

		[Fact]
		public async Task Register_ReturnsBadRequest_WhenUsernameIsInUse()
		{
			var userDto = new UserRegisterDto
			{
				Email = "test@test.com",
				Username = "test",
				Password = "test",
				PasswordConfirmation = "test"
			};

			var users = new List<User> { new User { Username = "test" } }.AsQueryable();

			var mockSet = new Mock<DbSet<User>>();
			mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
			mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
			mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
			mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

			_mockDataContext.Setup(c => c.Users).Returns(mockSet.Object);

			var result = await _controller.Register(userDto);
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task Register_ReturnsBadRequest_WhenEmailIsInUse()
		{
			var userDto = new UserRegisterDto
			{
				Email = "test@test.com",
				Username = "test",
				Password = "test",
				PasswordConfirmation = "test"
			};

			var users = new List<User> { new User { Email = "test@test.com" } }.AsQueryable();

			var mockSet = new Mock<DbSet<User>>();
			mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
			mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
			mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
			mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

			_mockDataContext.Setup(c => c.Users).Returns(mockSet.Object);

			var result = await _controller.Register(userDto);
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
		}


		[Fact]
		public async Task Register_ReturnsOk_WhenUserIsRegisteredSuccessfully()
		{
			var userDto = new UserRegisterDto
			{
				Email = "test@test.com",
				Username = "test",
				Password = "test",
				PasswordConfirmation = "test"
			};

			var users = new List<User>().AsQueryable();

			var mockSet = new Mock<DbSet<User>>();
			mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
			mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
			mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
			mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

			_mockDataContext.Setup(c => c.Users).Returns(mockSet.Object);

			var result = await _controller.Register(userDto);

			var okResult = Assert.IsType<OkObjectResult>(result);
			var message = Assert.IsType<string>(okResult.Value);
			Assert.Equal("User registered successfully.", Assert.IsType<string>(okResult.Value));
		}
	}
}
