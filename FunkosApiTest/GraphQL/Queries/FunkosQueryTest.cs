using FluentAssertions;
using FunkosApi.Models;
using FunkosApi.Repositories;
using FunkosAPI.GraphQL.Queries;
using Moq;
using NUnit.Framework;

namespace FunkosApiTest.GraphQL.Queries;

[TestFixture]
public class FunkosQueryTest
{
    private Mock<IFunkoRepository> _mockRepository;
    private FunkosQuery _query;
    private List<Funko> _testFunkos;

    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<IFunkoRepository>();
        _query = new FunkosQuery();

        _testFunkos = new List<Funko>
        {
            new()
            {
                Id = "1",
                Nombre = "Funko 1",
                Precio = 10.0,
                Stock = 5.0,
                Categoria = "Cat1",
                Descripcion = "Desc 1",
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now,
                IsDeleted = false
            },
            new()
            {
                Id = "2",
                Nombre = "Funko 2",
                Precio = 20.0,
                Stock = 10.0,
                Categoria = "Cat2",
                Descripcion = "Desc 2",
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now,
                IsDeleted = false
            }
        };
    }

    [Test]
    public void GetFunkos_ShouldReturnQueryable()
    {
        // Arrange
        var queryable = _testFunkos.AsQueryable();
        _mockRepository.Setup(r => r.FindAllAsNoTracking()).Returns(queryable);

        // Act
        var result = _query.GetFunkos(_mockRepository.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IQueryable<Funko>>();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(_testFunkos);
    }

    [Test]
    public void GetFunkos_ShouldReturnEmptyQueryable_WhenNoFunkos()
    {
        // Arrange
        var emptyQueryable = new List<Funko>().AsQueryable();
        _mockRepository.Setup(r => r.FindAllAsNoTracking()).Returns(emptyQueryable);

        // Act
        var result = _query.GetFunkos(_mockRepository.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Test]
    public async Task GetFunkoById_ShouldReturnFunko_WhenExists()
    {
        // Arrange
        var expectedFunko = _testFunkos[0];
        _mockRepository.Setup(r => r.FindByIdAsync("1")).ReturnsAsync(expectedFunko);

        // Act
        var result = await _query.GetFunkoById("1", _mockRepository.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedFunko);
        result!.Id.Should().Be("1");
        result.Nombre.Should().Be("Funko 1");
    }

    [Test]
    public async Task GetFunkoById_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        _mockRepository.Setup(r => r.FindByIdAsync("999")).ReturnsAsync((Funko?)null);

        // Act
        var result = await _query.GetFunkoById("999", _mockRepository.Object);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void GetFunkosPaged_ShouldReturnQueryable()
    {
        // Arrange
        var queryable = _testFunkos.AsQueryable();
        _mockRepository.Setup(r => r.FindAllAsNoTracking()).Returns(queryable);

        // Act
        var result = _query.GetFunkosPaged(_mockRepository.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IQueryable<Funko>>();
        result.Should().HaveCount(2);
    }

    [Test]
    public void GetFunkosPaged_ShouldReturnEmptyQueryable_WhenNoFunkos()
    {
        // Arrange
        var emptyQueryable = new List<Funko>().AsQueryable();
        _mockRepository.Setup(r => r.FindAllAsNoTracking()).Returns(emptyQueryable);

        // Act
        var result = _query.GetFunkosPaged(_mockRepository.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Test]
    public void GetFunkoPaged_ShouldReturnQueryable()
    {
        // Arrange
        var queryable = _testFunkos.AsQueryable();
        _mockRepository.Setup(r => r.FindAllAsNoTracking()).Returns(queryable);

        // Act
        var result = _query.GetFunkoPaged(_mockRepository.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IQueryable<Funko>>();
        result.Should().HaveCount(2);
    }

    [Test]
    public void GetFunkoPaged_ShouldReturnAllFunkos_IncludingDeleted()
    {
        // Arrange
        var funkosWithDeleted = new List<Funko>
        {
            _testFunkos[0],
            _testFunkos[1],
            new()
            {
                Id = "3",
                Nombre = "Deleted Funko",
                IsDeleted = true,
                Precio = 5.0,
                Stock = 0.0,
                Categoria = "Cat",
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now
            }
        }.AsQueryable();

        _mockRepository.Setup(r => r.FindAllAsNoTracking()).Returns(funkosWithDeleted);

        // Act
        var result = _query.GetFunkoPaged(_mockRepository.Object);

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(f => f.IsDeleted);
    }

    [Test]
    public void GetFunkos_ShouldCallRepository_Once()
    {
        // Arrange
        var queryable = _testFunkos.AsQueryable();
        _mockRepository.Setup(r => r.FindAllAsNoTracking()).Returns(queryable);

        // Act
        _query.GetFunkos(_mockRepository.Object);

        // Assert
        _mockRepository.Verify(r => r.FindAllAsNoTracking(), Times.Once);
    }

    [Test]
    public async Task GetFunkoById_ShouldCallRepository_Once()
    {
        // Arrange
        var expectedFunko = _testFunkos[0];
        _mockRepository.Setup(r => r.FindByIdAsync("1")).ReturnsAsync(expectedFunko);

        // Act
        await _query.GetFunkoById("1", _mockRepository.Object);

        // Assert
        _mockRepository.Verify(r => r.FindByIdAsync("1"), Times.Once);
    }
}
