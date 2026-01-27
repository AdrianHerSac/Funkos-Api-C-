using FluentAssertions;
using FunkosAPI.GraphQL.Events;
using FunkosAPI.GraphQL.Subscriptions;
using NUnit.Framework;

namespace FunkosApiTest.GraphQL.Subscriptions;

[TestFixture]
public class FunkosSubscriptionTest
{
    private FunkosSubscription _subscription;

    [SetUp]
    public void Setup()
    {
        _subscription = new FunkosSubscription();
    }

    #region OnFunkoCreado Tests

    [Test]
    public void OnFunkoCreado_ShouldReturnSameEvent()
    {
        // Arrange
        var evt = new FunkoCreadoEvent
        {
            FunkoId = 1L,
            Nombre = "New Funko",
            Precio = 25.99m,
            Stock = 10,
            CreatedAt = DateTime.Now
        };

        // Act
        var result = _subscription.OnFunkoCreado(evt);

        // Assert
        result.Should().Be(evt);
        result.FunkoId.Should().Be(1L);
        result.Nombre.Should().Be("New Funko");
    }

    [Test]
    public void OnFunkoCreado_WithMultipleEvents_ShouldReturnCorrectEvent()
    {
        // Arrange
        var evt1 = new FunkoCreadoEvent { FunkoId = 1L, Nombre = "Funko 1", Precio = 10m, Stock = 5, CreatedAt = DateTime.Now };
        var evt2 = new FunkoCreadoEvent { FunkoId = 2L, Nombre = "Funko 2", Precio = 20m, Stock = 15, CreatedAt = DateTime.Now };

        // Act
        var result1 = _subscription.OnFunkoCreado(evt1);
        var result2 = _subscription.OnFunkoCreado(evt2);

        // Assert
        result1.Should().Be(evt1);
        result2.Should().Be(evt2);
        result1.Should().NotBe(result2);
    }

    #endregion

    #region OnFunkoActualizado Tests

    [Test]
    public void OnFunkoActualizado_ShouldReturnSameEvent()
    {
        // Arrange
        var evt = new FunkoActualizadoEvent
        {
            FunkoId = 5L,
            Nombre = "Updated Funko",
            Precio = 35.50m,
            Stock = 20,
            UpdatedAt = DateTime.Now
        };

        // Act
        var result = _subscription.OnFunkoActualizado(evt);

        // Assert
        result.Should().Be(evt);
        result.FunkoId.Should().Be(5L);
        result.Nombre.Should().Be("Updated Funko");
    }

    [Test]
    public void OnFunkoActualizado_WithPartialUpdate_ShouldHandleNulls()
    {
        // Arrange
        var evt = new FunkoActualizadoEvent
        {
            FunkoId = 10L,
            Nombre = "Only Name",
            Precio = null,
            Stock = null,
            UpdatedAt = DateTime.Now
        };

        // Act
        var result = _subscription.OnFunkoActualizado(evt);

        // Assert
        result.Should().Be(evt);
        result.Nombre.Should().Be("Only Name");
        result.Precio.Should().BeNull();
        result.Stock.Should().BeNull();
    }

    #endregion

    #region OnFunkoEliminado Tests

    [Test]
    public void OnFunkoEliminado_ShouldReturnSameEvent()
    {
        // Arrange
        var evt = new FunkoEliminadoEvent
        {
            FunkoId = 99L,
            DeletedAt = DateTime.Now
        };

        // Act
        var result = _subscription.OnFunkoEliminado(evt);

        // Assert
        result.Should().Be(evt);
        result.FunkoId.Should().Be(99L);
    }

    [Test]
    public void OnFunkoEliminado_WithDifferentIds_ShouldReturnCorrectEvent()
    {
        // Arrange
        var evt1 = new FunkoEliminadoEvent { FunkoId = 1L, DeletedAt = DateTime.Now };
        var evt2 = new FunkoEliminadoEvent { FunkoId = 2L, DeletedAt = DateTime.Now };

        // Act
        var result1 = _subscription.OnFunkoEliminado(evt1);
        var result2 = _subscription.OnFunkoEliminado(evt2);

        // Assert
        result1.FunkoId.Should().Be(1L);
        result2.FunkoId.Should().Be(2L);
    }

    #endregion

    #region OnFunkoBajo Tests

    [Test]
    public void OnFunkoBajo_ShouldReturnSameEvent()
    {
        // Arrange
        var evt = new FunkoStockBajoEvent
        {
            FunkoId = 7L,
            Nombre = "Low Stock Funko",
            StockActual = 2,
            UmbralStock = 10,
            DetectedAt = DateTime.Now
        };

        // Act
        var result = _subscription.OnFunkoBajo(evt);

        // Assert
        result.Should().Be(evt);
        result.FunkoId.Should().Be(7L);
        result.Nombre.Should().Be("Low Stock Funko");
        result.StockActual.Should().Be(2);
        result.UmbralStock.Should().Be(10);
    }

    [Test]
    public void OnFunkoBajo_WithZeroStock_ShouldReturnEvent()
    {
        // Arrange
        var evt = new FunkoStockBajoEvent
        {
            FunkoId = 15L,
            Nombre = "Out of Stock",
            StockActual = 0,
            UmbralStock = 5,
            DetectedAt = DateTime.Now
        };

        // Act
        var result = _subscription.OnFunkoBajo(evt);

        // Assert
        result.Should().Be(evt);
        result.StockActual.Should().Be(0);
        result.StockActual.Should().BeLessThan(result.UmbralStock);
    }

    [Test]
    public void OnFunkoBajo_WithCriticalStock_ShouldReturnEvent()
    {
        // Arrange
        var evt = new FunkoStockBajoEvent
        {
            FunkoId = 20L,
            Nombre = "Critical Stock",
            StockActual = 1,
            UmbralStock = 20,
            DetectedAt = DateTime.Now
        };

        // Act
        var result = _subscription.OnFunkoBajo(evt);

        // Assert
        result.Should().Be(evt);
        result.StockActual.Should().BeLessThan(result.UmbralStock);
    }

    #endregion

    #region Integration Tests

    [Test]
    public void AllSubscriptionMethods_ShouldExist()
    {
        // Assert - Verify all methods are present
        var type = typeof(FunkosSubscription);
        
        type.GetMethod("OnFunkoCreado").Should().NotBeNull();
        type.GetMethod("OnFunkoActualizado").Should().NotBeNull();
        type.GetMethod("OnFunkoEliminado").Should().NotBeNull();
        type.GetMethod("OnFunkoBajo").Should().NotBeNull();
    }

    #endregion
}
