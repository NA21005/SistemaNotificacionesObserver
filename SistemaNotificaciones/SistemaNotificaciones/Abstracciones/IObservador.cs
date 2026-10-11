namespace SistemaNotificaciones.Abstracciones
{
    /// <summary>
    /// Contrato genérico para receptores de eventos que reaccionan a los cambios del sujeto.
    /// </summary>
    /// <typeparam name="T">Tipo de dato de la entidad notificada (Producto o Tarea).</typeparam>
    public interface IObservador<T>
    {
        /// <summary>
        /// Recibe y procesa la información propagada por el sujeto cuando ocurre un evento.
        /// </summary>
        /// <param name="datos">Instancia con la información de la entidad que lanza la notificación.</param>
        void actualizar(T datos);
    }
}