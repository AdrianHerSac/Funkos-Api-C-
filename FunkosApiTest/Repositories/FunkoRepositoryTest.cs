using FunkosApi.Models;
using FunkosApi.Repositories;

namespace FunkosApiTest.Repositories;

[TestFixture]
public class FunkoRepositoryTest
{
    [Test]
    public void Repository_Interface_ShouldHaveAllRequiredMethods()
    {
        var interfaceType = typeof(IFunkoRepository);
        var methods = interfaceType.GetMethods();
        
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
        var repositoryType = typeof(FunkoRepository);
        var interfaces = repositoryType.GetInterfaces();
        
        Assert.That(interfaces.Contains(typeof(IFunkoRepository)), Is.True, 
            "FunkoRepository should implement IFunkoRepository");
    }
    
    [Test]
    public void Repository_Methods_ShouldHaveCorrectSignatures()
    {
        var repositoryType = typeof(FunkoRepository);
        
        var getAllAsync = repositoryType.GetMethod("GetAllAsync");
        Assert.That(getAllAsync, Is.Not.Null);
        Assert.That(getAllAsync.ReturnType.Name, Is.EqualTo("Task`1"));
        
        var getByIdAsync = repositoryType.GetMethod("GetByIdAsync");
        Assert.That(getByIdAsync, Is.Not.Null);
        var parameters = getByIdAsync.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(1));
        Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(string)));
        
        var addAsync = repositoryType.GetMethod("AddAsync");
        Assert.That(addAsync, Is.Not.Null);
        parameters = addAsync.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(1));
        Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(Funko)));
        
        var updateAsync = repositoryType.GetMethod("UpdateAsync");
        Assert.That(updateAsync, Is.Not.Null);
        parameters = updateAsync.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(2));
        Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(string)));
        Assert.That(parameters[1].ParameterType, Is.EqualTo(typeof(Funko)));
        
        var deleteAsync = repositoryType.GetMethod("DeleteAsync");
        Assert.That(deleteAsync, Is.Not.Null);
        parameters = deleteAsync.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(1));
        Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(string)));
        
        var findByNombreAsync = repositoryType.GetMethod("FindByNombreAsync");
        Assert.That(findByNombreAsync, Is.Not.Null);
        parameters = findByNombreAsync.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(1));
        Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(string)));
        
        var findAllAsNoTracking = repositoryType.GetMethod("FindAllAsNoTracking");
        Assert.That(findAllAsNoTracking, Is.Not.Null);
        Assert.That(findAllAsNoTracking.ReturnType.Name, Does.Contain("IQueryable"));
        
        var findByIdAsync = repositoryType.GetMethod("FindByIdAsync");
        Assert.That(findByIdAsync, Is.Not.Null);
        parameters = findByIdAsync.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(1));
        Assert.That(parameters[0].ParameterType, Is.EqualTo(typeof(string)));
    }
    
    [Test]
    public void Repository_ShouldHavePublicConstructor()
    {
        var repositoryType = typeof(FunkoRepository);
        var constructor = repositoryType.GetConstructors().FirstOrDefault();
        
        Assert.That(constructor, Is.Not.Null, "Repository should have a public constructor");
        var parameters = constructor.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(1), "Constructor should take one parameter");
    }
}