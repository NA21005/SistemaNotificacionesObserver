using System;
using SistemaNotificaciones.Abstracciones;
using SistemaNotificaciones.Dominio;

namespace SistemaNotificaciones.Observadores
{
    /// <summary>
    /// Observador concreto encargado de simular el envío de notificaciones vía correo electrónico.
    /// Es polimórfico y reutilizable tanto para eventos de productos como de tareas.
    /// </summary>
    public class NotificadorEmail : IObservador<Producto>, IObservador<Tarea>
    {
        private readonly string _correoDestinatario;

        /// <summary>
        /// Inicializa el observador con el correo destino de las notificaciones.
        /// </summary>
        /// <param name="correoDestinatario">Dirección de correo a la que se envían los avisos.</param>
        public NotificadorEmail(string correoDestinatario = "usuario@sistema.com")
        {
            _correoDestinatario = correoDestinatario;
        }

        /// <summary>
        /// Procesa la notificación cuando se agrega un nuevo producto al catálogo.
        /// </summary>
        /// <param name="datos">Entidad producto recibida desde el sujeto.</param>
        public void actualizar(Producto datos)
        {
            if (datos == null) return;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[EMAIL] Enviando aviso a {_correoDestinatario}...");
            Console.WriteLine($"        Nuevo Producto Disponible: '{datos.nombre}'");
            Console.WriteLine($"        Precio: ${datos.precio:F2} | Stock Inicial: {datos.stock} uds.");
            Console.ResetColor();
        }

        /// <summary>
        /// Procesa la notificación cuando se crea una nueva tarea en el sistema.
        /// </summary>
        /// <param name="datos">Entidad tarea recibida desde el sujeto.</param>
        public void actualizar(Tarea datos)
        {
            if (datos == null) return;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[EMAIL] Enviando aviso a {_correoDestinatario}...");
            Console.WriteLine($"        Nueva Tarea Asignada: [#{datos.id}] '{datos.titulo}'");
            Console.WriteLine($"        Prioridad: {datos.prioridad} | Detalle: {datos.detalle}");
            Console.ResetColor();
        }
    }
}