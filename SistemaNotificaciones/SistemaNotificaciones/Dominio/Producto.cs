namespace SistemaNotificaciones.Dominio
{
    /// <summary>
    /// Representa la entidad de dominio de un producto dentro del sistema de inventario.
    /// </summary>
    public class Producto
    {
        /// <summary>
        /// Obtiene o establece el identificador único del producto.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre comercial del producto.
        /// </summary>
        public string nombre { get; set; }

        /// <summary>
        /// Obtiene o establece el precio unitario del producto.
        /// </summary>
        public decimal precio { get; set; }

        /// <summary>
        /// Obtiene o establece la cantidad disponible en stock.
        /// </summary>
        public int stock { get; set; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Producto"/>.
        /// </summary>
        /// <param name="id">Identificador único.</param>
        /// <param name="nombre">Nombre o descripción corta del producto.</param>
        /// <param name="precio">Precio unitario del producto (debe ser mayor o igual a cero).</param>
        /// <param name="stock">Existencias iniciales en inventario (debe ser mayor o igual a cero).</param>
        public Producto(int id, string nombre, decimal precio, int stock)
        {
            if (precio < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(precio), "El precio no puede ser negativo.");
            }

            if (stock < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stock), "El stock no puede ser negativo.");
            }

            this.id = id;
            this.nombre = nombre ?? throw new ArgumentNullException(nameof(nombre), "El nombre no puede ser nulo.");
            this.precio = precio;
            this.stock = stock;
        }

        /// <summary>
        /// Devuelve una representación en texto del producto con formato legible.
        /// </summary>
        /// <returns>Cadena formateada con la información básica del producto.</returns>
        public override string ToString()
        {
            return $"[Producto #{id}] {nombre} | Precio: ${precio:F2} | Stock: {stock} unidades";
        }
    }
}