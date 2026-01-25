using FunkosApi.Models;
using FunkosApi.Repositories;
using HotChocolate;
using HotChocolate.Types;

namespace FunkosAPI.GraphQL.Queries;

public class FunkosQuery
{
    [UseFirstOrDefault]
    [UseProjection]
    public IQueryable<Funko> GetFunkos([Service] IFunkoRepository funkoRepository) =>
        funkoRepository.FindAllAsNoTracking();

    [UseFirstOrDefault]
    public Task<Funko?> GetFunkoById(string id, [Service] IFunkoRepository funkoRepository) =>
        funkoRepository.FindByIdAsync(id);

    [UsePaging(MaxPageSize = 100, DefaultPageSize = 10)]
    public IQueryable<Funko> GetFunkosPaged([Service] IFunkoRepository funkoRepository) =>
        funkoRepository.FindAllAsNoTracking();

    [UsePaging(MaxPageSize = 100, DefaultPageSize = 10)]
    public IQueryable<Funko> GetFunkoPaged([Service] IFunkoRepository funkoRepository) =>
        funkoRepository.FindAllAsNoTracking();
}