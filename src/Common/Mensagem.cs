using System.Text.Json;

namespace Common;

public enum TipoMensagem
{
    ConexaoInicial,
    ClockSolicitacao,
    ClockResposta,
    Ajuste,
}

public record Mensagem
{
    public Mensagem(long ticks, TipoMensagem tipo)
    {
        Ticks = ticks;
        Tipo = tipo;
    }

    public long Ticks { get; set; }
    public TipoMensagem Tipo { get; set; }

    public override string ToString()
    {
        JsonSerializerOptions options = new();
        options.WriteIndented = false;
        
        return JsonSerializer.Serialize(this, options);
    }
}
