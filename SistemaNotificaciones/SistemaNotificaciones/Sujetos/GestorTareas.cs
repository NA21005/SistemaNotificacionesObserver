using SistemaNotificaciones.Dominio;

namespace SistemaNotificaciones.Sujetos
{
    /// <summary>
    /// Sujeto concreto (Subject) del patrón Observer para el módulo de tareas.
    /// Administra la colección de tareas, asigna los identificadores y notifica a los
    /// observadores cada vez que se crea una tarea. La gestión de suscripciones se hereda
    /// de <see cref="SujetoBase{T}"/>.
    /// </summary>
    public class GestorTareas : SujetoBase<Tarea>
    {
        private readonly List<Tarea> _tareas = new List<Tarea>();
        private int _siguienteId = 1;

        /// <summary>
        /// Vista de solo lectura de las tareas registradas.
        /// </summary>
        public IReadOnlyList<Tarea> tareas => _tareas.AsReadOnly();

        /// <summary>
        /// Registra una nueva tarea, le asigna un id y notifica a los observadores.
        /// </summary>
        /// <param name="t">Tarea a crear.</param>
        /// <returns>La tarea registrada con su id asignado.</returns>
        /// <exception cref="ArgumentNullException">Si la tarea es nula.</exception>
        public Tarea crearTarea(Tarea t)
        {
            ArgumentNullException.ThrowIfNull(t);

            t.id = _siguienteId++;
            _tareas.Add(t);
            notificar(t);
            return t;
        }
    }
}
