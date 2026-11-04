using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AsesoriaLegalGlobal_Parcial2.Pages;

public class SalirModel : PageModel
{
    public IActionResult OnGet()
    {
        // BORRAR toda la sesión y la cookie
        HttpContext.Session.Clear();
        Response.Cookies.Delete("clave");

        return RedirectToPage("/Index");
    }
}
