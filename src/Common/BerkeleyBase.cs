namespace Common;

public abstract class BerkeleyBase
{
    protected const int PORTA = 6969;

    protected DateTime _clock;

    protected BerkeleyBase(DateTime clockInicial, long dessincroniaNaturalSegundos)
    {
        _clock = clockInicial;

        new Thread(async () => await ThreadClock()).Start();
        new Thread(ThreadConexaoAsync).Start();
        new Thread(async () => await ThreadDessincronia(dessincroniaNaturalSegundos)).Start();
    }

    protected abstract void ThreadConexaoAsync();

    protected async Task ThreadDessincronia(long dessincroniaNaturalSegundos)
    {
        PeriodicTimer timer = new(TimeSpan.FromSeconds(10));

        while (await timer.WaitForNextTickAsync())
            _clock = _clock.AddSeconds(dessincroniaNaturalSegundos);
    }

    protected void AjustaHorario(long ticksOffset)
    {
        DateTime antes = _clock;
        _clock = _clock.AddTicks(ticksOffset);
        Console.WriteLine($"Horário ajustado. Antes: {antes} - Depois: {_clock}");
    }

    private async Task ThreadClock()
    {
        const int INCREMENTO = 500;
        PeriodicTimer timer = new(TimeSpan.FromMilliseconds(INCREMENTO));

        while (await timer.WaitForNextTickAsync())
            _clock = _clock.AddMilliseconds(INCREMENTO);
    }
}
