namespace Agregator.API;

/// <summary>
/// Nastavení pro úlohu na pozadí.
/// </summary>
public class BackgroundTaskOptions
{
    /// <summary>Jak dlouho počkat.</summary>
    public int GracePeriodTime { get; set; }

    /// <summary>Jak často aktualizovat.</summary>
    public int CheckUpdateTime { get; set; }
}
