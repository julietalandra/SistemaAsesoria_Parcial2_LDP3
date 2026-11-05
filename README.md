# Asesoría Legal Global — Registro de clientes y gestión de documentos legales

Parcial 2 de **Laboratorio de Programación 3** 
Tecnicatura Superior en Desarrollo de Software, Instituto Superior Santo Domingo.

**Autora:** Julieta Landra ([@julietalandra](https://github.com/julietalandra))
**Profesor:** Christian Mansilla

## Descripción

Sitio web para la empresa ficticia *Asesoría Legal Global*. Permite que los clientes se registren, inicien sesión y gestionen sus documentos legales en un espacio propio. Está desarrollado con **ASP.NET Core Razor Pages (.NET 8)** y aplica los temas de la materia: Master Page (layout), validación con DataAnnotations, cookies, variables de sesión y de aplicación, y subida de archivos con `IFormFile`.

## Funcionalidades

- **Master Page (`_Layout.cshtml`):** navbar y footer comunes a todas las páginas. El menú cambia según haya o no una sesión activa.
- **Registro de clientes:** nombre, apellido, correo, nombre de usuario, edad, contraseña y confirmación.
  - Todos los campos son obligatorios.
  - Formato de correo validado con expresión regular.
  - Edad numérica y mayor de 15 años.
  - Las contraseñas deben coincidir.
  - Validación en el cliente (jQuery Unobtrusive) y en el servidor (`ModelState`).
- **Almacenamiento temporal al registrar:** el nombre de usuario se guarda en una **variable de sesión** y la contraseña en una **cookie**.
- **Inicio de sesión:** valida usuario y contraseña y lleva al usuario a su espacio de documentos.
- **Gestión de documentos** (requiere sesión activa; sin sesión redirige al login):
  - Subida de archivos con `IFormFile` e `IWebHostEnvironment`.
  - Carpeta propia por usuario: `wwwroot/documentos/<usuario>`.
  - Validación de extensión (`.pdf`, `.doc`, `.docx`, `.jpg`, `.jpeg`, `.png`), tamaño máximo de 5 MB y nombres duplicados.
  - Listado, apertura y eliminación de los documentos propios.
- **Diseño responsivo** con CSS propio y uso de las imágenes de la empresa.

## Tecnologías

- C# y ASP.NET Core Razor Pages (.NET 8)
- DataAnnotations y Tag Helpers
- Sesión (`AddSession`), cookies (`CookieOptions`) y servicio singleton
- HTML5 y CSS3 (flexbox y media queries)
- jQuery Validation (validación del lado del cliente)

## Estructura del proyecto

```
AsesoriaLegalGlobal_Parcial2/
├── Program.cs                      Configuración: sesión, servicio singleton y middleware
├── Servicios/
│   └── RegistroUsuarios.cs         Usuarios registrados, en memoria (singleton)
├── Pages/
│   ├── Shared/_Layout.cshtml       Master Page (navbar + footer)
│   ├── _ViewStart.cshtml           Aplica el layout a todo el sitio
│   ├── Index.cshtml                Inicio
│   ├── Registro.cshtml(.cs)        Formulario de registro y validaciones
│   ├── Login.cshtml(.cs)           Inicio de sesión
│   ├── Documentos.cshtml(.cs)      Espacio de gestión de documentos
│   ├── Salir.cshtml(.cs)           Cierre de sesión
│   └── Error.cshtml(.cs)           Página de error
└── wwwroot/
    ├── css/site.css                Estilos responsivos
    ├── imagenes/                   Imágenes de la empresa
    └── documentos/                 Archivos subidos (se crea automáticamente)
```

## Cómo ejecutarlo

**Requisito:** [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
git clone https://github.com/julietalandra/SistemaAsesoria_Parcial2_LDP3.git
cd SistemaAsesoria_Parcial2_LDP3
# ubicarse en la carpeta que contiene el archivo .csproj y ejecutar:
dotnet run
```

Abrir en el navegador la dirección que indique la consola (por ejemplo `https://localhost:7xxx`). También se puede abrir la solución en Visual Studio 2022 y ejecutar con **F5**.

## Flujo de uso

1. En **Registro**, completar el formulario. Al confirmar, la sesión queda iniciada y se redirige a **Documentos**.
2. Subir, abrir o eliminar documentos desde el espacio propio.
3. **Salir** cierra la sesión y borra la cookie.
4. Para volver a entrar, usar **Iniciar sesión** con el usuario y la contraseña registrados.
5. Si se intenta abrir **Documentos** sin sesión, el sitio redirige al login con un aviso.

## Decisiones técnicas

- **Cookie vs. sesión:** la cookie se guarda en el navegador del cliente y viaja en cada pedido; la sesión vive en el servidor y se identifica con una cookie de identificador. Por eso el nombre de usuario va en sesión y la contraseña, como pide la consigna, en una cookie `HttpOnly`.
- **Variable de aplicación:** los usuarios registrados se guardan en memoria en un servicio *singleton* (`RegistroUsuarios`), el equivalente de `Application["x"]` de Web Forms. Permite validar el login y evitar que dos usuarios compartan el mismo nombre (y, por lo tanto, la misma carpeta de documentos).
- **Sin base de datos:** la consigna habla de almacenamiento temporal, por lo que los datos de registro viven en memoria.
- **Subida de archivos segura:** lista blanca de extensiones, límite de tamaño, `Path.GetFileName` para descartar rutas dentro del nombre, nombre de usuario restringido a letras, números y guion bajo (se usa como nombre de carpeta), protección antifalsificación (antiforgery) en los formularios y control de sesión en cada operación.

## Limitaciones conocidas

- Los usuarios registrados y las sesiones están en memoria: **se pierden al reiniciar la aplicación** y hay que registrarse de nuevo.
- Guardar la contraseña en una cookie es un requisito de la consigna; en un sistema real no se haría (se almacenaría un hash en una base de datos y se usaría ASP.NET Core Identity).
- Los archivos están dentro de `wwwroot`, por lo que son accesibles por URL directa. En producción se guardarían fuera de `wwwroot` y se descargarían mediante un handler que verifique la sesión.