using CSharpFunctionalExtensions;
using FluentAssertions;
using FunkosApi.Controllers;
using FunkosApi.dto;
using FunkosApi.Error;
using FunkosApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace FunkosApiTest.Controllers;

[TestFixture]
public class FunkosControllerTest
{
    private Mock<IService> _mockService; 
    private FunkosController _controller; 

    [SetUp]
    public void Setup()
    {
        _mockService = new Mock<IService>();
        _controller = new FunkosController(_mockService.Object);
    }

    #region GET Tests

    [Test]
    public async Task GetAsync_ShouldReturnOk_WithListOfFunkos()
    {
        // Arrange
        var expectedList = new List<FunkoResponseDto>
        {
            new("1", "Test", 10.0, "Categoria1", 5, "Descripcion", null, false, DateTime.Now, DateTime.Now)
        };
        _mockService.Setup(s => s.GetFunkosAsync()).ReturnsAsync(expectedList);

        // Act
        var result = await _controller.GetAsync();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(expectedList);
    }

    [Test]
    public async Task GetAsync_ShouldReturnOk_WithEmptyList()
    {
        // Arrange
        var expectedList = new List<FunkoResponseDto>();
        _mockService.Setup(s => s.GetFunkosAsync()).ReturnsAsync(expectedList);

        // Act
        var result = await _controller.GetAsync();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(expectedList);
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnOk_WhenFunkoExists()
    {
        var id = "1";
        var expectedFunko = new FunkoResponseDto(id, "Goku", 25.0, "Anime", 10, "Funko de Goku", null, false, DateTime.Now, DateTime.Now);
        
        _mockService.Setup(s => s.GetFunkoAsync(id))
            .ReturnsAsync(Result.Success<FunkoResponseDto, FunkoError>(expectedFunko));

        var result = await _controller.GetByIdAsync(id);
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(expectedFunko);
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnNotFound_WhenFunkoDoesNotExist()
    {
        var id = "99";
        _mockService.Setup(s => s.GetFunkoAsync(id))
            .ReturnsAsync(Result.Failure<FunkoResponseDto, FunkoError>(new FunkoNotFoundError("No existe")));

        var result = await _controller.GetByIdAsync(id);
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    #endregion

    #region POST Tests

    [Test]
    public async Task PostAsync_ShouldReturnCreated_WhenDataIsValid()
    {
        // Arrange
        var request = new FunkoRequestDto 
        { 
            Nombre = "Batman", Precio = 30.0, Categoria = "Comics", Stock = 15 
        };
        var response = new FunkoResponseDto("123", "Batman", 30.0, "Comics", 15, "Funko de Batman", null, false, DateTime.Now, DateTime.Now);

        _mockService.Setup(s => s.SaveFunkoAsync(request))
            .ReturnsAsync(Result.Success<FunkoResponseDto, FunkoError>(response));

        // Act
        var result = await _controller.PostAsync(request);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedResult>().Subject;
        createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        createdResult.Location.Should().Be("/api/funkos/123");
        createdResult.Value.Should().Be(response);
    }

    [Test]
    public async Task PostAsync_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var request = new FunkoRequestDto { Nombre = "" }; // Inválido
        _mockService.Setup(s => s.SaveFunkoAsync(request))
            .ReturnsAsync(Result.Failure<FunkoResponseDto, FunkoError>(new FunkoValidationError("Nombre inválido")));

        // Act
        var result = await _controller.PostAsync(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test]
    public async Task PostAsync_ShouldReturnConflict_WhenFunkoExists()
    {
        // Arrange
        var request = new FunkoRequestDto { Nombre = "Repe" };
        _mockService.Setup(s => s.SaveFunkoAsync(request))
            .ReturnsAsync(Result.Failure<FunkoResponseDto, FunkoError>(new FunkoConflictError("Ya existe")));

        // Act
        var result = await _controller.PostAsync(request);

        // Assert
        result.Should().BeOfType<ConflictObjectResult>();
    }

    #endregion

    #region PUT Tests

    [Test]
    public async Task PutAsync_ShouldReturnOk_WhenUpdateIsSuccessful()
    {
        // Arrange
        var id = "1";
        var request = new FunkoRequestDto { Nombre = "Updated", Precio = 20.0, Categoria = "Updated", Stock = 10 };
        var response = new FunkoResponseDto(id, "Updated", 20.0, "Updated", 10, "Updated Description", null, false, DateTime.Now, DateTime.Now);

        _mockService.Setup(s => s.UpdateFunkoAsync(id, request))
            .ReturnsAsync(Result.Success<FunkoResponseDto, FunkoError>(response));

        // Act
        var result = await _controller.PutAsync(id, request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(response);
    }

    [Test]
    public async Task PutAsync_ShouldReturnNotFound_WhenIdDoesNotExist()
    {
        // Arrange
        var id = "99";
        var request = new FunkoRequestDto();
        _mockService.Setup(s => s.UpdateFunkoAsync(id, request))
            .ReturnsAsync(Result.Failure<FunkoResponseDto, FunkoError>(new FunkoNotFoundError("No encontrado")));

        var result = await _controller.PutAsync(id, request);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test]
    public async Task PutAsync_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var id = "1";
        var request = new FunkoRequestDto { Nombre = "", Precio = 10.0, Categoria = "Cat", Stock = 5 }; // Nombre inválido
        _mockService.Setup(s => s.UpdateFunkoAsync(id, request))
            .ReturnsAsync(Result.Failure<FunkoResponseDto, FunkoError>(new FunkoValidationError("Nombre inválido")));

        // Act
        var result = await _controller.PutAsync(id, request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region PATCH Tests

    [Test]
    public async Task PatchAsync_ShouldReturnAccepted_WhenUpdateIsSuccessful()
    {
        // Arrange
        var id = "1";
        var request = new FunkoRequestDto { Nombre = "Funko", Precio = 50.0, Categoria = "Test", Stock = 5 };
        var response = new FunkoResponseDto(id, "Funko", 50.0, "Test", 5, "Description", null, false, DateTime.Now, DateTime.Now);

        _mockService.Setup(s => s.PatchFunkoAsync(id, request))
            .ReturnsAsync(Result.Success<FunkoResponseDto, FunkoError>(response));

        // Act
        var result = await _controller.PatchAsync(id, request);

        // Assert
        var acceptedResult = result.Should().BeOfType<AcceptedAtActionResult>().Subject;
        acceptedResult.StatusCode.Should().Be(StatusCodes.Status202Accepted);
        acceptedResult.ActionName.Should().Be(nameof(FunkosController.GetByIdAsync));
        acceptedResult.RouteValues.Should().ContainKey("id");
        acceptedResult.RouteValues!["id"].Should().Be(id);
        _mockService.Verify(s => s.PatchFunkoAsync(id, request), Times.Once);
    }

    [Test]
    public async Task PatchAsync_ShouldReturnNotFound_WhenIdDoesNotExist()
    {
        // Arrange
        var id = "99";
        var request = new FunkoRequestDto { Nombre = "Test", Precio = 10.0, Categoria = "Cat", Stock = 5 };
        var errorMessage = "Funko no encontrado";
        
        _mockService.Setup(s => s.PatchFunkoAsync(id, request))
            .ReturnsAsync(Result.Failure<FunkoResponseDto, FunkoError>(new FunkoNotFoundError(errorMessage)));

        // Act
        var result = await _controller.PatchAsync(id, request);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        notFoundResult.Value.Should().NotBeNull();
        _mockService.Verify(s => s.PatchFunkoAsync(id, request), Times.Once);
    }

    [Test]
    public async Task PatchAsync_ShouldReturnInternalServerError_OnGenericError()
    {
        // Arrange
        var id = "1";
        var request = new FunkoRequestDto { Nombre = "Test", Precio = 10.0, Categoria = "Cat", Stock = 5 };
        var errorMessage = "Error de base de datos";
        
        _mockService.Setup(s => s.PatchFunkoAsync(id, request))
            .ReturnsAsync(Result.Failure<FunkoResponseDto, FunkoError>(new FunkoError(errorMessage)));

        // Act
        var result = await _controller.PatchAsync(id, request);

        // Assert
        var serverErrorResult = result.Should().BeOfType<ObjectResult>().Subject;
        serverErrorResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        serverErrorResult.Value.Should().NotBeNull();
        _mockService.Verify(s => s.PatchFunkoAsync(id, request), Times.Once);
    }

    #endregion

    #region DELETE Tests

    [Test]
    public async Task DeleteAsync_ShouldReturnNoContent_WhenDeleted()
    {
        // Arrange
        var id = "1";
        var deletedFunko = new FunkoResponseDto(id, "Deleted", 10.0, "Cat", 0, "Desc", null, true, DateTime.Now, DateTime.Now);
        
        _mockService.Setup(s => s.DeleteFunkoAsync(id))
            .ReturnsAsync(Result.Success<FunkoResponseDto, FunkoError>(deletedFunko));

        // Act
        var result = await _controller.DeleteAsync(id);

        // Assert
        var noContentResult = result.Should().BeOfType<NoContentResult>().Subject;
        noContentResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        _mockService.Verify(s => s.DeleteFunkoAsync(id), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_ShouldReturnNotFound_WhenIdDoesNotExist()
    {
        // Arrange
        var id = "99";
        var errorMessage = "Funko no encontrado";
        
        _mockService.Setup(s => s.DeleteFunkoAsync(id))
            .ReturnsAsync(Result.Failure<FunkoResponseDto, FunkoError>(new FunkoNotFoundError(errorMessage)));

        // Act
        var result = await _controller.DeleteAsync(id);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        notFoundResult.Value.Should().NotBeNull();
        _mockService.Verify(s => s.DeleteFunkoAsync(id), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_ShouldReturnInternalServerError_OnGenericError()
    {
        // Arrange
        var id = "1";
        var errorMessage = "Error base de datos";
        
        _mockService.Setup(s => s.DeleteFunkoAsync(id))
            .ReturnsAsync(Result.Failure<FunkoResponseDto, FunkoError>(new FunkoError(errorMessage)));
        
        // Act
        var result = await _controller.DeleteAsync(id);

        // Assert
        var serverErrorResult = result.Should().BeOfType<ObjectResult>().Subject;
        serverErrorResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        serverErrorResult.Value.Should().NotBeNull();
        _mockService.Verify(s => s.DeleteFunkoAsync(id), Times.Once);
    }

    #endregion
}