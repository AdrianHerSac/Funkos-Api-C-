using FluentAssertions;
using FunkosApi.Models;
using FunkosAPI.GraphQL.Types;
using HotChocolate.Types;
using NUnit.Framework;

namespace FunkosApiTest.GraphQL.Types;

[TestFixture]
public class FunkoTypeTest
{
    private FunkoType _funkoType;

    [SetUp]
    public void Setup()
    {
        _funkoType = new FunkoType();
    }

    [Test]
    public void FunkoType_ShouldInheritFromObjectType()
    {
        // Assert
        _funkoType.Should().BeAssignableTo<ObjectType<Funko>>();
    }

    [Test]
    public void FunkoType_ShouldHavePublicConstructor()
    {
        // Act
        var instance = new FunkoType();

        // Assert
        instance.Should().NotBeNull();
        instance.Should().BeOfType<FunkoType>();
    }

    [Test]
    public void FunkoType_ShouldHaveConfigureMethod()
    {
        // Arrange
        // Añadimos el array de tipos (tercer argumento) para decir: 
        // "Dame el Configure que recibe un IObjectTypeDescriptor<Funko>"
        var method = typeof(FunkoType).GetMethod(
            "Configure",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
            new Type[] { typeof(IObjectTypeDescriptor<Funko>) } 
        );

        // Assert
        method.Should().NotBeNull("El método Configure debe existir y no ser ambiguo");
        method!.ReturnType.Should().Be(typeof(void));
    }

    [Test]
    public void FunkoType_ConfigureMethod_ShouldHaveCorrectParameter()
    {
        // Arrange
        // Especificamos que buscamos el método "Configure" que acepta un parámetro de tipo IObjectTypeDescriptor<Funko>
        var method = typeof(FunkoType).GetMethod(
            "Configure",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
            new Type[] { typeof(IObjectTypeDescriptor<Funko>) } // <--- ESTO ES LO QUE FALTABA
        );

        // Assert
        method.Should().NotBeNull("El método Configure debería existir y ser protected/private");
    
        var parameters = method!.GetParameters();
        parameters.Should().HaveCount(1);
        parameters[0].ParameterType.Should().Be(typeof(IObjectTypeDescriptor<Funko>));
    }

    [Test]
    public void FunkoType_ShouldBePublicClass()
    {
        // Assert
        typeof(FunkoType).IsPublic.Should().BeTrue();
    }

    [Test]
    public void FunkoType_ShouldNotBeAbstract()
    {
        // Assert
        typeof(FunkoType).IsAbstract.Should().BeFalse();
    }

    [Test]
    public void FunkoType_ShouldNotBeSealed()
    {
        // Assert
        typeof(FunkoType).IsSealed.Should().BeFalse();
    }

    [Test]
    public void FunkoType_Namespace_ShouldBeCorrect()
    {
        // Assert
        typeof(FunkoType).Namespace.Should().Be("FunkosAPI.GraphQL.Types");
    }
}
