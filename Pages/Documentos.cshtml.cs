using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AsesoriaLegalGlobal_Parcial2.Pages;

public class DocumentosModel : PageModel
{
    // El Server del apunte: la clase que sabe dónde vive el sitio en el disco
    private readonly IWebHostEnvironment _env;

    private const long LimiteBytes = 5242880; // 5 MB

    private static readonly string[] ExtensionesPermitidas =
        { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };

    public DocumentosModel(IWebHostEnvironment env)
    {
        _env = env;
    }

    // El control FileUpload del apunte, ahora como propiedad del formulario
    [BindProperty]
    public IFormFile? Archivo { get; set; }

    // Usuario tomado de la SESIÓN (no del formulario)
    public string? Usuario { get; set; }

    public string? Mensaje { get; set; }

    // Los documentos que ya subió este usuario
    public List<string> Documentos { get; set; } = new();

    public IActionResult OnGet()
    {
        Usuario = HttpContext.Session.GetString("usuario");

        // Sin sesión activa no se entra: se vuelve al registro
        if (Usuario is null)
        {
            return RedirectToPage("/Registro");
        }

        CargarDocumentos();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // La sesión se controla también al subir, no solo al entrar a la página
        Usuario = HttpContext.Session.GetString("usuario");
        if (Usuario is null)
        {
            return RedirectToPage("/Registro");
        }

        // Validaciones del archivo
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

        // El Server.MapPath(".") del apunte es _env.WebRootPath.
        // Cada usuario tiene su propia carpeta: wwwroot/documentos/<usuario>
        string carpeta = Path.Combine(_env.WebRootPath, "documentos", Usuario);
        Directory.CreateDirectory(carpeta);   // si ya existe, no hace nada

        // Path.GetFileName: descarta cualquier ruta que venga adentro del nombre
        string nombreArchivo = Path.GetFileName(Archivo!.FileName);
        string ruta = Path.Combine(carpeta, nombreArchivo);

        // El File.Exists del apunte: no pisar un archivo existente
        if (System.IO.File.Exists(ruta))
        {
            ModelState.AddModelError(nameof(Archivo), "Ya existe un documento con ese nombre");
            CargarDocumentos();
            return Page();
        }

        // El SaveAs del apunte: copiar el contenido al disco
        using (FileStream destino = new FileStream(ruta, FileMode.Create))
        {
            await Archivo.CopyToAsync(destino);
        }

        Mensaje = "Documento subido correctamente";
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
