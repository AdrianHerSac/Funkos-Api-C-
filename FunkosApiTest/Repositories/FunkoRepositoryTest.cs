using FunkosApi.Models;
using FunkosApi.Repositories;

namespace FunkosApiTest.Repositories;

/// <summary>
/// Unit tests for FunkoRepository
/// Note: Since FunkoRepository depends on MongoDbContext which requires actual MongoDB connection,
/// these tests focus on testing the service layer that uses the repository (FunkoService).
/// For complete repository testing, integration tests with TestContainers would be more appropriate.
/// </summary>
[TestFixture]
public class FunkoRepositoryTest
{
    // Note: The FunkoRepository requires a real MongoDB connection through MongoDbContext
    // Unit tests for the repository layer are already covered through FunkoService tests
    // which properly mock the IFunkoRepository interface.
    
    // For true repository testing, consider:
    // 1. Integration tests with TestContainers (MongoDB container)
    // 2. Testing through the service layer (already done in FunkoServiceTest.cs)
    
    [Test]
    public void Repository_Interface_ShouldHaveAllRequiredMethods()
    {
        // Arrange & Act
        var interfaceType = typeof(IFunkoRepository);
        var methods = interfaceType.GetMethods();
        
        // Assert
        Assert.That(methods.Any(m => m.Name == "GetAllAsync"), Is.True, "GetAllAsync method should exist");
        Assert.That(methods.Any(m => m.Name == "GetByIdAsync"), Is.True, "GetByIdAsync method should exist");
        Assert.That(methods.Any(m => m.Name == "AddAsync"), Is.True, "AddAsync method should exist");
        Assert.That(methods.Any(m => m.Name == "UpdateAsync"), Is.True, "UpdateAsync method should exist");
        Assert.That(methods.Any(m => m.Name == "DeleteAsync"), Is.True, "DeleteAsync method should exist");
        Assert.That(methods.Any(m => m.Name == "FindByNombreAsync"), Is.True, "FindByNombreAsync method should exist");
        Assert.That(methods.Any(m => m.Name == "FindAllAsNoTracking"), Is.True, "FindAllAsNoTracking method should exist");
        Assert.That(methods.Any(m => m.Name == "FindByIdAsync"), Is.True, "FindByIdAsync method should exist");
    }
    
    [Test]
    public void Repository_Implementation_ShouldImplementInterface()
    {
        // Arrange & Act
        var repositoryType = typeof(FunkoRepository);
        var interfaces = repositoryType.GetInterfaces();
        
        // Assert
        Assert.That(interfaces.Contains(typeof(IFunkoRepository)), Is.True, 
            "FunkoRepository should implement IFunkoRepository");
    }
    
    [Test]
    public void Repository_Methods_ShouldHaveCorrectSignatures()
    {
        // Arrange
        var repositoryType = typeof(FunkoRepository);
        
        // Act & Assert - GetAllAsync
        var getAllAsync = repositoryType.GetMethod("GetAllAsync");
        Assert.That(getAllAsync, Is.Not.Null);
        Assert.That(getAllAsync.ReturnType.Name, Is.EqualTo("Task`1"));
        
        // Act & Assert - GetByIdAsync
        var getByIdAsync = repositoryType.GetMethod("GetByIdAsync");
        Assert.That(getByIdAsync, Is.Not.Null);
        var parameters = getByIdAsync.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(1));
        Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(string)));
        
        // Act & Assert - AddAsync
        var addAsync = repositoryType.GetMethod("AddAsync");
        Assert.That(addAsync, Is.Not.Null);
        parameters = addAsync.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(1));
        Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(Funko)));
        
        // Act & Assert - UpdateAsync
        var updateAsync = repositoryType.GetMethod("UpdateAsync");
        Assert.That(updateAsync, Is.Not.Null);
        parameters = updateAsync.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(2));
        Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(string)));
        Assert.That(parameters[1].ParameterType, Is.EqualTo(typeof(Funko)));
        
        // Act & Assert - DeleteAsync
        var deleteAsync = repositoryType.GetMethod("DeleteAsync");
        Assert.That(deleteAsync, Is.Not.Null);
        parameters = deleteAsync.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(1));
        Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(string)));
        
        // Act & Assert - FindByNombreAsync
        var findByNombreAsync = repositoryType.GetMethod("FindByNombreAsync");
        Assert.That(findByNombreAsync, Is.Not.Null);
        parameters = findByNombreAsync.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(1));
        Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(string)));
        
        // Act & Assert - FindAllAsNoTracking
        var findAllAsNoTracking = repositoryType.GetMethod("FindAllAsNoTracking");
        Assert.That(findAllAsNoTracking, Is.Not.Null);
        Assert.That(findAllAsNoTracking.ReturnType.Name, Does.Contain("IQueryable"));
        
        // Act & Assert - FindByIdAsync (duplicate method)
        var findByIdAsync = repositoryType.GetMethod("FindByIdAsync");
        Assert.That(findByIdAsync, Is.Not.Null);
        parameters = findByIdAsync.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(1));
        Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(string)));
    }
    
    [Test]
    public void Repository_ShouldHavePublicConstructor()
    {
        // Arrange & Act
        var repositoryType = typeof(FunkoRepository);
        var constructor = repositoryType.GetConstructors().FirstOrDefault();
        
        // Assert
        Assert.That(constructor, Is.Not.Null, "Repository should have a public constructor");
        var parameters = constructor.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(1), "Constructor should take one parameter");
    }
}