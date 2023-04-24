using Client;
using Server;

internal class Program
{
    private const int TEMPO_SINCRONIA_CLOCK = 20;

    private static void Main(string[] args)
    {
        ServerSideBerkeley servidor = new(DateTime.Now, GetDessincroniaRandom(), TEMPO_SINCRONIA_CLOCK);

        List<ClientSideBerkeley> clientes = new();
        for (int i = 0; i < 10; i++)
        {
            ClientSideBerkeley cliente = new(DateTime.Now, GetDessincroniaRandom());
            clientes.Add(cliente);
        }

        while (true)
        {
            Thread.Sleep(1000);
        }
    }

    private static long GetDessincroniaRandom()
    {
        return Random.Shared.NextInt64(-60, 60);
    }
}