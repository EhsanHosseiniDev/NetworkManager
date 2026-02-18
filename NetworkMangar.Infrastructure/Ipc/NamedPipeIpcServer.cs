using System.IO.Pipes;

namespace NetworkMangar.Infrastructure.Ipc;

public class NamedPipeIpcServer : IIpcServer
{
    private const string PipeName = "MyVpnPipeChannel";

    public async Task WaitForCommandAsync(Action onReloadRequested, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                await using var server = new NamedPipeServerStream(PipeName, PipeDirection.In);
                await server.WaitForConnectionAsync(token);

                using var reader = new StreamReader(server);
                var command = await reader.ReadToEndAsync(token);

                if (command == "RELOAD")
                {
                    onReloadRequested?.Invoke();
                }
            }
            catch (OperationCanceledException) { break; }
            catch
            {
            }
        }
    }
}
