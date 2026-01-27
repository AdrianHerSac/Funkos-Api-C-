using FluentAssertions;
using FunkosApi.Models;
using FunkosAPI.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types;
using System.Reflection;

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
        _funkoType.Should().BeAssignableTo<ObjectType<Funko>>();
    }

    [Test]
    public void FunkoType_ShouldHavePublicConstructor()
    {
        var instance = new FunkoType();
        instance.Should().NotBeNull();
        instance.Should().BeOfType<FunkoType>();
    }

    [Test]
    public void FunkoType_ShouldHaveConfigureMethod()
    {
        var method = typeof(FunkoType).GetMethod(
            "Configure",
            BindingFlags.NonPublic | BindingFlags.Instance,
            new[] { typeof(IObjectTypeDescriptor<Funko>) } 
        );

        method.Should().NotBeNull("El método Configure debe existir");
        method!.ReturnType.Should().Be(typeof(void));
    }

    [Test]
    public void Configure_ShouldSetTypeName()
    {
        var schema = SchemaBuilder.New()
            .AddType(_funkoType)
            .ModifyOptions(o => o.StrictValidation = false)
            .Create();

        var funkoType = (ObjectType)schema.Types.Single(t => t.Name == "Funko");

        funkoType.Should().NotBeNull();
        funkoType.Name.Should().Be("Funko");
    }

    [Test]
    public void Configure_ShouldSetTypeDescription()
    {
        var schema = SchemaBuilder.New()
            .AddType(_funkoType)
            .ModifyOptions(o => o.StrictValidation = false)
            .Create();

        var funkoType = (ObjectType)schema.Types.Single(t => t.Name == "Funko");

        funkoType.Description.Should().Be("Entidad Funko");
    }

    [Test]
    public void Configure_ShouldConfigureCampos()
    {
        var schema = SchemaBuilder.New()
            .AddType(_funkoType)
            .ModifyOptions(o => o.StrictValidation = false)
            .Create();

        var funkoType = (ObjectType)schema.Types.Single(t => t.Name == "Funko");

        funkoType.Fields["id"].Description.Should().Be("El ID del funko");
        funkoType.Fields["id"].Type.Kind.Should().Be(TypeKind.NonNull);

        funkoType.Fields["nombre"].Description.Should().Be("El nombre del funko");
        funkoType.Fields["nombre"].Type.Kind.Should().Be(TypeKind.NonNull);

        funkoType.Fields["precio"].Description.Should().Be("El precio del funko");
        funkoType.Fields["precio"].Type.Kind.Should().Be(TypeKind.NonNull);

        funkoType.Fields["imagen"].Description.Should().Be("URL de la imagen");
        funkoType.Fields["imagen"].Type.Kind.Should().NotBe(TypeKind.NonNull);
        
        funkoType.Fields["categoria"].Type.Kind.Should().Be(TypeKind.NonNull);
        funkoType.Fields["stock"].Type.Kind.Should().Be(TypeKind.NonNull);
    }

    [Test]
    public void Configure_ShouldHaveCorrectNumberOfFields()
    {
        var schema = SchemaBuilder.New()
            .AddType(_funkoType)
            .ModifyOptions(o => o.StrictValidation = false)
            .Create();

        var funkoType = (ObjectType)schema.Types.Single(t => t.Name == "Funko");

        funkoType.Fields.Should().HaveCount(11);
    }
}