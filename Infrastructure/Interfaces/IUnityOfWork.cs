namespace ApiGetewayAppPesquisa.Infrastructure.Interfaces;

public interface IUnityOfWork
{
    Task SaveChangesAsync(CancellationToken cancellation);
}
