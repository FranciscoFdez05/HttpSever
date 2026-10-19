namespace HttpServer.Core;

/// <summary>
/// Severidad o categoría de una entrada del registro.
/// </summary>
public enum NivelRegistro
{
    Info,
    Peticion,
    Exito,
    Advertencia,
    Error
}

/// <summary>
/// Entrada de registro emitida por el servidor.
/// </summary>
public sealed class RegistroEventArgs(NivelRegistro nivel, string mensaje) : EventArgs
{
    public DateTime Fecha { get; } = DateTime.Now;

    public NivelRegistro Nivel { get; } = nivel;

    public string Mensaje { get; } = mensaje;
}
