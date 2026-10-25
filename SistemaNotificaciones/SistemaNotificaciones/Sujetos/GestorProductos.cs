using SistemaNotificaciones.Dominio;

namespace SistemaNotificaciones.Sujetos
{
    /// <summary>
    /// Sujeto concreto (Subject) del patrón Observer para la gestión de productos.
    /// Administra el catálogo, asigna los identificadores y notifica a los observadores
    /// ante cada alta. La gestión de suscripciones se hereda de <see cref="SujetoBase{T}"/>.
    /// </summary>
    public class GestorProductos : SujetoBase<Producto>
    {
        private readonly List<Producto> _catalogo = new List<Producto>();
        private int _siguienteId = 1;

        /// <summary>
        /// Vista de solo lectura del catálogo actual de productos.
        /// </summary>
        public IReadOnlyList<Producto> catalogo => _catalogo.AsReadOnly();

        /// <summary>
        /// Identificador que se asignará al próximo producto registrado.
        /// </summary>
        public int siguienteId => _siguienteId;

        /// <summary>
        /// Registra un producto en el catálogo, le asigna un id y notifica a los observadores.
        /// </summary>
        /// <param name="producto">Producto a agregar.</param>
        /// <returns>El producto registrado con su id asignado.</returns>
        /// <exception cref="ArgumentNullException">Si el producto es nulo.</exception>
        public Producto agregarProducto(Producto producto)
        {
            ArgumentNullException.ThrowIfNull(producto);

            producto.id = _siguienteId++;
            _catalogo.Add(producto);
            notificar(producto);
            return producto;
        }
    }
}