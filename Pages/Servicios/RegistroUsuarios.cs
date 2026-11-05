namespace AsesoriaLegalGlobal_Parcial2.Servicios;
public class RegistroUsuarios
{
    private readonly Dictionary<string, (string Usuario, string Clave)> _usuarios =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly object _bloqueo = new object();

    public bool Registrar(string usuario, string clave)
    {
        lock (_bloqueo)
        {
            if (_usuarios.ContainsKey(usuario))
            {
                return false;
            }

            _usuarios[usuario] = (usuario, clave);
            return true;
        }
    }

    public string? Validar(string usuario, string clave)
    {
        lock (_bloqueo)
        {
            if (_usuarios.TryGetValue(usuario, out var datos) && datos.Clave.Equals(clave))
            {
                return datos.Usuario;
            }

            return null;
        }
    }
}