using FluentAssertions;
using FunkosAPI.GraphQL.Inputs;
using NUnit.Framework;

namespace FunkosApiTest.GraphQL.Inputs;

[TestFixture]
public class CreateProductoInputTest
{
    [Test]
    public void CreateProductoInput_ShouldInitializeWithDefaultValues()
    {
        // Act
        var input = new CreateProductoInput();

        // Assert
        input.Nombre.Should().Be(string.Empty);
        input.Descripcion.Should().BeNull();
        input.Precio.Should().Be(0m);
        input.Stock.Should().Be(0);
        input.Imagen.Should().BeNull();
        input.CategoriaId.Should().Be(0L);
    }

    [Test]
    public void CreateProductoInput_ShouldSetAllProperties()
    {
        // Act
        var input = new CreateProductoInput
        {
            Nombre = "Test Funko",
            Descripcion = "Test Description",
            Precio = 25.99m,
            Stock = 10,
            Imagen = "https://example.com/image.jpg",
            CategoriaId = 5L
        };

        // Assert
        input.Nombre.Should().Be("Test Funko");
        input.Descripcion.Should().Be("Test Description");
        input.Precio.Should().Be(25.99m);
        input.Stock.Should().Be(10);
        input.Imagen.Should().Be("https://example.com/image.jpg");
        input.CategoriaId.Should().Be(5L);
    }

    [Test]
    public void CreateProductoInput_ShouldBeRecord()
    {
        // Arrange
        var input1 = new CreateProductoInput
        {
            Nombre = "Test",
            Precio = 10m,
            Stock = 5,
            CategoriaId = 1L
        };

        var input2 = new CreateProductoInput
        {
            Nombre = "Test",
            Precio = 10m,
            Stock = 5,
            CategoriaId = 1L
        };

        // Assert - Records have value equality
        input1.Should().Be(input2);
    }

    [Test]
    public void CreateProductoInput_WithNullOptionalFields_ShouldBeValid()
    {
        // Act
        var input = new CreateProductoInput
        {
            Nombre = "Test Funko",
            Precio = 15.50m,
            Stock = 20,
            CategoriaId = 3L,
            Descripcion = null,
            Imagen = null
        };

        // Assert
        input.Should().NotBeNull();
        input.Descripcion.Should().BeNull();
        input.Imagen.Should().BeNull();
    }

    [Test]
    public void CreateProductoInput_ShouldSupportWith_Expression()
    {
        // Arrange
        var original = new CreateProductoInput
        {
            Nombre = "Original",
            Precio = 10m,
            Stock = 5,
            CategoriaId = 1L
        };

        // Act
        var modified = original with { Nombre = "Modified" };

        // Assert
        original.Nombre.Should().Be("Original");
        modified.Nombre.Should().Be("Modified");
        modified.Precio.Should().Be(original.Precio);
    }
}
