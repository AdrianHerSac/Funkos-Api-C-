using CSharpFunctionalExtensions;
using FunkosApi.Error;
using FunkosApi.dto;

namespace FunkosApi.Services;

public interface IService
{
    Task<List<FunkoResponseDto>> GetFunkosAsync();
    Task<Result<FunkoResponseDto,FunkoError>> GetFunkoAsync(string id);
    Task<Result<FunkoResponseDto,FunkoError>> SaveFunkoAsync( FunkoRequestDto request);
    Task<Result<FunkoResponseDto,FunkoError>> DeleteFunkoAsync(string id);
    Task<Result<FunkoResponseDto,FunkoError>> UpdateFunkoAsync(string id,FunkoRequestDto request);
    Task<Result<FunkoResponseDto, FunkoError>> PatchFunkoAsync(string id, FunkoRequestDto request);
}