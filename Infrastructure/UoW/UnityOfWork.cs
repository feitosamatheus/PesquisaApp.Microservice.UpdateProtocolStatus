using ApiGetewayAppPesquisa.Infrastructure.Contexts;
using ApiGetewayAppPesquisa.Infrastructure.Interfaces;

namespace ApiGetewayAppPesquisa.Infrastructure.UoW;

public class UnityOfWork : IUnityOfWork
{
    private readonly ApiGatewayContext _gatewayContext;

    public UnityOfWork(ApiGatewayContext gatewayContext)
    {
        _gatewayContext = gatewayContext;
    }

    public async Task SaveChangesAsync(CancellationToken cancellation)
        => await _gatewayContext.SaveChangesAsync(cancellation);

}
