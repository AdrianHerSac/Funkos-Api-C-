using CSharpFunctionalExtensions;
using FunkosApi.dto;
using FunkosApi.Error;
using FunkosApi.Services;
using Microsoft.AspNetCore.Mvc;
 
namespace FunkosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class FunkosController(IService service):ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FunkoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync()
    {
        return Ok(await service.GetFunkosAsync());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(FunkoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        
        return await service.GetFunkoAsync(id).Match(
            onSuccess: response => Ok(response),
            onFailure: error=> error switch
            {
                FunkoNotFoundError=> NotFound(new { message = error.Error }),
                _=> StatusCode(500,new  { message = error.Error })
            });
    }

    [HttpPost]
    [ProducesResponseType(typeof(FunkoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] FunkoRequestDto request)
    {
        return await service.SaveFunkoAsync(request).Match(
            onSuccess: response => Created($"/api/funkos/{response.Id}", response),
            onFailure: error => error switch
            {
                FunkoValidationError => BadRequest(new { message = error.Error }),
            
                FunkoConflictError => Conflict(new { message = error.Error }), 
            _ => StatusCode(500, new { message = error.Error })
            });
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(FunkoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutAsync(string id, [FromBody] FunkoRequestDto request)
    {
        return await service.UpdateFunkoAsync(id, request).Match(
            onSuccess: response => Ok(response),
            onFailure: error => error switch
            {
                FunkoValidationError => BadRequest(new { message = error.Error }),
                FunkoNotFoundError => NotFound(new { message = error.Error }),
                _ => StatusCode(500, new { message = error.Error })
            });
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(FunkoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        return await service.DeleteFunkoAsync(id).Match(
            onSuccess: response => Ok($"/api/funkos/{id}"),
            onFailure: error => error switch
            {
                FunkoNotFoundError => NotFound(new { message = error.Error }),
                _ => StatusCode(500, new { message = error.Error })
            });
    }
}