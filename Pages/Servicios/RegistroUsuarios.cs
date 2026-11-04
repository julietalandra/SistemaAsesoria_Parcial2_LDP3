namespace AsesoriaLegalGlobal_Parcial2.Servicios;

// Equivalente al Application["x"] de Web Forms (guia 09, seccion 5):
// se registra como singleton, asi que hay UNA sola instancia para todos los usuarios.
// Guarda en memoria los usuarios que se registraron, para poder validar el login.
public class RegistroUsuarios
{
    // La clave es el nombre de usuario (sin distinguir mayusculas); el valor, el usuario tal
    // como se registro y su clave
    private readonly Dictionary<string, (string Usuario, string Clave)> _usuarios =
        new(StringComparer.OrdinalIgnoreCase);

    // Varias personas pueden usar el sitio a la vez: se protege el acceso a la lista
    private readonly object _bloqueo = new object();

    // Devuelve false si ya hay alguien registrado con ese nombre de usuario
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

    // Si usuario y clave coinciden devuelve el nombre de usuario registrado; si no, null
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