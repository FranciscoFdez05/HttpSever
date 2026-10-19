using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace HttpServer.Core;

/// <summary>
/// Dirección IPv4 local junto con la interfaz de red a la que pertenece.
/// </summary>
public sealed record DireccionLocal(IPAddress Direccion, string Interfaz, bool TienePuertaEnlace)
{
    public override string ToString() => $"{Direccion} · {Interfaz}";
}

public static class UtilidadesRed
{
    /// <summary>
    /// Devuelve las direcciones IPv4 utilizables de este equipo, ordenadas de más a menos
    /// probable para ser alcanzadas desde otro equipo de la red local: primero las privadas
    /// de interfaces con puerta de enlace, después el resto de privadas y por último las demás.
    /// Siempre incluye 127.0.0.1 al final.
    /// </summary>
    public static IReadOnlyList<DireccionLocal> ObtenerDireccionesLocales()
    {
        List<DireccionLocal> direcciones = [];

        try
        {
            foreach (NetworkInterface interfaz in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (interfaz.OperationalStatus != OperationalStatus.Up ||
                    interfaz.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                    interfaz.NetworkInterfaceType == NetworkInterfaceType.Tunnel)
                {
                    continue;
                }

                IPInterfaceProperties propiedades = interfaz.GetIPProperties();
                bool tienePuertaEnlace = propiedades.GatewayAddresses.Any(g =>
                    g.Address.AddressFamily == AddressFamily.InterNetwork && !g.Address.Equals(IPAddress.Any));

                foreach (UnicastIPAddressInformation unicast in propiedades.UnicastAddresses)
                {
                    IPAddress ip = unicast.Address;
                    if (ip.AddressFamily != AddressFamily.InterNetwork || EsEnlaceLocal(ip))
                    {
                        continue;
                    }

                    direcciones.Add(new DireccionLocal(ip, interfaz.Name, tienePuertaEnlace));
                }
            }
        }
        catch (NetworkInformationException)
        {
            // Sin información de red: se devuelve al menos la dirección de loopback.
        }

        List<DireccionLocal> ordenadas = direcciones
            .OrderByDescending(d => EsPrivada(d.Direccion) && d.TienePuertaEnlace)
            .ThenByDescending(d => EsPrivada(d.Direccion))
            .ToList();

        ordenadas.Add(new DireccionLocal(IPAddress.Loopback, "Loopback", false));
        return ordenadas;
    }

    /// <summary>
    /// Indica si la dirección pertenece a un rango privado RFC 1918
    /// (10.0.0.0/8, 172.16.0.0/12 o 192.168.0.0/16).
    /// </summary>
    public static bool EsPrivada(IPAddress ip)
    {
        if (ip.AddressFamily != AddressFamily.InterNetwork)
        {
            return false;
        }

        byte[] b = ip.GetAddressBytes();
        return b[0] == 10 ||
               (b[0] == 172 && b[1] >= 16 && b[1] <= 31) ||
               (b[0] == 192 && b[1] == 168);
    }

    private static bool EsEnlaceLocal(IPAddress ip)
    {
        byte[] b = ip.GetAddressBytes();
        return b[0] == 169 && b[1] == 254;
    }

    /// <summary>
    /// Formatea un número de bytes en una unidad legible (B, KB, MB, GB, TB).
    /// </summary>
    public static string FormatearTamano(long bytes)
    {
        string[] unidades = ["B", "KB", "MB", "GB", "TB"];
        double valor = bytes;
        int unidad = 0;

        while (valor >= 1024 && unidad < unidades.Length - 1)
        {
            valor /= 1024;
            unidad++;
        }

        return unidad == 0 ? $"{bytes} B" : $"{valor:0.##} {unidades[unidad]}";
    }
}
