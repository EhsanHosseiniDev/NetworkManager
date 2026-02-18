namespace NetworkMangar.Infrastructure.Ipc;

public interface IIpcServer
{
    Task WaitForCommandAsync(Action onReloadRequested, CancellationToken token);
}
