using FluentAssertions;
using FunkosAPI.GraphQL.Events;
using NUnit.Framework;

namespace FunkosApiTest.GraphQL.Events;

[TestFixture]
public class FunkoActualizadoEventTest
{
    [Test]
    public void FunkoActualizadoEvent_ShouldInitializeWithDefaultValues()
    {
        // Act
        var evt = new FunkoActualizadoEvent();

        // Assert
        evt.FunkoId.Should().Be(0L);
        evt.Nombre.Should().BeNull();
        evt.Precio.Should().BeNull();
        evt.Stock.Should().BeNull();
        evt.UpdatedAt.Should().Be(default(DateTime));
    }

    [Test]
    public void FunkoActualizadoEvent_ShouldSetAllProperties()
    {
        // Arrange
        var now = DateTime.Now;

        // Act
        var evt = new FunkoActualizadoEvent
        {
            FunkoId = 456L,
            Nombre = "Updated Funko",
            Precio = 39.99m,
            Stock = 25,
            UpdatedAt = now
        };

        // Assert
        evt.FunkoId.Should().Be(456L);
        evt.Nombre.Should().Be("Updated Funko");
        evt.Precio.Should().Be(39.99m);
        evt.Stock.Should().Be(25);
        evt.UpdatedAt.Should().Be(now);
    }

    [Test]
    public void FunkoActualizadoEvent_WithPartialUpdate_ShouldAllowNulls()
    {
        // Arrange
        var now = DateTime.Now;

        // Act - Partial update event
        var evt = new FunkoActualizadoEvent
        {
            FunkoId = 789L,
            Nombre = "Only Name Updated",
            UpdatedAt = now
            // Precio and Stock remain null
        };

        // Assert
        evt.FunkoId.Should().Be(789L);
        evt.Nombre.Should().Be("Only Name Updated");
        evt.Precio.Should().BeNull();
        evt.Stock.Should().BeNull();
        evt.UpdatedAt.Should().Be(now);
    }

    [Test]
    public void FunkoActualizadoEvent_ShouldBeRecord()
    {
        // Arrange
        var now = DateTime.Now;
        var evt1 = new FunkoActualizadoEvent
        {
            FunkoId = 1L,
            Nombre = "Test",
            UpdatedAt = now
        };

        var evt2 = new FunkoActualizadoEvent
        {
            FunkoId = 1L,
            Nombre = "Test",
            UpdatedAt = now
        };

        // Assert - Records have value equality
        evt1.Should().Be(evt2);
    }

    [Test]
    public void FunkoActualizadoEvent_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var evt1 = new FunkoActualizadoEvent { FunkoId = 1L };
        var evt2 = new FunkoActualizadoEvent { FunkoId = 2L };

        // Assert
        evt1.Should().NotBe(evt2);
    }
}
