// Espacio de nombres raiz del dominio. Todos los tipos del nucleo viven aqui.
namespace McpVentas.Domain;

/// <summary>
/// Formas de pago soportadas por el POS. Cada valor corresponde a una columna
/// monetaria de la tabla <c>venta</c> en la base de datos <c>pos_caja</c>.
/// Este enum es la representacion de dominio (agnostica de persistencia) y se
/// usa para parametrizar las consultas y presentar los desgloses al cliente MCP.
/// </summary>
public enum FormaPago
{
    /// <summary>Pago en efectivo. Columna de base de datos: <c>pago_efectivo</c>.</summary>
    Efectivo = 0,

    /// <summary>Pago con cheque. Columna de base de datos: <c>pago_cheque</c>.</summary>
    Cheque = 1,

    /// <summary>Pago con vales de despensa. Columna de base de datos: <c>pago_vales</c>.</summary>
    Vales = 2,

    /// <summary>Pago con tarjeta de credito. Columna de base de datos: <c>pago_tc</c>.</summary>
    TarjetaCredito = 3,

    /// <summary>Pago con tarjeta de debito. Columna de base de datos: <c>pago_td</c>.</summary>
    TarjetaDebito = 4,

    /// <summary>Pago por transferencia SPEI. Columna de base de datos: <c>pago_spei</c>.</summary>
    Spei = 5
}
