using System;

namespace SistemaNotificaciones.Dominio
{
    /// <summary>
    /// Niveles de prioridad disponibles para una <see cref="Tarea"/>.
    /// </summary>
    public enum Prioridad
    {
        Baja = 1,
        Media = 2,
        Alta = 3
    }

    /// <summary>
    /// Entidad de dominio que representa una tarea del sistema.
    /// Es un objeto de datos simple (sin lógica de notificación), de modo que
    /// puede reutilizarse en cualquier otro módulo sin arrastrar dependencias.
    /// </summary>
    public class Tarea
    {
        /// <summary>Identificador único de la tarea (asignado por el gestor).</summary>
        public int id { get; internal set; }

        /// <summary>Título corto de la tarea.</summary>
        public string titulo { get; set; }

        /// <summary>Descripción o detalle de la tarea.</summary>
        public string detalle { get; set; }

        /// <summary>Prioridad de la tarea.</summary>
        public Prioridad prioridad { get; set; }

        /// <summary>
        /// Crea una nueva tarea validando que el título no esté vacío.
        /// </summary>
        /// <param name="titulo">Título de la tarea (obligatorio).</param>
        /// <param name="detalle">Detalle de la tarea (opcional).</param>
        /// <param name="prioridad">Prioridad de la tarea; por defecto Media.</param>
        /// <exception cref="ArgumentException">Si el título es nulo o vacío.</exception>
        public Tarea(string titulo, string detalle = "", Prioridad prioridad = Prioridad.Media)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                throw new ArgumentException("El título de la tarea no puede estar vacío.", nameof(titulo));
            }

            this.titulo = titulo.Trim();
            this.detalle = detalle ?? string.Empty;
            this.prioridad = prioridad;
        }

        /// <summary>
        /// Representación legible de la tarea, útil para los observadores
        /// que imprimen en consola.
        /// </summary>
        public override string ToString()
        {
            return $"[Tarea #{id}] {titulo} (Prioridad: {prioridad}) - {detalle}";
        }
    }
}
