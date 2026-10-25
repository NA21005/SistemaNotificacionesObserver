using SistemaNotificaciones.Abstracciones;

namespace SistemaNotificaciones.Sujetos
{
    /// <summary>
    /// Implementación reutilizable del rol de Sujeto del patrón Observer.
    /// Centraliza la lista de observadores y la difusión de eventos, de modo que los
    /// gestores concretos solo se ocupan de su lógica de negocio.
    /// </summary>
    /// <typeparam name="T">Tipo de dato que se transmite a los observadores.</typeparam>
    public abstract class SujetoBase<T> : ISujeto<T> where T : class
    {
        private readonly List<IObservador<T>> _observadores = new List<IObservador<T>>();

        /// <summary>
        /// Cantidad de observadores suscritos actualmente.
        /// </summary>
        public int totalObservadores => _observadores.Count;

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Si el observador es nulo.</exception>
        public void suscribir(IObservador<T> obs)
        {
            ArgumentNullException.ThrowIfNull(obs);

            if (!_observadores.Contains(obs))
            {
                _observadores.Add(obs);
            }
        }

        /// <inheritdoc/>
        public void desuscribir(IObservador<T> obs)
        {
            if (obs != null)
            {
                _observadores.Remove(obs);
            }
        }

        /// <inheritdoc/>
        public bool estaSuscrito(IObservador<T> obs)
        {
            return obs != null && _observadores.Contains(obs);
        }

        /// <summary>
        /// Notifica a todos los observadores suscritos. Se recorre una copia de la lista para que
        /// un observador pueda desuscribirse durante la notificación sin romper la iteración.
        /// Si un observador falla, el error se informa y los demás siguen recibiendo el evento.
        /// </summary>
        /// <param name="datos">Entidad que originó el evento.</param>
        public void notificar(T datos)
        {
            ArgumentNullException.ThrowIfNull(datos);

            foreach (IObservador<T> obs in _observadores.ToArray())
            {
                try
                {
                    obs.actualizar(datos);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[ERROR] {obs.GetType().Name} falló al procesar la notificación: {ex.Message}");
                }
            }
        }
    }
}
