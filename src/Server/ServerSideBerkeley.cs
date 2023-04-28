using Common;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace Server;

public class ServerSideBerkeley : BerkeleyBase
{
    private readonly ConcurrentDictionary<Socket, long> _clientes = new();
    private readonly PeriodicTimer _timerSincronia;

    public ServerSideBerkeley(DateTime clockInicial, long dessincroniaNatural, int tempoSincronizacao) : base(clockInicial, dessincroniaNatural)
    {
        Console.WriteLine($"Inicializando server: {clockInicial} - Dessincronia: {(dessincroniaNatural >= 0 ? '+' : "")}{dessincroniaNatural}s - Tempo de sincronia: {tempoSincronizacao}s");
        _timerSincronia = new PeriodicTimer(TimeSpan.FromSeconds(tempoSincronizacao));
        new Thread(async () => await ThreadSincronia()).Start();
    }

    protected override async void ThreadConexaoAsync()
    {
        TcpListener listenSocket = null;
        try
        {
            // cria um socket TCP para pedidos de conexão
            listenSocket = new(IPAddress.Any, PORTA);
            listenSocket.Start();

            PeriodicTimer timer = new(TimeSpan.FromMilliseconds(1));
            while (await timer.WaitForNextTickAsync())
            {
                // aguarda ate um cliente pedir por uma conexao
                Socket socket = await listenSocket.AcceptSocketAsync();

                await Task.Run(async () =>
                {
                    Mensagem mensagemEntrada = await ConexaoHelper.ReceiveMessageAsync(socket);
                    switch (mensagemEntrada.Tipo)
                    {
                        case TipoMensagem.ConexaoInicial:
                        {
                            _clientes.TryAdd(socket, DateTime.MinValue.Ticks);
                        }
                        break;
                    }
                });
            }
        }
        finally
        {
            listenSocket.Stop();
        }
    }

    private async Task ThreadSincronia()
    {
        while (await _timerSincronia.WaitForNextTickAsync())
        {
            Console.WriteLine("\nServidor executando sincronização");
            foreach (var item in _clientes)
            {
                _clientes[item.Key] = DateTime.MinValue.Ticks;
                Mensagem mensagem = new(0, TipoMensagem.ClockSolicitacao);
                await ConexaoHelper.SendMessageAsync(item.Key, mensagem.ToString());
                Mensagem resposta = await ConexaoHelper.ReceiveMessageAsync(item.Key);
                if (resposta.Tipo == TipoMensagem.ClockResposta)
                    _clientes[item.Key] = resposta.Ticks;
            }

            long mediaTicks = (_clientes.Values.Sum() + _clock.Ticks) / (_clientes.Count + 1);
            Console.WriteLine($"\nMédia de horário: {new DateTime(mediaTicks)}");

            AjustaHorario(mediaTicks - _clock.Ticks);

            foreach (var item in _clientes)
            {
                long ticks = mediaTicks - item.Value;
                Mensagem mensagem = new(ticks, TipoMensagem.Ajuste);
                await ConexaoHelper.SendMessageAsync(item.Key, mensagem.ToString());
            }
        }
    }
}
