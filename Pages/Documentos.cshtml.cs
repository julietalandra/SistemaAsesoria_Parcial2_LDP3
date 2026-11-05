using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AsesoriaLegalGlobal_Parcial2.Pages;

public class DocumentosModel : PageModel
{
    private readonly IWebHostEnvironment _env;

    private const long LimiteBytes = 5242880; // 5 MB

    private static readonly string[] ExtensionesPermitidas =
        { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };

    public DocumentosModel(IWebHostEnvironment env)
    {
        _env = env;
    }

    [BindProperty]
    public IFormFile? Archivo { get; set; }

    public string? Usuario { get; set; }

    public string? Mensaje { get; set; }

    public List<string> Documentos { get; set; } = new();

    public IActionResult OnGet()
    {
        Usuario = HttpContext.Session.GetString("usuario");

        if (Usuario is null)
        {
            TempData["Aviso"] = "Necesitás iniciar sesión para acceder a tus documentos";
            return RedirectToPage("/Login");
        }

        CargarDocumentos();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Usuario = HttpContext.Session.GetString("usuario");
        if (Usuario is null)
        {
            return RedirectToPage("/Login");
        }

        if (Archivo is null || Archivo.Length <= 0)
        {
            ModelState.AddModelError(nameof(Archivo), "Debe elegir un documento");
        }
        else
        {
            string extension = Path.GetExtension(Archivo.FileName).ToLowerInvariant();

            if (!ExtensionesPermitidas.Contains(extension))
            {
                ModelState.AddModelError(nameof(Archivo),
                    "Solo se aceptan archivos .pdf, .doc, .docx, .jpg, .jpeg o .png");
            }

            if (Archivo.Length > LimiteBytes)
            {
                ModelState.AddModelError(nameof(Archivo),
                    "El documento no puede superar los 5 MB");
            }
        }

        if (!ModelState.IsValid)
        {
            CargarDocumentos();
            return Page();
        }

        string carpeta = Path.Combine(_env.WebRootPath, "documentos", Usuario);
        Directory.CreateDirectory(carpeta);   

        string nombreArchivo = Path.GetFileName(Archivo!.FileName);
        string ruta = Path.Combine(carpeta, nombreArchivo);

        if (System.IO.File.Exists(ruta))
        {
            ModelState.AddModelError(nameof(Archivo), "Ya existe un documento con ese nombre");
            CargarDocumentos();
            return Page();
        }

        using (FileStream destino = new FileStream(ruta, FileMode.Create))
        {
            await Archivo.CopyToAsync(destino);
        }

        Mensaje = "Documento subido correctamente";
        CargarDocumentos();
        return Page();
    }

    public IActionResult OnPostEliminar(string nombre)
    {
        Usuario = HttpContext.Session.GetString("usuario");
        if (Usuario is null)
        {
            return RedirectToPage("/Login");
        }

        string ruta = Path.Combine(_env.WebRootPath, "documentos", Usuario, Path.GetFileName(nombre));

        if (System.IO.File.Exists(ruta))
        {
            System.IO.File.Delete(ruta);
            Mensaje = "Documento eliminado";
        }

        CargarDocumentos();
        return Page();
    }

    private void CargarDocumentos()
    {
        Documentos = new List<string>();
        string carpeta = Path.Combine(_env.WebRootPath, "documentos", Usuario!);

        if (Directory.Exists(carpeta))
        {
            Documentos = Directory.GetFiles(carpeta)
                .Select(ruta => Path.GetFileName(ruta))
                .OrderBy(nombre => nombre)
                .ToList();
        }
    }
}
