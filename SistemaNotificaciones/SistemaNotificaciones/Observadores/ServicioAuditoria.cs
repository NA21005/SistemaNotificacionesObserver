using System;
using SistemaNotificaciones.Abstracciones;
using SistemaNotificaciones.Dominio;

namespace SistemaNotificaciones.Observadores
{
    /// <summary>
    /// Observador concreto encargado del registro histórico y auditoría del sistema.
    /// Registra en bitácora la adición de productos y la creación de tareas con marca temporal.
    /// </summary>
    public class ServicioAuditoria : IObservador<Producto>, IObservador<Tarea>
    {
        private int _totalRegistros;

        public ServicioAuditoria()
        {
            _totalRegistros = 0;
        }

        /// <summary>
        /// Registra en la bitácora el evento de un nuevo producto agregado.
        /// </summary>
        /// <param name="datos">Entidad producto a auditar.</param>
        public void actualizar(Producto datos)
        {
            if (datos == null) return;

            _totalRegistros++;
            string marcaTiempo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"[AUDITORIA #{_totalRegistros}] [{marcaTiempo}] Evento: ALTA_PRODUCTO");
            Console.WriteLine($"                      ID: {datos.id} | Nombre: \"{datos.nombre}\" | Valor: ${datos.precio:F2}");
            Console.ResetColor();
        }

        /// <summary>
        /// Registra en la bitácora el evento de una nueva tarea creada.
        /// </summary>
        /// <param name="datos">Entidad tarea a auditar.</param>
        public void actualizar(Tarea datos)
        {
            if (datos == null) return;

            _totalRegistros++;
            string marcaTiempo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"[AUDITORIA #{_totalRegistros}] [{marcaTiempo}] Evento: CREACION_TAREA");
            Console.WriteLine($"                      ID: {datos.id} | Titulo: \"{datos.titulo}\" | Nivel: {datos.prioridad}");
            Console.ResetColor();
        }
    }
}