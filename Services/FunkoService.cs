using CSharpFunctionalExtensions;
using FunkosApi.dto;
using FunkosApi.Error;
using FunkosApi.Models;
using FunkosApi.Repository;
using FunkosApi.Services;

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

        var listaDtos = funkos.Select(f => new FunkoResponseDto(
            f.Id ?? string.Empty, 
            f.Nombre, 
            f.Precio,
            f.Categoria,
            f.Imagen, 
            f.FechaCreacion,
            f.FechaModificacion
        )).ToList();

        return listaDtos;
    }

    public async Task<Result<FunkoResponseDto, FunkoError>> GetFunkoAsync(string id)
    {
        var f = await _repository.GetByIdAsync(id);

        if (f == null)
        {
            return Result.Failure<FunkoResponseDto, FunkoError>
            (new FunkoError($"Funko con ID {id} no encontrado" ));
        }

        var dto = new FunkoResponseDto(
            f.Id ?? string.Empty, 
            f.Nombre, 
            f.Precio, 
            f.Categoria, 
            f.Imagen ?? string.Empty, 
            f.FechaCreacion, 
            f.FechaModificacion
        );

        return Result.Success<FunkoResponseDto, FunkoError>(dto);
    }

    public async Task<Result<FunkoResponseDto, FunkoError>> SaveFunkoAsync(FunkoRequestDto request)
    {
        _logger.LogInformation("Guardando Funko: {Nombre}", request.Nombre);

        // 1. VERIFICAR DUPLICADOS
        // Usamos 'request.Nombre' (que viene de tu FunkoRequestDto)
        var existe = await _repository.FindByNombreAsync(request.Nombre);
    
        if (existe is not null)
        {
            // Si existe, devolvemos el error que creamos en el paso 2
            return new FunkoConflictError($"El Funko '{request.Nombre}' ya existe.");
        }

        try
        {
            // 2. CREAR SI NO EXISTE
            var nuevoFunko = new Funko
            {
                Nombre = request.Nombre,
                Precio = request.Precio,
                Categoria = request.Categoria,
                Imagen = request.Imagen ?? "https://via.placeholder.com/150", // Imagen por defecto
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now
            };

            var funkoGuardado = await _repository.AddAsync(nuevoFunko);

            // Devolvemos el DTO respuesta
            return new FunkoResponseDto(
                funkoGuardado.Id ?? "",
                funkoGuardado.Nombre,
                funkoGuardado.Precio,
                funkoGuardado.Categoria,
                funkoGuardado.Imagen,
                funkoGuardado.FechaCreacion,
                funkoGuardado.FechaModificacion
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en BBDD");
            return new FunkoError("Error al guardar en la base de datos");
        }
    }

    public Task<Result<FunkoResponseDto, FunkoError>> DeleteFunkoAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<FunkoResponseDto, FunkoError>> UpdateFunkoAsync(string id, FunkoRequestDto request)
    {
        throw new NotImplementedException();
    }
}