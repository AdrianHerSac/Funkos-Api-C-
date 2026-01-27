using FluentAssertions;
using FunkosAPI.GraphQL.Events;
using NUnit.Framework;

namespace FunkosApiTest.GraphQL.Events;

[TestFixture]
public class FunkoStockBajoEventTest
{
    [Test]
    public void FunkoStockBajoEvent_ShouldInitializeWithDefaultValues()
    {
        // Act
        var evt = new FunkoStockBajoEvent();

        // Assert
        evt.FunkoId.Should().Be(0L);
        evt.Nombre.Should().Be(string.Empty);
        evt.StockActual.Should().Be(0);
        evt.UmbralStock.Should().Be(0);
        evt.DetectedAt.Should().Be(default(DateTime));
    }

    [Test]
    public void FunkoStockBajoEvent_ShouldSetAllProperties()
    {
        // Arrange
        var now = DateTime.Now;

        // Act
        var evt = new FunkoStockBajoEvent
        {
            FunkoId = 555L,
            Nombre = "Low Stock Funko",
            StockActual = 3,
            UmbralStock = 10,
            DetectedAt = now
        };

        // Assert
        evt.FunkoId.Should().Be(555L);
        evt.Nombre.Should().Be("Low Stock Funko");
        evt.StockActual.Should().Be(3);
        evt.UmbralStock.Should().Be(10);
        evt.DetectedAt.Should().Be(now);
    }

    [Test]
    public void FunkoStockBajoEvent_ShouldBeRecord()
    {
        // Arrange
        var now = DateTime.Now;
        var evt1 = new FunkoStockBajoEvent
        {
            FunkoId = 1L,
            Nombre = "Test",
            StockActual = 2,
            UmbralStock = 5,
            DetectedAt = now
        };

        var evt2 = new FunkoStockBajoEvent
        {
            FunkoId = 1L,
            Nombre = "Test",
            StockActual = 2,
            UmbralStock = 5,
            DetectedAt = now
        };

        // Assert - Records have value equality
        evt1.Should().Be(evt2);
    }

    [Test]
    public void FunkoStockBajoEvent_StockBelowThreshold_ShouldBeValid()
    {
        // Act
        var evt = new FunkoStockBajoEvent
        {
            FunkoId = 1L,
            Nombre = "Critical Stock",
            StockActual = 1,
            UmbralStock = 10,
            DetectedAt = DateTime.Now
        };

        // Assert
        evt.StockActual.Should().BeLessThan(evt.UmbralStock);
    }

    [Test]
    public void FunkoStockBajoEvent_WithZeroStock_ShouldBeValid()
    {
        // Act
        var evt = new FunkoStockBajoEvent
        {
            FunkoId = 1L,
            Nombre = "Out of Stock",
            StockActual = 0,
            UmbralStock = 5,
            DetectedAt = DateTime.Now
        };

        // Assert
        evt.StockActual.Should().Be(0);
        evt.StockActual.Should().BeLessThan(evt.UmbralStock);
    }

    [Test]
    public void FunkoStockBajoEvent_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var evt1 = new FunkoStockBajoEvent { FunkoId = 1L, StockActual = 2 };
        var evt2 = new FunkoStockBajoEvent { FunkoId = 1L, StockActual = 3 };

        // Assert
        evt1.Should().NotBe(evt2);
    }
}
