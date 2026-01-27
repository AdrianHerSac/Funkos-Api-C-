using FluentAssertions;
using FunkosAPI.GraphQL.Events;
using NUnit.Framework;

namespace FunkosApiTest.GraphQL.Events;

[TestFixture]
public class FunkoCreadoEventTest
{
    [Test]
    public void FunkoCreadoEvent_ShouldInitializeWithDefaultValues()
    {
        // Act
        var evt = new FunkoCreadoEvent();

        // Assert
        evt.FunkoId.Should().Be(0L);
        evt.Nombre.Should().Be(string.Empty);
        evt.Precio.Should().Be(0m);
        evt.Stock.Should().Be(0);
        evt.CreatedAt.Should().Be(default(DateTime));
    }

    [Test]
    public void FunkoCreadoEvent_ShouldSetAllProperties()
    {
        // Arrange
        var now = DateTime.Now;

        // Act
        var evt = new FunkoCreadoEvent
        {
            FunkoId = 123L,
            Nombre = "New Funko",
            Precio = 29.99m,
            Stock = 50,
            CreatedAt = now
        };

        // Assert
        evt.FunkoId.Should().Be(123L);
        evt.Nombre.Should().Be("New Funko");
        evt.Precio.Should().Be(29.99m);
        evt.Stock.Should().Be(50);
        evt.CreatedAt.Should().Be(now);
    }

    [Test]
    public void FunkoCreadoEvent_ShouldBeRecord()
    {
        // Arrange
        var now = DateTime.Now;
        var evt1 = new FunkoCreadoEvent
        {
            FunkoId = 1L,
            Nombre = "Test",
            Precio = 10m,
            Stock = 5,
            CreatedAt = now
        };

        var evt2 = new FunkoCreadoEvent
        {
            FunkoId = 1L,
            Nombre = "Test",
            Precio = 10m,
            Stock = 5,
            CreatedAt = now
        };

        // Assert - Records have value equality
        evt1.Should().Be(evt2);
    }

    [Test]
    public void FunkoCreadoEvent_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var evt1 = new FunkoCreadoEvent { FunkoId = 1L };
        var evt2 = new FunkoCreadoEvent { FunkoId = 2L };

        // Assert
        evt1.Should().NotBe(evt2);
    }
}
