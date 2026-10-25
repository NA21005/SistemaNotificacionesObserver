namespace SistemaNotificaciones.Dominio
{
    /// <summary>
    /// Representa la entidad de dominio de un producto dentro del sistema de inventario.
    /// </summary>
    public class Producto
    {
        /// <summary>
        /// Identificador único del producto (asignado por el gestor al registrarlo).
        /// </summary>
        public int id { get; internal set; }

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
        /// <param name="nombre">Nombre o descripción corta del producto (obligatorio).</param>
        /// <param name="precio">Precio unitario del producto (debe ser mayor o igual a cero).</param>
        /// <param name="stock">Existencias iniciales en inventario (debe ser mayor o igual a cero).</param>
        /// <exception cref="ArgumentException">Si el nombre está vacío.</exception>
        public Producto(string nombre, decimal precio, int stock)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre del producto no puede estar vacío.", nameof(nombre));
            }

            if (precio < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(precio), "El precio no puede ser negativo.");
            }

            if (stock < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(stock), "El stock no puede ser negativo.");
            }

            this.nombre = nombre.Trim();
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