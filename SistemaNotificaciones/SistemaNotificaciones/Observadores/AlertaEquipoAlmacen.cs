using System;
using SistemaNotificaciones.Abstracciones;
using SistemaNotificaciones.Dominio;

namespace SistemaNotificaciones.Observadores
{
    /// <summary>
    /// Observador especializado que procesa únicamente eventos del módulo de productos
    /// para coordinar el espacio y la logística del equipo de almacén.
    /// </summary>
    public class AlertaEquipoAlmacen : IObservador<Producto>
    {
        private readonly string _nombreBodega;

        /// <summary>
        /// Inicializa el observador para un centro de distribución o bodega específica.
        /// </summary>
        /// <param name="nombreBodega">Nombre identificativo del almacén o bodega.</param>
        public AlertaEquipoAlmacen(string nombreBodega = "Bodega Central")
        {
            _nombreBodega = nombreBodega;
        }

        /// <summary>
        /// Alerta al personal de bodega para preparar la recepción física del nuevo producto.
        /// </summary>
        /// <param name="datos">Producto registrado en catálogo.</param>
        public void actualizar(Producto datos)
        {
            if (datos == null) return;

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"[ALMACEN - {_nombreBodega}] Notificación de recepción:");
            Console.WriteLine($"                          Preparar bahía para: '{datos.nombre}'");
            Console.WriteLine($"                          Espacio estimado para: {datos.stock} unidades.");
            Console.ResetColor();
        }
    }
}