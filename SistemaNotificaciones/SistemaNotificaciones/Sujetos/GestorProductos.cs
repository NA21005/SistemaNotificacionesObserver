using SistemaNotificaciones.Abstracciones;
using SistemaNotificaciones.Dominio;

namespace SistemaNotificaciones.Sujetos
{
    /// <summary>
    /// Actúa como el Sujeto Concreto (Subject) en el patrón Observer para la gestión de productos.
    /// Administra el catálogo y notifica a los observadores ante cualquier adición.
    /// </summary>
    public class GestorProductos : ISujeto<Producto>
    {
        private readonly List<Producto> _catalogo;
        private readonly List<IObservador<Producto>> _observadores;

        public GestorProductos()
        {
            _catalogo = new List<Producto>();
            _observadores = new List<IObservador<Producto>>();
        }

        /// <summary>
        /// Obtiene una vista de solo lectura del catálogo actual de productos.
        /// </summary>
        public IReadOnlyList<Producto> catalogo => _catalogo.AsReadOnly();

        /// <summary>
        /// Suscribe un nuevo observador para recibir notificaciones.
        /// </summary>
        public void suscribir(IObservador<Producto> observador)
        {
            if (observador == null)
            {
                throw new ArgumentNullException(nameof(observador), "El observador a suscribir no puede ser nulo.");
            }

            if (!_observadores.Contains(observador))
            {
                _observadores.Add(observador);
            }
        }

        /// <summary>
        /// Desuscribe un observador previamente registrado.
        /// </summary>
        public void desuscribir(IObservador<Producto> observador)
        {
            if (observador != null && _observadores.Contains(observador))
            {
                _observadores.Remove(observador);
            }
        }

        /// <summary>
        /// Notifica a todos los observadores registrados sobre la adición de un producto.
        /// </summary>
        public void notificar(Producto producto)
        {
            if (producto == null) return;

            foreach (var observador in _observadores.ToList())
            {
                observador.actualizar(producto);
            }
        }

        /// <summary>
        /// Agrega un nuevo producto al catálogo interno y dispara la notificación a los observadores.
        /// </summary>
        public void agregarProducto(Producto producto)
        {
            if (producto == null)
            {
                throw new ArgumentNullException(nameof(producto), "El producto a agregar no puede ser nulo.");
            }

            _catalogo.Add(producto);
            notificar(producto);
        }
    }
}