using Client;
using Server;

internal class Program
{
    private const int TEMPO_SINCRONIA_CLOCK = 20;

    private static async Task Main()
    {
        ServerSideBerkeley servidor = new(DateTime.Now, GetDessincroniaRandom(), TEMPO_SINCRONIA_CLOCK);

        var clientes = Enumerable.Range(0, 10)
                                 .Select(x => new ClientSideBerkeley(DateTime.Now, GetDessincroniaRandom()))
                                 .ToList();

        PeriodicTimer periodicTimer = new(TimeSpan.FromSeconds(1));
        while (await periodicTimer.WaitForNextTickAsync()) { }
    }

    private static long GetDessincroniaRandom() => Random.Shared.NextInt64(-60, 60);
}