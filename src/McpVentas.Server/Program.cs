// Importa la capa de infraestructura (registro de dependencias y persistencia).
using McpVentas.Infrastructure;
// Importa las clases de opciones tipadas.
using McpVentas.Infrastructure.Options;
// Importa el tipo Implementation usado para identificar el servidor ante el cliente MCP.
using ModelContextProtocol.Protocol;

// Crea el "builder" de la aplicacion web (ASP.NET Core) leyendo argumentos y configuracion.
var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// CONFIGURACION
// ---------------------------------------------------------------------------
// Agrega una fuente de configuracion basada en variables de entorno con prefijo
// "MCP_". El separador "__" (doble guion bajo) representa jerarquia de secciones.
// Ejemplo: MCP_Database__ConnectionString -> Database:ConnectionString.
builder.Configuration.AddEnvironmentVariables(prefix: "MCP_");

// Lee las opciones del servidor MCP (URL, hosts, instrucciones) desde la configuracion.
var serverOptions = builder.Configuration
    .GetSection(ServerOptions.SectionName)
    .Get<ServerOptions>() ?? new ServerOptions();

// Aplica la lista de hosts permitidos al filtrado de host de Kestrel (anti DNS rebinding).
if (!string.IsNullOrWhiteSpace(serverOptions.AllowedHosts))
{
    // La clave "AllowedHosts" es la que consume el middleware de host filtering.
    builder.Configuration["AllowedHosts"] = serverOptions.AllowedHosts;
}

// ---------------------------------------------------------------------------
// SERVICIOS (Dependency Injection)
// ---------------------------------------------------------------------------
// Registra DbContext de EF Core, opciones y repositorios de la capa Infrastructure
// (que a su vez registra los casos de uso de la capa Application).
builder.Services.AddInfrastructure(builder.Configuration);

// Registra el servidor MCP y su transporte HTTP.
builder.Services
    .AddMcpServer(options =>
    {
        // Identifica el servidor ante el cliente MCP (nombre y version).
        options.ServerInfo = new Implementation
        {
            Name = "McpVentas",
            Version = "1.0.0"
        };
        // Instrucciones iniciales que orientan al LLM sobre el uso de las herramientas.
        if (!string.IsNullOrWhiteSpace(serverOptions.Instructions))
        {
            options.ServerInstructions = serverOptions.Instructions;
        }
    })
    // Habilita el transporte HTTP (Streamable HTTP). Modo sin sesion recomendado
    // para servidores que solo exponen tools (sin notificaciones servidor->cliente).
    .WithHttpTransport(options =>
    {
        options.Stateless = true;
    })
    // Descubre y registra AUTOMATICAMENTE todas las herramientas MCP del ensamblado
    // (clases marcadas con [McpServerToolType] y metodos con [McpServerTool]).
    .WithToolsFromAssembly();

// ---------------------------------------------------------------------------
// PIPELINE HTTP
// ---------------------------------------------------------------------------
// Construye la aplicacion con todos los servicios registrados.
var app = builder.Build();

// Mapea el endpoint del protocolo MCP (por defecto en la ruta raiz "/").
app.MapMcp();

// Endpoint de salud/sondeo simple (util para verificar que el servidor esta vivo).
app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    server = "McpVentas",
    transport = "streamable-http"
}));

// Arranca el servidor escuchando en la URL configurada.
app.Run(serverOptions.Url);
