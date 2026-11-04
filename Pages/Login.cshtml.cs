using System.ComponentModel.DataAnnotations;
using AsesoriaLegalGlobal_Parcial2.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AsesoriaLegalGlobal_Parcial2.Pages;

public class LoginModel : PageModel
{
    private readonly RegistroUsuarios _registro;

    public LoginModel(RegistroUsuarios registro)
    {
        _registro = registro;
    }

    [Required(ErrorMessage = "Debe ingresar el nombre de usuario")]
    [BindProperty]
    public string? NombreUsuario { get; set; }

    [Required(ErrorMessage = "Debe ingresar la contraseña")]
    [DataType(DataType.Password)]
    [BindProperty]
    public string? Clave { get; set; }

    // Mensaje que deja la página Documentos cuando se entra sin sesión
    [TempData]
    public string? Aviso { get; set; }

    public IActionResult OnGet()
    {
        // Si ya hay sesión activa, directo a su espacio de documentos
        if (HttpContext.Session.GetString("usuario") is not null)
        {
            return RedirectToPage("/Documentos");
        }

        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Se consulta la variable de aplicación: devuelve el usuario o null
        string? usuario = _registro.Validar(NombreUsuario!, Clave!);

        if (usuario is null)
        {
            // Mismo mensaje si falla el usuario o la clave: no se revela cuál fue
            ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos");
            return Page();
        }

        // Inicio de sesión: el usuario queda en la variable de sesión
        HttpContext.Session.SetString("usuario", usuario);

        // "Al iniciar sesión, el usuario será dirigido a su propio espacio de gestión de archivos"
        return RedirectToPage("/Documentos");
    }
}