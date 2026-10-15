using System;
using System.Collections.Generic;
using SistemaNotificaciones.Abstracciones;
using SistemaNotificaciones.Dominio;

namespace SistemaNotificaciones.Sujetos
{
    /// <summary>
    /// Sujeto concreto (Subject) del patrón Observer para el módulo de tareas.
    /// Administra la colección de tareas y la lista de observadores, y les
    /// notifica cada vez que se crea una tarea nueva.
    /// </summary>
    public class GestorTareas : ISujeto<Tarea>
    {
        // Colecciones encapsuladas: solo el gestor puede modificarlas.
        private readonly List<Tarea> _tareas = new List<Tarea>();
        private readonly List<IObservador<Tarea>> _observadores = new List<IObservador<Tarea>>();
        private int _siguienteId = 1;

        /// <summary>
        /// Vista de solo lectura de las tareas registradas.
        /// </summary>
        public IReadOnlyList<Tarea> tareas => _tareas.AsReadOnly();

        /// <summary>
        /// Cantidad de observadores suscritos actualmente.
        /// </summary>
        public int totalObservadores => _observadores.Count;

        /// <summary>
        /// Registra un observador para que reciba notificaciones de tareas.
        /// Un mismo observador no se registra dos veces.
        /// </summary>
        /// <param name="obs">Observador a suscribir.</param>
        /// <exception cref="ArgumentNullException">Si el observador es nulo.</exception>
        public void suscribir(IObservador<Tarea> obs)
        {
            if (obs == null)
            {
                throw new ArgumentNullException(nameof(obs));
            }

            if (!_observadores.Contains(obs))
            {
                _observadores.Add(obs);
            }
        }

        /// <summary>
        /// Remueve un observador de la lista de difusión.
        /// </summary>
        /// <param name="obs">Observador a desuscribir.</param>
        public void desuscribir(IObservador<Tarea> obs)
        {
            _observadores.Remove(obs);
        }

        /// <summary>
        /// Notifica a todos los observadores suscritos sobre una tarea.
        /// Se recorre una copia de la lista para que un observador pueda
        /// desuscribirse durante la notificación sin romper el ciclo.
        /// </summary>
        /// <param name="datos">Tarea que originó el evento.</param>
        public void notificar(Tarea datos)
        {
            foreach (IObservador<Tarea> obs in _observadores.ToArray())
            {
                obs.actualizar(datos);
            }
        }

        /// <summary>
        /// Registra una nueva tarea, le asigna un id y notifica a los observadores.
        /// </summary>
        /// <param name="t">Tarea a crear.</param>
        /// <returns>La tarea registrada con su id asignado.</returns>
        /// <exception cref="ArgumentNullException">Si la tarea es nula.</exception>
        public Tarea crearTarea(Tarea t)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t));
            }

            t.id = _siguienteId++;
            _tareas.Add(t);
            notificar(t);
            return t;
        }
    }
}
