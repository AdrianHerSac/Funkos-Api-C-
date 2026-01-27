using FluentAssertions;
using FunkosAPI.GraphQL.Inputs;
using NUnit.Framework;

namespace FunkosApiTest.GraphQL.Inputs;

[TestFixture]
public class UpdateProductoInputTest
{
    [Test]
    public void UpdateProductoInput_ShouldInitializeWithNullValues()
    {
        // Act
        var input = new UpdateProductoInput();

        // Assert
        input.Nombre.Should().BeNull();
        input.Descripcion.Should().BeNull();
        input.Precio.Should().BeNull();
        input.Stock.Should().BeNull();
        input.Imagen.Should().BeNull();
        input.CategoriaId.Should().BeNull();
    }

    [Test]
    public void UpdateProductoInput_ShouldSetAllProperties()
    {
        // Act
        var input = new UpdateProductoInput
        {
            Nombre = "Updated Funko",
            Descripcion = "Updated Description",
            Precio = 35.99m,
            Stock = 15,
            Imagen = "https://example.com/updated.jpg",
            CategoriaId = 7L
        };

        // Assert
        input.Nombre.Should().Be("Updated Funko");
        input.Descripcion.Should().Be("Updated Description");
        input.Precio.Should().Be(35.99m);
        input.Stock.Should().Be(15);
        input.Imagen.Should().Be("https://example.com/updated.jpg");
        input.CategoriaId.Should().Be(7L);
    }

    [Test]
    public void UpdateProductoInput_ShouldBeRecord()
    {
        // Arrange
        var input1 = new UpdateProductoInput
        {
            Nombre = "Test",
            Precio = 10m
        };

        var input2 = new UpdateProductoInput
        {
            Nombre = "Test",
            Precio = 10m
        };

        // Assert - Records have value equality
        input1.Should().Be(input2);
    }

    [Test]
    public void UpdateProductoInput_WithPartialUpdate_ShouldOnlySetSpecifiedFields()
    {
        // Act - Partial update pattern
        var input = new UpdateProductoInput
        {
            Nombre = "Only Name Updated",
            Precio = 25m
            // Other fields remain null
        };

        // Assert
        input.Nombre.Should().Be("Only Name Updated");
        input.Precio.Should().Be(25m);
        input.Stock.Should().BeNull();
        input.Descripcion.Should().BeNull();
        input.Imagen.Should().BeNull();
        input.CategoriaId.Should().BeNull();
    }

    [Test]
    public void UpdateProductoInput_ShouldAllowAllNullableFields()
    {
        // Act
        var input = new UpdateProductoInput
        {
            Nombre = null,
            Descripcion = null,
            Precio = null,
            Stock = null,
            Imagen = null,
            CategoriaId = null
        };

        // Assert
        input.Should().NotBeNull();
        input.Nombre.Should().BeNull();
        input.Descripcion.Should().BeNull();
        input.Precio.Should().BeNull();
        input.Stock.Should().BeNull();
        input.Imagen.Should().BeNull();
        input.CategoriaId.Should().BeNull();
    }

    [Test]
    public void UpdateProductoInput_ShouldSupportWith_Expression()
    {
        // Arrange
        var original = new UpdateProductoInput
        {
            Nombre = "Original",
            Precio = 10m
        };

        // Act
        var modified = original with { Precio = 20m };

        // Assert
        original.Precio.Should().Be(10m);
        modified.Precio.Should().Be(20m);
        modified.Nombre.Should().Be("Original");
    }

    [Test]
    public void UpdateProductoInput_DifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var input1 = new UpdateProductoInput { Nombre = "Name1" };
        var input2 = new UpdateProductoInput { Nombre = "Name2" };

        // Assert
        input1.Should().NotBe(input2);
    }
}
