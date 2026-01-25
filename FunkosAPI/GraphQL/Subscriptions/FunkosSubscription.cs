using FunkosAPI.GraphQL.Events;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;

namespace FunkosAPI.GraphQL.Subscriptions;

public class FunkosSubscription
{
    [Authorize]
    [Subscribe]
    [Topic]
    public FunkoCreadoEvent OnFunkoCreado([EventMessage] FunkoCreadoEvent message) => message;

    [Authorize]
    [Subscribe]
    [Topic]
    public FunkoActualizadoEvent OnFunkoActualizado([EventMessage] FunkoActualizadoEvent message) => message;

    [Authorize]
    [Subscribe]
    [Topic]
    public FunkoEliminadoEvent OnFunkoEliminado([EventMessage] FunkoEliminadoEvent message) => message;
    
    [Authorize]
    [Subscribe]
    [Topic]
    public FunkoStockBajoEvent OnFunkoBajo([EventMessage] FunkoStockBajoEvent message) => message;
}