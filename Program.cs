var builder = WebApplication.CreateBuilder(args);

// Razor Pages + mensajes en español cuando el dato no se puede convertir
// (por ejemplo, si escriben letras en la edad)
builder.Services.AddRazorPages().AddMvcOptions(opciones =>
{
    opciones.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(
        campo => $"El campo {campo} debe ser un valor numérico");
    opciones.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor(
        (valor, campo) => $"'{valor}' no es un valor válido para {campo}");
});

// La sesión necesita un lugar donde guardar los datos (en memoria del servidor)
builder.Services.AddDistributedMemoryCache();

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
app.UseSession();          // DESPUÉS de UseRouting y ANTES de MapRazorPages
app.UseAuthorization();
app.MapRazorPages();

app.Run();
