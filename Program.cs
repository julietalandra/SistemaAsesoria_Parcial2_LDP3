using AsesoriaLegalGlobal_Parcial2.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages + mensajes en espanol cuando el dato no se puede convertir
// (por ejemplo, si escriben letras en la edad).
// Los acentos van como codigos unicode para que el archivo no dependa de la codificacion.
builder.Services.AddRazorPages().AddMvcOptions(opciones =>
{
    opciones.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(
        campo => $"El campo {campo} debe ser un valor num\u00e9rico");
    opciones.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor(
        (valor, campo) => $"'{valor}' no es un valor v\u00e1lido para {campo}");
});

// La sesion necesita un lugar donde guardar los datos (en memoria del servidor)
builder.Services.AddDistributedMemoryCache();

// Variable de aplicacion (guia 09, seccion 5): UNA sola instancia para todo el sitio
builder.Services.AddSingleton<RegistroUsuarios>();

// El apunte: this.Session.Timeout = 20;
builder.Services.AddSession(opciones =>
{
    opciones.IdleTimeout = TimeSpan.FromMinutes(20);
    opciones.Cookie.HttpOnly = true;
    opciones.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();          // DESPUES de UseRouting y ANTES de MapRazorPages
app.UseAuthorization();
app.MapRazorPages();

app.Run();