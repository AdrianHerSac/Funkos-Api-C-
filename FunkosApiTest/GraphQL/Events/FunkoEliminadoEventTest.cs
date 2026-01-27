using FluentAssertions;
using FunkosAPI.GraphQL.Events;
using NUnit.Framework;

namespace FunkosApiTest.GraphQL.Events;

[TestFixture]
public class FunkoEliminadoEventTest
{
    [Test]
    public void FunkoEliminadoEvent_ShouldInitializeWithDefaultValues()
    {
        // Act
        var evt = new FunkoEliminadoEvent();

        // Assert
        evt.FunkoId.Should().Be(0L);
        evt.DeletedAt.Should().Be(default(DateTime));
    }

    [Test]
    public void FunkoEliminadoEvent_ShouldSetAllProperties()
    {
        // Arrange
        var now = DateTime.Now;

        // Act
        var evt = new FunkoEliminadoEvent
        {
            FunkoId = 999L,
            DeletedAt = now
        };

        // Assert
        evt.FunkoId.Should().Be(999L);
        evt.DeletedAt.Should().Be(now);
    }

    [Test]
    public void FunkoEliminadoEvent_ShouldBeRecord()
    {
        // Arrange
        var now = DateTime.Now;
        var evt1 = new FunkoEliminadoEvent
        {
            FunkoId = 1L,
            DeletedAt = now
        };

        var evt2 = new FunkoEliminadoEvent
        {
            FunkoId = 1L,
            DeletedAt = now
        };

        // Assert - Records have value equality
        evt1.Should().Be(evt2);
    }

    [Test]
    public void FunkoEliminadoEvent_WithDifferentIds_ShouldNotBeEqual()
    {
        // Arrange
        var now = DateTime.Now;
        var evt1 = new FunkoEliminadoEvent { FunkoId = 1L, DeletedAt = now };
        var evt2 = new FunkoEliminadoEvent { FunkoId = 2L, DeletedAt = now };

        // Assert
        evt1.Should().NotBe(evt2);
    }

    [Test]
    public void FunkoEliminadoEvent_WithDifferentTimestamps_ShouldNotBeEqual()
    {
        // Arrange
        var evt1 = new FunkoEliminadoEvent { FunkoId = 1L, DeletedAt = DateTime.Now };
        var evt2 = new FunkoEliminadoEvent { FunkoId = 1L, DeletedAt = DateTime.Now.AddMinutes(1) };

        // Assert
        evt1.Should().NotBe(evt2);
    }
}
