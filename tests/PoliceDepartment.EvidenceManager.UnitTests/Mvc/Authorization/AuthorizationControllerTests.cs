using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using PoliceDepartment.EvidenceManager.MVC.Authorization.Interfaces;
using PoliceDepartment.EvidenceManager.MVC.Controllers;
using PoliceDepartment.EvidenceManager.MVC.Models;
using PoliceDepartment.EvidenceManager.SharedKernel.Logger;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using PoliceDepartment.EvidenceManager.SharedKernel.Responses;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;

namespace PoliceDepartment.EvidenceManager.UnitTests.Mvc.Authorization
{
    public class AuthorizationControllerTests
    {
        private readonly ILoggerManager _logger;
        private readonly ILogin _login;
        private readonly ILogout _logout;

        public AuthorizationControllerTests()
        {
            _logger = Substitute.For<ILoggerManager>();
            _login = Substitute.For<ILogin>();
            _logout = Substitute.For<ILogout>();
        }

        [Theory]
        [InlineData("invalidEmailFormat", "password", "Username is invalid")]
        [InlineData("", "password", "Username is required")]
        [InlineData(" ", "password", "Username is required")]
        [InlineData(null, "password", "Username is required")]
        [InlineData("valid@email.com", "", "Password is required")]
        [InlineData("valid@email.com", " ", "Password is required")]
        [InlineData("valid@email.com", null, "Password is required")]
        [InlineData(null, null, "Username is required", "Password is required")]
        [InlineData("", null, "Username is required", "Password is required")]
        [InlineData(" ", null, "Username is required", "Password is required")]
        [InlineData("", "", "Username is required", "Password is required")]
        [InlineData("", " ", "Username is required", "Password is required")]
        [InlineData(" ", " ", "Username is required", "Password is required")]
        [InlineData(null, " ", "Username is required", "Password is required")]
        [InlineData(null, "", "Username is required", "Password is required")]
        public void LoginModelState_InvalidModel_ShouldReturnInvalid(string username, string password, params string[] errorMessages)
        {
            //Arrange
            var sut = new LoginModel { Username = username, Password = password };
            var context = new ValidationContext(sut, null, null);
            var results = new List<ValidationResult>();
            TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(LoginModel), typeof(LoginModel)), typeof(LoginModel));

            //Act
            var response = Validator.TryValidateObject(sut, context, results, true);

            //Assert
            response.Should().BeFalse();
            if (errorMessages is not null && errorMessages.Any())
            {
                results.Select(r => r.ErrorMessage).Should().Contain(errorMessages);
                results.Count.Should().Be(errorMessages.Length);
            }
        }

        [Fact]
        public void LoginPost_InvalidCredentials_ShouldReturnViewWithErrors()
        {
            //Arrange
            var viewModel = new LoginModel { Username = "valid@email.com", Password = "password123" };
            _login.RunAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                  .Returns(Task.FromResult(new BaseResponse().AsError(ResponseMessage.InvalidCredentials)));

            var sut = new AuthorizationController(_logger, _login, _logout);

            //Act
            var response = sut.Login(viewModel, CancellationToken.None).Result as ViewResult;

            //Assert
            response?.ViewData.ModelState.Keys.Count().Should().Be(1);
            response?.ViewData.ModelState.Keys.Should().Contain(string.Empty);
            response?.ViewData?.ModelState[string.Empty]?.Errors.Count.Should().Be(1);
            response?.ViewData?.ModelState[string.Empty]?.Errors.Select(e => e.ErrorMessage).Should().Contain("Invalid credentials");
        }

        [Fact]
        public void LoginPost_InvalidRequestWithMultipleErrorMessages_ShouldReturnViewWithErrors()
        {
            //Arrange
            var viewModel = new LoginModel { Username = "valid@email.com", Password = "password123" };
            var expectedResponse = new BaseResponse().AsError(ResponseMessage.InvalidCredentials, "error example", "second error example");
            _login.RunAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                  .Returns(Task.FromResult(expectedResponse));

            var sut = new AuthorizationController(_logger, _login, _logout);

            //Act
            var response = sut.Login(viewModel, CancellationToken.None).Result as ViewResult;

            //Assert
            response?.ViewData.ModelState.Keys.Count().Should().Be(1);
            response?.ViewData.ModelState.Keys.Should().Contain(string.Empty);
            response?.ViewData?.ModelState[string.Empty]?.Errors.Count.Should().Be(2);
            response?.ViewData?.ModelState[string.Empty]?.Errors.Select(e => e.ErrorMessage).Should().Contain(expectedResponse.ResponseDetails.Errors);
        }

        [Fact]
        public void LoginPost_ValidRequestButNoSignIn_ShouldReturnError()
        {
            //Arrange
            var viewModel = new LoginModel { Username = "valid@email.com", Password = "password123" };
            _login.RunAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(new BaseResponse().AsSuccess()));

            var sut = new AuthorizationController(_logger, _login, _logout)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal()
                    }
                }
            };

            //Act
            var response = sut.Login(viewModel, CancellationToken.None).Result as ViewResult;

            //Assert
            response?.ViewData.ModelState.Keys.Count().Should().Be(1);
            response?.ViewData.ModelState.Keys.Should().Contain(string.Empty);
            response?.ViewData?.ModelState[string.Empty]?.Errors.Count.Should().Be(1);
            response?.ViewData?.ModelState[string.Empty]?.Errors.Select(e => e.ErrorMessage).Should().Contain("An error ocurred, try again later");
        }

        [Fact]
        public void LoginPost_ValidRequest_ShouldReturnSuccess()
        {
            //Arrange
            var viewModel = new LoginModel { Username = "valid@email.com", Password = "password123" };
            _login.RunAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(new BaseResponse().AsSuccess()));

            var identity = new GenericIdentity("valid@email.com", "email");
            var sut = new AuthorizationController(_logger, _login, _logout)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(identity)
                    }
                }
            };

            //Act
            var response = sut.Login(viewModel, CancellationToken.None).Result as ViewResult;

            //Assert
            response?.ViewData.ModelState.Keys.Count().Should().Be(0);
        }

        [Fact]
        public void LogoutGet_UserNotAuthenticated_ShouldReturnToHome()
        {
            //Arrange
            var sut = new AuthorizationController(_logger, _login, _logout)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal()
                    }
                }
            };

            //Act
            var response = sut.Logout(CancellationToken.None).Result as RedirectToActionResult;

            //Assert
            response?.ActionName.Should().Be("Index");
            response?.ControllerName.Should().Be("Home");
        }

        [Fact]
        public void LogoutGet_UserAuthenticated_ShouldReturnToHome()
        {
            //Arrange
            var identity = new GenericIdentity("valid@email.com", "email");
            var sut = new AuthorizationController(_logger, _login, _logout)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(identity)
                    }
                }
            };

            //Act
            var response = sut.Logout(CancellationToken.None).Result as RedirectToActionResult;

            //Assert
            response?.ActionName.Should().Be("Index");
            response?.ControllerName.Should().Be("Home");
        }
    }
}
