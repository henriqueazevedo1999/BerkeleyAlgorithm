using System.Net.Sockets;
using System.Text.Json;

namespace Common;

public static class ConexaoHelper
{
    public static async Task SendMessageAsync(Socket socket, string message)
    {
        using StreamWriter stream = new(new NetworkStream(socket));
        await stream.WriteLineAsync(message);
        await stream.FlushAsync();
    }

    public static async Task<Mensagem> ReceiveMessageAsync(Socket socket)
    {
        using StreamReader streamEntrada = new(new NetworkStream(socket));
        string message = await streamEntrada.ReadLineAsync();
        return JsonSerializer.Deserialize<Mensagem>(message);
    }
}
