using AsesoriaLegalGlobal_Parcial2.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages().AddMvcOptions(opciones =>
{
    opciones.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(
        campo => $"El campo {campo} debe ser un valor num\u00e9rico");
    opciones.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor(
        (valor, campo) => $"'{valor}' no es un valor v\u00e1lido para {campo}");
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSingleton<RegistroUsuarios>();

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
app.UseSession();          
app.UseAuthorization();
app.MapRazorPages();

app.Run();