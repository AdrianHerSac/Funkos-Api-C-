using FunkosApi.config;
using FunkosApi.Models;
using FunkosApi.Repositories;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace FunkosApiTest.Repositories;

[TestFixture]
public class FunkoRepositoryTest
{
    private MongoDbContainer _mongoDbContainer;
    private IFunkoRepository _repository;
    private IMongoCollection<Funko> _funkosCollection;

#pragma warning disable NUnit1032
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        try
        {
            _mongoDbContainer = new MongoDbBuilder()
                .WithImage("mongo:latest")
                .Build();
            await _mongoDbContainer.StartAsync();

            var mongoClient = new MongoClient(_mongoDbContainer.GetConnectionString());
            var context = new MongoDbContext(_mongoDbContainer.GetConnectionString());
            _funkosCollection = context.Funkos;
            _repository = new FunkoRepository(context);
        }
        catch (Exception ex) when (ex.GetType().Name.Contains("DockerUnavailableException") || ex.Message.Contains("Docker"))
        {
            Assert.Ignore("Docker is not available. Skipping integration tests.");
        }
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        if (_mongoDbContainer != null)
        {
            await _mongoDbContainer.DisposeAsync();
        }
    }

    [SetUp]
    public async Task SetUp()
    {
        // Clean database before each test
        await _funkosCollection.DeleteManyAsync(_ => true);
    }

#pragma warning restore NUnit1032

    [Test]
    public async Task GetByIdAsync_WithInvalidObjectId_ShouldReturnNull()
    {
        // Arrange
        string invalidId = "invalid-id-format";

        // Act
        var result = await _repository.GetByIdAsync(invalidId);

        // Assert
        Assert.That(result, Is.Null, "Invalid ObjectId should return null instead of throwing exception");
    }

    [Test]
    public async Task GetByIdAsync_WithValidNonExistentObjectId_ShouldReturnNull()
    {
        // Arrange
        string validButNonExistentId = "507f1f77bcf86cd799439011";

        // Act
        var result = await _repository.GetByIdAsync(validButNonExistentId);

        // Assert
        Assert.That(result, Is.Null, "Non-existent valid ObjectId should return null");
    }

    [Test]
    public async Task GetByIdAsync_WithExistingFunko_ShouldReturnFunko()
    {
        // Arrange
        var funko = new Funko
        {
            Nombre = "Test Funko",
            Categoria = "Test",
            Precio = 10.99,
            Stock = 5,
            Descripcion = "Test Description",
            Imagen = "test.jpg",
            IsDeleted = false
        };
        var addedFunko = await _repository.AddAsync(funko);

        // Act
        var result = await _repository.GetByIdAsync(addedFunko.Id!);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(addedFunko.Id));
        Assert.That(result.Nombre, Is.EqualTo("Test Funko"));
    }

    [Test]
    public async Task DeleteAsync_WithInvalidObjectId_ShouldReturnNull()
    {
        // Arrange
        string invalidId = "someid";

        // Act
        var result = await _repository.DeleteAsync(invalidId);

        // Assert
        Assert.That(result, Is.Null, "Deleting with invalid ObjectId should return null instead of throwing exception");
    }

    [Test]
    public async Task DeleteAsync_WithValidNonExistentObjectId_ShouldReturnNull()
    {
        // Arrange
        string validButNonExistentId = "507f1f77bcf86cd799439011";

        // Act
        var result = await _repository.DeleteAsync(validButNonExistentId);

        // Assert
        Assert.That(result, Is.Null, "Deleting non-existent valid ObjectId should return null");
    }

    [Test]
    public async Task DeleteAsync_WithExistingFunko_ShouldDeleteAndReturnFunko()
    {
        // Arrange
        var funko = new Funko
        {
            Nombre = "Funko to Delete",
            Categoria = "Test",
            Precio = 15.99,
            Stock = 3,
            Descripcion = "Will be deleted",
            Imagen = "delete.jpg",
            IsDeleted = false
        };
        var addedFunko = await _repository.AddAsync(funko);

        // Act
        var result = await _repository.DeleteAsync(addedFunko.Id!);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(addedFunko.Id));
        
        // Verify it's actually deleted
        var searchResult = await _repository.GetByIdAsync(addedFunko.Id!);
        Assert.That(searchResult, Is.Null);
    }

    [Test]
    public async Task UpdateAsync_WithInvalidObjectId_ShouldReturnNull()
    {
        // Arrange
        string invalidId = "invalid";
        var funkoUpdate = new Funko
        {
            Nombre = "Updated Funko",
            Categoria = "Updated",
            Precio = 20.99,
            Stock = 10,
            Descripcion = "Updated",
            Imagen = "updated.jpg"
        };

        // Act
        var result = await _repository.UpdateAsync(invalidId, funkoUpdate);

        // Assert
        Assert.That(result, Is.Null, "Updating with invalid ObjectId should return null instead of throwing exception");
    }

    [Test]
    public async Task UpdateAsync_WithValidNonExistentObjectId_ShouldReturnNull()
    {
        // Arrange
        string validButNonExistentId = "507f1f77bcf86cd799439011";
        var funkoUpdate = new Funko
        {
            Nombre = "Updated Funko",
            Categoria = "Updated",
            Precio = 20.99,
            Stock = 10,
            Descripcion = "Updated",
            Imagen = "updated.jpg"
        };

        // Act
        var result = await _repository.UpdateAsync(validButNonExistentId, funkoUpdate);

        // Assert
        Assert.That(result, Is.Null, "Updating non-existent valid ObjectId should return null");
    }

    [Test]
    public async Task UpdateAsync_WithExistingFunko_ShouldUpdateAndReturnUpdatedFunko()
    {
        // Arrange
        var funko = new Funko
        {
            Nombre = "Original Funko",
            Categoria = "Original",
            Precio = 10.99,
            Stock = 5,
            Descripcion = "Original",
            Imagen = "original.jpg",
            IsDeleted = false
        };
        var addedFunko = await _repository.AddAsync(funko);

        var funkoUpdate = new Funko
        {
            Nombre = "Updated Funko",
            Categoria = "Updated Category",
            Precio = 25.99,
            Stock = 15,
            Descripcion = "Updated Description",
            Imagen = "updated.jpg"
        };

        // Act
        var result = await _repository.UpdateAsync(addedFunko.Id!, funkoUpdate);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Nombre, Is.EqualTo("Updated Funko"));
        Assert.That(result.Categoria, Is.EqualTo("Updated Category"));
        Assert.That(result.Precio, Is.EqualTo(25.99));
        Assert.That(result.Stock, Is.EqualTo(15));
    }

    [Test]
    public async Task FindByIdAsync_WithInvalidObjectId_ShouldReturnNull()
    {
        string invalidId = "bad-format";

        var result = await _repository.FindByIdAsync(invalidId);

        Assert.That(result, Is.Null, "FindByIdAsync with invalid ObjectId should return null instead of throwing exception");
    }

    [Test]
    public async Task FindByIdAsync_WithExistingFunko_ShouldReturnFunko()
    {
        // Arrange
        var funko = new Funko
        {
            Nombre = "FindById Test",
            Categoria = "Test",
            Precio = 12.99,
            Stock = 7,
            Descripcion = "FindById",
            Imagen = "find.jpg",
            IsDeleted = false
        };
        var addedFunko = await _repository.AddAsync(funko);

        // Act
        var result = await _repository.FindByIdAsync(addedFunko.Id!);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(addedFunko.Id));
        Assert.That(result.Nombre, Is.EqualTo("FindById Test"));
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllFunkos()
    {
        // Arrange
        await _repository.AddAsync(new Funko { Nombre = "Funko 1", Categoria = "Cat1", Precio = 10, Stock = 5 });
        await _repository.AddAsync(new Funko { Nombre = "Funko 2", Categoria = "Cat2", Precio = 20, Stock = 10 });
        await _repository.AddAsync(new Funko { Nombre = "Funko 3", Categoria = "Cat3", Precio = 30, Stock = 15 });

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(3));
    }

    [Test]
    public async Task AddAsync_ShouldAddFunkoAndReturnIt()
    {
        // Arrange
        var funko = new Funko
        {
            Nombre = "New Funko",
            Categoria = "New Category",
            Precio = 19.99,
            Stock = 8,
            Descripcion = "Brand New",
            Imagen = "new.jpg",
            IsDeleted = false
        };

        // Act
        var result = await _repository.AddAsync(funko);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.Not.Null);
        Assert.That(result.Nombre, Is.EqualTo("New Funko"));
        
        // Verify it's in the database
        var retrieved = await _repository.GetByIdAsync(result.Id!);
        Assert.That(retrieved, Is.Not.Null);
    }

    [Test]
    public async Task FindByNombreAsync_WithExistingName_ShouldReturnFunko()
    {
        // Arrange
        var funko = new Funko
        {
            Nombre = "Unique Name",
            Categoria = "Test",
            Precio = 14.99,
            Stock = 6,
            Descripcion = "Find by name test",
            Imagen = "name.jpg",
            IsDeleted = false
        };
        await _repository.AddAsync(funko);

        // Act
        var result = await _repository.FindByNombreAsync("Unique Name");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Nombre, Is.EqualTo("Unique Name"));
    }

    [Test]
    public async Task FindByNombreAsync_WithNonExistentName_ShouldReturnNull()
    {
        // Act
        var result = await _repository.FindByNombreAsync("Non Existent Name");

        // Assert
        Assert.That(result, Is.Null);
    }
}