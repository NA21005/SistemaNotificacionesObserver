namespace SistemaNotificaciones.Abstracciones
{
    /// <summary>
    /// Contrato genérico que define las operaciones de suscripción y difusión de eventos.
    /// </summary>
    /// <typeparam name="T">Tipo de dato que el sujeto maneja y transmite a los observadores.</typeparam>
    public interface ISujeto<T>
    {
        /// <summary>
        /// Registra un nuevo observador en la lista de difusión.
        /// </summary>
        /// <param name="obs">Observador que implementa el contrato compatible con el tipo T.</param>
        void suscribir(IObservador<T> obs);

        /// <summary>
        /// Remueve un observador de la lista de difusión para dejar de notificarle.
        /// </summary>
        /// <param name="obs">Observador que se desea desuscribir.</param>
        void desuscribir(IObservador<T> obs);

        /// <summary>
        /// Itera y ejecuta la actualización en todos los observadores registrados.
        /// </summary>
        /// <param name="datos">Datos de la entidad que se difundirán a los observadores.</param>
        void notificar(T datos);
    }
}