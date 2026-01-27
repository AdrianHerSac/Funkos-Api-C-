using CSharpFunctionalExtensions;
using FunkosApi.dto;
using FunkosApi.Error;
using FunkosApi.Models;
using FunkosApi.Repositories;
using FunkosApi.Repository;
using FunkosApi.Services;
using FunkoApi.mapper;

namespace FunkosAPI.Services;

public class FunkoService : IService
{
    private readonly IFunkoRepository _repository;
    private readonly ILogger<FunkoService> _logger;

    public FunkoService(IFunkoRepository repository, ILogger<FunkoService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<Funko>> ObtenerTodos()
    {
        _logger.LogInformation("Consultando catálogo completo de Funkos...");
        return await _repository.GetAllAsync();
    }

    public void Insertar(Funko funko)
    {
        funko.FechaCreacion = DateTime.Now;
        funko.FechaModificacion = DateTime.Now;
        _logger.LogInformation(
            "Insertando nuevo Funko: {Nombre} de la categoría {Categoria}",
            funko.Nombre,
            funko.Categoria);
        _repository.AddAsync(funko);
    }

    public async Task<List<FunkoResponseDto>> GetFunkosAsync()
    {
        var funkos = await _repository.GetAllAsync();

        var listaDtos = funkos.Select(f => f.ToDto()).ToList();

        return listaDtos;
    }

    public async Task<Result<FunkoResponseDto, FunkoError>> GetFunkoAsync(string id)
    {
        var f = await _repository.GetByIdAsync(id);

        if (f == null)
        {
            return Result.Failure<FunkoResponseDto, FunkoError>
            (new FunkoNotFoundError($"Funko con ID {id} no encontrado" ));
        }

        return Result.Success<FunkoResponseDto, FunkoError>(f.ToDto());
    }

    public async Task<Result<FunkoResponseDto, FunkoError>> SaveFunkoAsync(FunkoRequestDto request)
    {
        _logger.LogInformation("Guardando Funko: {Nombre}", request.Nombre);

        var existe = await _repository.FindByNombreAsync(request.Nombre);
    
        if (existe is not null)
        {
            return new FunkoConflictError($"El Funko '{request.Nombre}' ya existe.");
        }

        try
        {
            var nuevoFunko = new Funko
            {
                Nombre = request.Nombre,
                Precio = request.Precio,
                Categoria = request.Categoria,
                Stock = request.Stock,
                Descripcion = request.Descripcion ?? string.Empty,
                Imagen = request.Imagen ?? "https://via.placeholder.com/150",
                IsDeleted = false,
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now
            };

            var funkoGuardado = await _repository.AddAsync(nuevoFunko);

            return funkoGuardado.ToDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en BBDD");
            return new FunkoError("Error al guardar en la base de datos");
        }
    }

    public async Task<Result<FunkoResponseDto, FunkoError>> DeleteFunkoAsync(string id)
    {
        _logger.LogInformation("Eliminando Funko: {Id}", id);

        var funko = await _repository.GetByIdAsync(id);

        if (funko is null)
        {
            return Result.Failure<FunkoResponseDto, FunkoError>(
                new FunkoNotFoundError($"Funko con ID {id} no encontrado"));
        }

        await _repository.DeleteAsync(id);

        return Result.Success<FunkoResponseDto, FunkoError>(funko.ToDto());
    }

    public async Task<Result<FunkoResponseDto, FunkoError>> UpdateFunkoAsync(string id, FunkoRequestDto request)
    {
        _logger.LogInformation("Actualizando Funko: {Id}", id);

        var funko = await _repository.GetByIdAsync(id);

        if (funko is null)
        {
            return Result.Failure<FunkoResponseDto, FunkoError>(
                new FunkoNotFoundError($"Funko con ID {id} no encontrado"));
        }

        funko.Nombre = request.Nombre;
        funko.Precio = request.Precio;
        funko.Categoria = request.Categoria;
        funko.Stock = request.Stock;
        funko.Descripcion = request.Descripcion ?? string.Empty;
        funko.Imagen = request.Imagen ?? "https://via.placeholder.com/150";
        funko.FechaModificacion = DateTime.Now;

        await _repository.UpdateAsync(id, funko);

        return Result.Success<FunkoResponseDto, FunkoError>(funko.ToDto());
    }

    public async Task<Result<FunkoResponseDto, FunkoError>> PatchFunkoAsync(string id, FunkoRequestDto request)
    {
        _logger.LogInformation("Actualizando Funko: {Id}", id);

        var funko = await _repository.GetByIdAsync(id);

        if (funko is null)
        {
            return Result.Failure<FunkoResponseDto, FunkoError>(
                new FunkoNotFoundError($"Funko con ID {id} no encontrado"));
        }

        funko.Nombre = request.Nombre;
        funko.Precio = request.Precio;
        funko.Categoria = request.Categoria;
        funko.Stock = request.Stock;
        funko.Descripcion = request.Descripcion ?? string.Empty;
        funko.Imagen = request.Imagen ?? "https://via.placeholder.com/150";
        funko.FechaModificacion = DateTime.Now;

        await _repository.UpdateAsync(id, funko);

        return Result.Success<FunkoResponseDto, FunkoError>(funko.ToDto());
    }
}