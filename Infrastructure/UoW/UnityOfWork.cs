using ApiGetewayAppPesquisa.Infrastructure.Contexts;
using ApiGetewayAppPesquisa.Infrastructure.Interfaces;

namespace ApiGetewayAppPesquisa.Infrastructure.UoW;

public class UnityOfWork : IUnityOfWork
{
    private readonly ConsumerContext _gatewayContext;

    public UnityOfWork(ConsumerContext gatewayContext)
    {
        _gatewayContext = gatewayContext;
    }

    public async Task SaveChangesAsync(CancellationToken cancellation)
        => await _gatewayContext.SaveChangesAsync(cancellation);

}
