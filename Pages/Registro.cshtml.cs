using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AsesoriaLegalGlobal_Parcial2.Pages;

public class RegistroModel : PageModel
{
    // RegularExpressionValidator del apunte: formato del correo
    [Required(ErrorMessage = "Debe ingresar el correo electrónico")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        ErrorMessage = "El correo electrónico no tiene un formato válido")]
    [BindProperty]
    public string? Correo { get; set; }

    // El usuario se usa después como nombre de carpeta: solo letras, números y guion bajo
    [Required(ErrorMessage = "Debe ingresar el nombre de usuario")]
    [RegularExpression(@"^[A-Za-z0-9_]{3,20}$",
        ErrorMessage = "El usuario debe tener de 3 a 20 caracteres: letras, números o guion bajo")]
    [BindProperty]
    public string? NombreUsuario { get; set; }

    [Required(ErrorMessage = "Debe ingresar el nombre")]
    [BindProperty]
    public string? Nombre { get; set; }

    [Required(ErrorMessage = "Debe ingresar el apellido")]
    [BindProperty]
    public string? Apellido { get; set; }

    // RangeValidator del apunte. int? para que [Required] avise cuando queda vacío.
    // Mayores de 15 años = de 16 en adelante (los límites del Range son inclusivos).
    // Si escriben letras, el binding rechaza el valor con el mensaje de Program.cs.
    [Required(ErrorMessage = "Debe ingresar la edad")]
    [Range(16, 120, ErrorMessage = "Debe ser mayor de 15 años (se acepta de 16 a 120)")]
    [BindProperty]
    public int? Edad { get; set; }

    [Required(ErrorMessage = "Debe ingresar la contraseña")]
    [DataType(DataType.Password)]
    [BindProperty]
    public string? Clave { get; set; }

    // CompareValidator del apunte: Operator = Equal contra la primera clave
    [Required(ErrorMessage = "Debe confirmar la contraseña")]
    [Compare(nameof(Clave), ErrorMessage = "Las contraseñas no coinciden")]
    [DataType(DataType.Password)]
    [BindProperty]
    public string? ConfirmarClave { get; set; }

    public void OnGet()
    {
    }

    // El Button1_Click "Registrar" del apunte
    public IActionResult OnPost()
    {
        // El this.IsValid del WebForm
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Variable de SESIÓN (vive en el servidor): el nombre de usuario
        HttpContext.Session.SetString("usuario", NombreUsuario!);

        // COOKIE (vive en el cliente): la contraseña, como pide el enunciado
        var opciones = new CookieOptions
        {
            Expires = new DateTimeOffset(DateTime.Now.AddDays(7)),
            HttpOnly = true,
            IsEssential = true
        };
        Response.Cookies.Append("clave", Clave!, opciones);

        // "Al iniciar sesión, el usuario es dirigido a su espacio de gestión de archivos"
        return RedirectToPage("/Documentos");
    }
}
