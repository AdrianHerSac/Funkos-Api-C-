using FunkosApi.dto;
using FunkosApi.Error;
using FunkosApi.Models;
using FunkosApi.Repository;
using FunkosAPI.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace FunkosApiTest.Services;

[TestFixture]
public class FunkoServiceTest
{
    private Mock<IFunkoRepository> _mockRepository;
    private Mock<ILogger<FunkoService>> _mockLogger;
    private FunkoService _service;

    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<IFunkoRepository>();
        _mockLogger = new Mock<ILogger<FunkoService>>();
        _service = new FunkoService(_mockRepository.Object, _mockLogger.Object);
    }

    #region ObtenerTodos Tests

    [Test]
    public async Task ObtenerTodos_ShouldReturnAllFunkos()
    {
        // Arrange
        var expectedFunkos = new List<Funko>
        {
            new() { Id = "1", Nombre = "Funko 1", Precio = 10.0, Categoria = "Marvel" },
            new() { Id = "2", Nombre = "Funko 2", Precio = 15.0, Categoria = "DC" }
        };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(expectedFunkos);

        // Act
        var result = await _service.ObtenerTodos();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    #endregion

    #region GetFunkosAsync Tests

    [Test]
    public async Task GetFunkosAsync_ShouldReturnListOfDtos()
    {
        // Arrange
        var funkos = new List<Funko>
        {
            new()
            {
                Id = "1",
                Nombre = "Iron Man",
                Precio = 25.99,
                Categoria = "Marvel",
                Imagen = "imagen1.jpg",
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now
            },
            new()
            {
                Id = "2",
                Nombre = "Batman",
                Precio = 30.99,
                Categoria = "DC",
                Imagen = "imagen2.jpg",
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now
            }
        };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(funkos);

        // Act
        var result = await _service.GetFunkosAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].Nombre, Is.EqualTo("Iron Man"));
        Assert.That(result[1].Nombre, Is.EqualTo("Batman"));
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetFunkosAsync_ShouldReturnEmptyList_WhenNoFunkos()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Funko>());

        // Act
        var result = await _service.GetFunkosAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(0));
    }

    #endregion

    #region GetFunkoAsync Tests

    [Test]
    public async Task GetFunkoAsync_ShouldReturnSuccess_WhenFunkoExists()
    {
        // Arrange
        var funkoId = "123";
        var funko = new Funko
        {
            Id = funkoId,
            Nombre = "Spider-Man",
            Precio = 20.99,
            Categoria = "Marvel",
            Imagen = "spidey.jpg",
            FechaCreacion = DateTime.Now,
            FechaModificacion = DateTime.Now
        };
        _mockRepository.Setup(r => r.GetByIdAsync(funkoId)).ReturnsAsync(funko);

        // Act
        var result = await _service.GetFunkoAsync(funkoId);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Nombre, Is.EqualTo("Spider-Man"));
        Assert.That(result.Value.Precio, Is.EqualTo(20.99));
        _mockRepository.Verify(r => r.GetByIdAsync(funkoId), Times.Once);
    }

    [Test]
    public async Task GetFunkoAsync_ShouldReturnFailure_WhenFunkoNotFound()
    {
        // Arrange
        var funkoId = "999";
        _mockRepository.Setup(r => r.GetByIdAsync(funkoId)).ReturnsAsync((Funko?)null);

        // Act
        var result = await _service.GetFunkoAsync(funkoId);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Is.TypeOf<FunkoError>());
        Assert.That(result.Error.Error, Does.Contain("no encontrado"));
    }

    #endregion

    #region SaveFunkoAsync Tests

    [Test]
    public async Task SaveFunkoAsync_ShouldReturnSuccess_WhenFunkoIsNew()
    {
        // Arrange
        var request = new FunkoRequestDto 
        { 
            Nombre = "Hulk", 
            Precio = 35.99, 
            Categoria = "Marvel", 
            Imagen = "hulk.jpg" 
        };
        _mockRepository.Setup(r => r.FindByNombreAsync(request.Nombre)).ReturnsAsync((Funko?)null);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Funko>())).ReturnsAsync((Funko f) =>
        {
            f.Id = "new-id";
            return f;
        });

        // Act
        var result = await _service.SaveFunkoAsync(request);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Nombre, Is.EqualTo("Hulk"));
        Assert.That(result.Value.Precio, Is.EqualTo(35.99));
        Assert.That(result.Value.Id, Is.Not.Empty);
        _mockRepository.Verify(r => r.FindByNombreAsync(request.Nombre), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Funko>()), Times.Once);
    }

    [Test]
    public async Task SaveFunkoAsync_ShouldReturnConflict_WhenFunkoAlreadyExists()
    {
        // Arrange
        var request = new FunkoRequestDto 
        { 
            Nombre = "Iron Man", 
            Precio = 25.99, 
            Categoria = "Marvel" 
        };
        var existingFunko = new Funko { Id = "1", Nombre = "Iron Man" };
        _mockRepository.Setup(r => r.FindByNombreAsync(request.Nombre)).ReturnsAsync(existingFunko);

        // Act
        var result = await _service.SaveFunkoAsync(request);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Is.TypeOf<FunkoConflictError>());
        Assert.That(result.Error.Error, Does.Contain("ya existe"));
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Funko>()), Times.Never);
    }

    [Test]
    public async Task SaveFunkoAsync_ShouldUseDefaultImage_WhenImageIsNull()
    {
        // Arrange
        var request = new FunkoRequestDto 
        { 
            Nombre = "Thor", 
            Precio = 28.99, 
            Categoria = "Marvel" 
        };
        _mockRepository.Setup(r => r.FindByNombreAsync(request.Nombre)).ReturnsAsync((Funko?)null);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Funko>())).ReturnsAsync((Funko f) =>
        {
            f.Id = "new-id";
            return f;
        });

        // Act
        var result = await _service.SaveFunkoAsync(request);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Imagen, Is.EqualTo("https://via.placeholder.com/150"));
    }

    [Test]
    public async Task SaveFunkoAsync_ShouldReturnError_WhenDatabaseFails()
    {
        // Arrange
        var request = new FunkoRequestDto 
        { 
            Nombre = "Loki", 
            Precio = 22.99, 
            Categoria = "Marvel" 
        };
        _mockRepository.Setup(r => r.FindByNombreAsync(request.Nombre)).ReturnsAsync((Funko?)null);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Funko>())).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _service.SaveFunkoAsync(request);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Is.TypeOf<FunkoError>());
        Assert.That(result.Error.Error, Does.Contain("Error al guardar"));
    }

    #endregion

    #region DeleteFunkoAsync Tests

    [Test]
    public async Task DeleteFunkoAsync_ShouldReturnSuccess_WhenFunkoExists()
    {
        // Arrange
        var funkoId = "123";
        var funko = new Funko
        {
            Id = funkoId,
            Nombre = "Captain America",
            Precio = 27.99,
            Categoria = "Marvel",
            Imagen = "cap.jpg",
            FechaCreacion = DateTime.Now,
            FechaModificacion = DateTime.Now
        };
        _mockRepository.Setup(r => r.GetByIdAsync(funkoId)).ReturnsAsync(funko);
        _mockRepository.Setup(r => r.DeleteAsync(funkoId)).ReturnsAsync(funko);

        // Act
        var result = await _service.DeleteFunkoAsync(funkoId);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Nombre, Is.EqualTo("Captain America"));
        _mockRepository.Verify(r => r.GetByIdAsync(funkoId), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(funkoId), Times.Once);
    }

    [Test]
    public async Task DeleteFunkoAsync_ShouldReturnFailure_WhenFunkoNotFound()
    {
        // Arrange
        var funkoId = "999";
        _mockRepository.Setup(r => r.GetByIdAsync(funkoId)).ReturnsAsync((Funko?)null);

        // Act
        var result = await _service.DeleteFunkoAsync(funkoId);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Is.TypeOf<FunkoNotFoundError>());
        Assert.That(result.Error.Error, Does.Contain("no encontrado"));
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<string>()), Times.Never);
    }

    #endregion

    #region UpdateFunkoAsync Tests

    [Test]
    public async Task UpdateFunkoAsync_ShouldReturnSuccess_WhenFunkoExists()
    {
        // Arrange
        var funkoId = "123";
        var funko = new Funko
        {
            Id = funkoId,
            Nombre = "Old Name",
            Precio = 10.0,
            Categoria = "Old Category",
            Imagen = "old.jpg",
            FechaCreacion = DateTime.Now.AddDays(-10),
            FechaModificacion = DateTime.Now.AddDays(-5)
        };
        var request = new FunkoRequestDto 
        { 
            Nombre = "New Name", 
            Precio = 25.99, 
            Categoria = "New Category", 
            Imagen = "new.jpg" 
        };
        
        _mockRepository.Setup(r => r.GetByIdAsync(funkoId)).ReturnsAsync(funko);
        _mockRepository.Setup(r => r.UpdateAsync(funkoId, It.IsAny<Funko>())).ReturnsAsync(funko);

        // Act
        var result = await _service.UpdateFunkoAsync(funkoId, request);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Nombre, Is.EqualTo("New Name"));
        Assert.That(result.Value.Precio, Is.EqualTo(25.99));
        Assert.That(result.Value.Categoria, Is.EqualTo("New Category"));
        _mockRepository.Verify(r => r.UpdateAsync(funkoId, It.IsAny<Funko>()), Times.Once);
    }

    [Test]
    public async Task UpdateFunkoAsync_ShouldReturnFailure_WhenFunkoNotFound()
    {
        // Arrange
        var funkoId = "999";
        var request = new FunkoRequestDto 
        { 
            Nombre = "Name", 
            Precio = 10.0, 
            Categoria = "Category" 
        };
        _mockRepository.Setup(r => r.GetByIdAsync(funkoId)).ReturnsAsync((Funko?)null);

        // Act
        var result = await _service.UpdateFunkoAsync(funkoId, request);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Is.TypeOf<FunkoNotFoundError>());
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<string>(), It.IsAny<Funko>()), Times.Never);
    }

    [Test]
    public async Task UpdateFunkoAsync_ShouldUseDefaultImage_WhenImageIsNull()
    {
        // Arrange
        var funkoId = "123";
        var funko = new Funko
        {
            Id = funkoId,
            Nombre = "Test",
            Precio = 10.0,
            Categoria = "Test",
            Imagen = "test.jpg",
            FechaCreacion = DateTime.Now,
            FechaModificacion = DateTime.Now
        };
        var request = new FunkoRequestDto 
        { 
            Nombre = "Updated", 
            Precio = 15.0, 
            Categoria = "Updated" 
        };
        
        _mockRepository.Setup(r => r.GetByIdAsync(funkoId)).ReturnsAsync(funko);
        _mockRepository.Setup(r => r.UpdateAsync(funkoId, It.IsAny<Funko>())).ReturnsAsync(funko);

        // Act
        var result = await _service.UpdateFunkoAsync(funkoId, request);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Imagen, Is.EqualTo("https://via.placeholder.com/150"));
    }

    #endregion

    #region PatchFunkoAsync Tests

    [Test]
    public async Task PatchFunkoAsync_ShouldReturnSuccess_WhenFunkoExists()
    {
        // Arrange
        var funkoId = "123";
        var funko = new Funko
        {
            Id = funkoId,
            Nombre = "Original",
            Precio = 20.0,
            Categoria = "Original",
            Imagen = "original.jpg",
            FechaCreacion = DateTime.Now.AddDays(-10),
            FechaModificacion = DateTime.Now.AddDays(-5)
        };
        var request = new FunkoRequestDto 
        { 
            Nombre = "Patched", 
            Precio = 30.99, 
            Categoria = "Patched", 
            Imagen = "patched.jpg" 
        };
        
        _mockRepository.Setup(r => r.GetByIdAsync(funkoId)).ReturnsAsync(funko);
        _mockRepository.Setup(r => r.UpdateAsync(funkoId, It.IsAny<Funko>())).ReturnsAsync(funko);

        // Act
        var result = await _service.PatchFunkoAsync(funkoId, request);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Nombre, Is.EqualTo("Patched"));
        Assert.That(result.Value.Precio, Is.EqualTo(30.99));
        _mockRepository.Verify(r => r.UpdateAsync(funkoId, It.IsAny<Funko>()), Times.Once);
    }

    [Test]
    public async Task PatchFunkoAsync_ShouldReturnFailure_WhenFunkoNotFound()
    {
        // Arrange
        var funkoId = "999";
        var request = new FunkoRequestDto 
        { 
            Nombre = "Name", 
            Precio = 10.0, 
            Categoria = "Category" 
        };
        _mockRepository.Setup(r => r.GetByIdAsync(funkoId)).ReturnsAsync((Funko?)null);

        // Act
        var result = await _service.PatchFunkoAsync(funkoId, request);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Is.TypeOf<FunkoNotFoundError>());
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<string>(), It.IsAny<Funko>()), Times.Never);
    }

    [Test]
    public async Task PatchFunkoAsync_ShouldUseDefaultImage_WhenImageIsNull()
    {
        // Arrange
        var funkoId = "123";
        var funko = new Funko
        {
            Id = funkoId,
            Nombre = "Test",
            Precio = 10.0,
            Categoria = "Test",
            Imagen = "test.jpg",
            FechaCreacion = DateTime.Now,
            FechaModificacion = DateTime.Now
        };
        var request = new FunkoRequestDto 
        { 
            Nombre = "Patched", 
            Precio = 15.0, 
            Categoria = "Patched" 
        };
        
        _mockRepository.Setup(r => r.GetByIdAsync(funkoId)).ReturnsAsync(funko);
        _mockRepository.Setup(r => r.UpdateAsync(funkoId, It.IsAny<Funko>())).ReturnsAsync(funko);

        // Act
        var result = await _service.PatchFunkoAsync(funkoId, request);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Imagen, Is.EqualTo("https://via.placeholder.com/150"));
    }

    #endregion
}