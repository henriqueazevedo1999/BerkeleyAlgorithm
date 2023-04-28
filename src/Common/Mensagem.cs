using System.Text.Json;

namespace Common;

public enum TipoMensagem
{
    ConexaoInicial,
    ClockSolicitacao,
    ClockResposta,
    Ajuste,
}

public record Mensagem(long Ticks, TipoMensagem Tipo)
{
    public override string ToString()
    {
        JsonSerializerOptions options = new();
        options.WriteIndented = false;
        
        return JsonSerializer.Serialize(this, options);
    }
}
