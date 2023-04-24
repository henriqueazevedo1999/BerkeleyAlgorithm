using Common;
using System.Net.Sockets;
using System.Text.Json;

namespace Client;

public class ClientSideBerkeley : BerkeleyBase
{
    public ClientSideBerkeley(DateTime clockInicial, long dessincroniaNatural) : base(clockInicial, dessincroniaNatural)
    {
        Console.WriteLine($"Inicializando client: {clockInicial} - Dessincronia: {(dessincroniaNatural >= 0 ? '+' : "")}{dessincroniaNatural}s");
    }

    protected override async void ThreadConexaoAsync()
    {
        using var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        sock.Connect("localhost", PORTA);
        {
            Mensagem mensagem = new(0, TipoMensagem.ConexaoInicial);
            await ConexaoHelper.SendMessageAsync(sock, mensagem.ToString());
        }

        using var s = new StreamReader(new NetworkStream(sock));
        while (true)
        {
            Thread.Sleep(10);
            string resposta = s.ReadLine();
            Mensagem responseMessage = JsonSerializer.Deserialize<Mensagem>(resposta);

            if (responseMessage.Tipo == TipoMensagem.ClockSolicitacao)
            {
                Mensagem mensagem = new(_clock.Ticks, TipoMensagem.ClockResposta);
                await ConexaoHelper.SendMessageAsync(sock, mensagem.ToString());
                Console.WriteLine($"Cliente informou horário: {new DateTime(mensagem.Ticks)}");
            }
            else if (responseMessage.Tipo == TipoMensagem.Ajuste)
            {
                long ticksOffset = responseMessage.Ticks;
                AjustaHorario(ticksOffset);
            }
        }
    }
}
