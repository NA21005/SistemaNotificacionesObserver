using SistemaNotificaciones.Abstracciones;
using SistemaNotificaciones.Dominio;
using SistemaNotificaciones.Observadores;
using SistemaNotificaciones.Presentacion;
using SistemaNotificaciones.Sujetos;

namespace SistemaNotificaciones
{
    /// <summary>
    /// Punto de entrada y capa de composición del sistema. Crea los sujetos y los observadores,
    /// los conecta entre sí y expone los menús de consola. El estado de cada suscripción se
    /// consulta directamente al sujeto (<see cref="ISujeto{T}.estaSuscrito"/>).
    /// </summary>
    internal class Program
    {
        // Sujetos del sistema
        private static readonly GestorProductos _gestorProductos = new GestorProductos();
        private static readonly GestorTareas _gestorTareas = new GestorTareas();

        // Observadores concretos (instancias compartidas y reutilizables)
        private static readonly NotificadorEmail _emailObserver = new NotificadorEmail("notificaciones@empresa.com");
        private static readonly ServicioAuditoria _auditoriaObserver = new ServicioAuditoria();
        private static readonly AlertaEquipoAlmacen _almacenObserver = new AlertaEquipoAlmacen("Bodega Central");

        private static void Main(string[] args)
        {
            ConsolaUI.EstablecerTitulo("Sistema de Notificaciones - Patrón Observer");
            ConectarTodos(mostrarAviso: false);

            bool salir = false;
            while (!salir)
            {
                MostrarEncabezadoPrincipal();
                MostrarMenuPrincipal();

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" Seleccione una opción: ");
                Console.ResetColor();

                string? opcion = Console.ReadLine();
                if (opcion == null)
                {
                    break; // Fin del stream de entrada
                }

                switch (opcion.Trim())
                {
                    case "1": SubmenuProductos(); break;
                    case "2": SubmenuTareas(); break;
                    case "3": MenuSuscripcionesDinamicas(); break;
                    case "4": EjecutarDemostracionGuiada(); break;
                    case "5":
                        salir = true;
                        MostrarDespedida();
                        break;
                    default:
                        ConsolaUI.Error("Opción no válida. Por favor, seleccione un número del 1 al 5.");
                        ConsolaUI.Pausar();
                        break;
                }
            }
        }

        // =====================================================================
        // Suscripciones (composición de dependencias)
        // =====================================================================

        /// <summary>
        /// Suscribe o desuscribe un observador según su estado actual en el sujeto.
        /// </summary>
        private static void Alternar<T>(ISujeto<T> sujeto, IObservador<T> observador) where T : class
        {
            string nombreObs = observador.GetType().Name;
            string nombreSujeto = sujeto.GetType().Name;

            if (sujeto.estaSuscrito(observador))
            {
                sujeto.desuscribir(observador);
                ConsolaUI.AvisoCambio($"{nombreObs} desuscrito de {nombreSujeto}.");
            }
            else
            {
                sujeto.suscribir(observador);
                ConsolaUI.AvisoCambio($"{nombreObs} suscrito a {nombreSujeto}.");
            }
        }

        /// <summary>
        /// Deja el observador en el estado deseado, alternándolo solo si es necesario.
        /// </summary>
        private static void Asegurar<T>(ISujeto<T> sujeto, IObservador<T> observador, bool suscrito, bool mostrarAviso) where T : class
        {
            if (sujeto.estaSuscrito(observador) == suscrito)
            {
                return;
            }

            if (mostrarAviso)
            {
                Alternar(sujeto, observador);
            }
            else if (suscrito)
            {
                sujeto.suscribir(observador);
            }
            else
            {
                sujeto.desuscribir(observador);
            }
        }

        private static void EstablecerTodos(bool suscrito, bool mostrarAviso)
        {
            Asegurar<Producto>(_gestorProductos, _emailObserver, suscrito, mostrarAviso);
            Asegurar<Producto>(_gestorProductos, _auditoriaObserver, suscrito, mostrarAviso);
            Asegurar<Producto>(_gestorProductos, _almacenObserver, suscrito, mostrarAviso);
            Asegurar<Tarea>(_gestorTareas, _emailObserver, suscrito, mostrarAviso);
            Asegurar<Tarea>(_gestorTareas, _auditoriaObserver, suscrito, mostrarAviso);
        }

        private static void ConectarTodos(bool mostrarAviso = true)
        {
            EstablecerTodos(true, mostrarAviso);
            if (mostrarAviso)
            {
                ConsolaUI.AvisoCambio("Todos los observadores han sido SUSCRITOS exitosamente.");
            }
        }

        private static void DesconectarTodos()
        {
            EstablecerTodos(false, true);
            ConsolaUI.AvisoCambio("Todos los observadores han sido DESUSCRITOS exitosamente.");
        }

        // =====================================================================
        // Menú principal
        // =====================================================================

        private static void MostrarEncabezadoPrincipal()
        {
            ConsolaUI.Limpiar();
            ConsolaUI.Banner(ConsoleColor.Cyan,
                "            SISTEMA DE NOTIFICACIONES MULTICANAL",
                "            Patrón de Diseño de Comportamiento: Observer");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(" [Estado Suscripciones Productos] Email: ");
            ConsolaUI.EscribirEstado(_gestorProductos.estaSuscrito(_emailObserver));
            Console.Write(" | Auditoría: ");
            ConsolaUI.EscribirEstado(_gestorProductos.estaSuscrito(_auditoriaObserver));
            Console.Write(" | Almacén: ");
            ConsolaUI.EscribirEstado(_gestorProductos.estaSuscrito(_almacenObserver));
            Console.WriteLine();

            Console.Write(" [Estado Suscripciones Tareas]    Email: ");
            ConsolaUI.EscribirEstado(_gestorTareas.estaSuscrito(_emailObserver));
            Console.Write(" | Auditoría: ");
            ConsolaUI.EscribirEstado(_gestorTareas.estaSuscrito(_auditoriaObserver));
            Console.WriteLine();
            Console.WriteLine(new string('─', 74));
            Console.ResetColor();
        }

        private static void MostrarMenuPrincipal()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  [1] Módulo de Productos (GestorProductos)");
            Console.WriteLine("  [2] Módulo de Tareas (GestorTareas)");
            Console.WriteLine("  [3] Gestión Dinámica de Suscripciones (Observer en Caliente)");
            Console.WriteLine("  [4] Demostración Guiada en 5 fases");
            Console.WriteLine("  [5] Salir del Sistema");
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void MostrarDespedida()
        {
            ConsolaUI.Limpiar();
            ConsolaUI.Banner(ConsoleColor.Cyan,
                "    Gracias por utilizar el Sistema de Notificaciones Observer",
                "                    ¡Hasta pronto!");
            Console.WriteLine();
        }

        // =====================================================================
        // Módulo de Productos
        // =====================================================================

        private static void SubmenuProductos()
        {
            bool volver = false;
            while (!volver)
            {
                ConsolaUI.Limpiar();
                ConsolaUI.EncabezadoModulo("MÓDULO DE GESTIÓN DE PRODUCTOS", ConsoleColor.Green);
                Console.WriteLine("  [1] Registrar nuevo producto (Disparar Notificaciones)");
                Console.WriteLine("  [2] Ver catálogo actual de productos");
                Console.WriteLine("  [3] Volver al Menú Principal");
                Console.WriteLine();
                Console.Write(" Seleccione una opción: ");

                string? op = Console.ReadLine();
                if (op == null) break;

                switch (op.Trim())
                {
                    case "1": RegistrarProducto(); break;
                    case "2": MostrarCatalogoProductos(); break;
                    case "3": volver = true; break;
                    default:
                        ConsolaUI.Error("Opción no válida.");
                        ConsolaUI.Pausar();
                        break;
                }
            }
        }

        private static void RegistrarProducto()
        {
            ConsolaUI.Limpiar();
            ConsolaUI.EncabezadoModulo("REGISTRAR NUEVO PRODUCTO", ConsoleColor.Green);

            try
            {
                ConsolaUI.Linea($" ID asignado automáticamente: #{_gestorProductos.siguienteId}", ConsoleColor.DarkCyan);

                Console.Write(" Nombre del producto: ");
                string nombre = Console.ReadLine() ?? string.Empty;

                Console.Write(" Precio unitario ($): ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal precio) || precio < 0)
                {
                    throw new ArgumentException("El precio debe ser un número decimal mayor o igual a 0.");
                }

                Console.Write(" Stock inicial (unidades): ");
                if (!int.TryParse(Console.ReadLine(), out int stock) || stock < 0)
                {
                    throw new ArgumentException("El stock debe ser un número entero mayor o igual a 0.");
                }

                Producto nuevo = new Producto(nombre, precio, stock);

                ConsolaUI.EncabezadoDisparo("GestorProductos.agregarProducto()");
                _gestorProductos.agregarProducto(nuevo);

                Console.WriteLine();
                ConsolaUI.Exito($"Producto '{nuevo.nombre}' agregado exitosamente al catálogo.");
            }
            catch (ArgumentException ex)
            {
                ConsolaUI.Error($"Error al registrar producto: {ex.Message}");
            }

            ConsolaUI.Pausar();
        }

        private static void MostrarCatalogoProductos()
        {
            ConsolaUI.Limpiar();
            ConsolaUI.EncabezadoModulo("CATÁLOGO DE PRODUCTOS EN ALMACÉN", ConsoleColor.Green);

            var catalogo = _gestorProductos.catalogo;
            if (catalogo.Count == 0)
            {
                ConsolaUI.Linea(" No hay productos registrados aún en el catálogo.", ConsoleColor.DarkYellow);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(string.Format(" {0,-6} | {1,-30} | {2,12} | {3,10}", "ID", "Nombre", "Precio", "Stock"));
                Console.WriteLine(" " + new string('─', 68));
                Console.ResetColor();

                foreach (var p in catalogo)
                {
                    Console.WriteLine(string.Format(" #{0,-5} | {1,-30} | ${2,11:F2} | {3,7} uds", p.id, p.nombre, p.precio, p.stock));
                }
            }

            ConsolaUI.Pausar();
        }

        // =====================================================================
        // Módulo de Tareas
        // =====================================================================

        private static void SubmenuTareas()
        {
            bool volver = false;
            while (!volver)
            {
                ConsolaUI.Limpiar();
                ConsolaUI.EncabezadoModulo("MÓDULO DE GESTIÓN DE TAREAS", ConsoleColor.Blue);
                Console.WriteLine("  [1] Crear nueva tarea (Disparar Notificaciones)");
                Console.WriteLine("  [2] Ver listado de tareas registradas");
                Console.WriteLine("  [3] Volver al Menú Principal");
                Console.WriteLine();
                Console.Write(" Seleccione una opción: ");

                string? op = Console.ReadLine();
                if (op == null) break;

                switch (op.Trim())
                {
                    case "1": RegistrarTarea(); break;
                    case "2": MostrarListadoTareas(); break;
                    case "3": volver = true; break;
                    default:
                        ConsolaUI.Error("Opción no válida.");
                        ConsolaUI.Pausar();
                        break;
                }
            }
        }

        private static void RegistrarTarea()
        {
            ConsolaUI.Limpiar();
            ConsolaUI.EncabezadoModulo("CREAR NUEVA TAREA", ConsoleColor.Blue);

            try
            {
                Console.Write(" Título de la tarea: ");
                string titulo = Console.ReadLine() ?? string.Empty;

                Console.Write(" Detalle o descripción: ");
                string detalle = Console.ReadLine()?.Trim() ?? string.Empty;

                Console.WriteLine(" Prioridad [1=Baja, 2=Media, 3=Alta] (Enter para Media): ");
                Console.Write(" Ingrese prioridad (1-3): ");
                string? prioridadInput = Console.ReadLine()?.Trim();

                Prioridad prioridad = Prioridad.Media;
                if (!string.IsNullOrWhiteSpace(prioridadInput))
                {
                    if (!int.TryParse(prioridadInput, out int valor) || !Enum.IsDefined(typeof(Prioridad), valor))
                    {
                        throw new ArgumentException("La prioridad seleccionada no es válida. Use 1 (Baja), 2 (Media) o 3 (Alta).");
                    }
                    prioridad = (Prioridad)valor;
                }

                Tarea nueva = new Tarea(titulo, detalle, prioridad);

                ConsolaUI.EncabezadoDisparo("GestorTareas.crearTarea()");
                _gestorTareas.crearTarea(nueva);

                Console.WriteLine();
                ConsolaUI.Exito($"Tarea #{nueva.id} '{nueva.titulo}' registrada y notificada con éxito.");
            }
            catch (ArgumentException ex)
            {
                ConsolaUI.Error($"Error al crear tarea: {ex.Message}");
            }

            ConsolaUI.Pausar();
        }

        private static void MostrarListadoTareas()
        {
            ConsolaUI.Limpiar();
            ConsolaUI.EncabezadoModulo("LISTADO DE TAREAS REGISTRADAS", ConsoleColor.Blue);

            var tareas = _gestorTareas.tareas;
            if (tareas.Count == 0)
            {
                ConsolaUI.Linea(" No hay tareas registradas en el sistema.", ConsoleColor.DarkYellow);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(string.Format(" {0,-6} | {1,-24} | {2,-10} | {3,-24}", "ID", "Título", "Prioridad", "Detalle"));
                Console.WriteLine(" " + new string('─', 72));
                Console.ResetColor();

                foreach (var t in tareas)
                {
                    Console.WriteLine(string.Format(" #{0,-5} | {1,-24} | {2,-10} | {3,-24}", t.id, t.titulo, t.prioridad, t.detalle));
                }
            }

            ConsolaUI.Pausar();
        }

        // =====================================================================
        // Gestión dinámica de suscripciones
        // =====================================================================

        private static void MenuSuscripcionesDinamicas()
        {
            bool volver = false;
            while (!volver)
            {
                ConsolaUI.Limpiar();
                ConsolaUI.EncabezadoModulo("CONFIGURACIÓN DINÁMICA DE SUSCRIPCIONES (OBSERVER EN CALIENTE)", ConsoleColor.Magenta);

                Console.WriteLine(" Permite conectar o desconectar observadores en tiempo de ejecución.");
                Console.WriteLine(" Los cambios se aplican inmediatamente sin reiniciar el programa.");
                Console.WriteLine();

                ConsolaUI.Linea(" ─── Suscripciones al Sujeto: GestorProductos ─────────────────────────", ConsoleColor.Yellow);
                MostrarOpcionSuscripcion("  [1] NotificadorEmail:        ", _gestorProductos.estaSuscrito(_emailObserver));
                MostrarOpcionSuscripcion("  [2] ServicioAuditoria:       ", _gestorProductos.estaSuscrito(_auditoriaObserver));
                MostrarOpcionSuscripcion("  [3] AlertaEquipoAlmacen:     ", _gestorProductos.estaSuscrito(_almacenObserver));

                Console.WriteLine();
                ConsolaUI.Linea(" ─── Suscripciones al Sujeto: GestorTareas ────────────────────────────", ConsoleColor.Yellow);
                MostrarOpcionSuscripcion("  [4] NotificadorEmail:        ", _gestorTareas.estaSuscrito(_emailObserver));
                MostrarOpcionSuscripcion("  [5] ServicioAuditoria:       ", _gestorTareas.estaSuscrito(_auditoriaObserver));

                Console.WriteLine();
                ConsolaUI.Linea(" ─── Opciones Globales ────────────────────────────────────────────────", ConsoleColor.Yellow);
                Console.WriteLine("  [6] Conectar TODOS los observadores a ambos sujetos");
                Console.WriteLine("  [7] Desconectar TODOS los observadores (Silenciar notificaciones)");
                Console.WriteLine("  [8] Volver al Menú Principal");
                Console.WriteLine();

                Console.Write(" Seleccione una opción para alternar (1-8): ");
                string? op = Console.ReadLine();
                if (op == null) break;

                switch (op.Trim())
                {
                    case "1": Alternar<Producto>(_gestorProductos, _emailObserver); break;
                    case "2": Alternar<Producto>(_gestorProductos, _auditoriaObserver); break;
                    case "3": Alternar<Producto>(_gestorProductos, _almacenObserver); break;
                    case "4": Alternar<Tarea>(_gestorTareas, _emailObserver); break;
                    case "5": Alternar<Tarea>(_gestorTareas, _auditoriaObserver); break;
                    case "6": ConectarTodos(); break;
                    case "7": DesconectarTodos(); break;
                    case "8": volver = true; break;
                    default:
                        ConsolaUI.Error("Opción no válida.");
                        ConsolaUI.Pausar();
                        break;
                }
            }
        }

        private static void MostrarOpcionSuscripcion(string etiqueta, bool suscrito)
        {
            Console.Write(etiqueta);
            if (suscrito)
            {
                ConsolaUI.Linea("[SUSCRITO / ACTIVO]      (Presione número para desuscribir)", ConsoleColor.Green);
            }
            else
            {
                ConsolaUI.Linea("[DESUSCRITO / INACTIVO]  (Presione número para suscribir)", ConsoleColor.Red);
            }
        }

        // =====================================================================
        // Demostración guiada
        // =====================================================================

        private static void EjecutarDemostracionGuiada()
        {
            ConsolaUI.Limpiar();
            ConsolaUI.Banner(ConsoleColor.Cyan,
                "        DEMOSTRACIÓN GUIADA DEL PATRÓN OBSERVER",
                "        Validación en Vivo de Suscripción Dinámica");

            Console.WriteLine();
            Console.WriteLine(" Esta demostración ejecuta automáticamente cinco fases:");
            Console.WriteLine("   1. Suscripción inicial de todos los observadores.");
            Console.WriteLine("   2. Disparo del evento 1 -> Recepción por 3 observadores.");
            Console.WriteLine("   3. Desuscripción dinámica en caliente de NotificadorEmail.");
            Console.WriteLine("   4. Disparo del evento 2 -> Recepción selectiva (Email ya NO recibe).");
            Console.WriteLine("   5. Reutilización con Tareas (misma instancia, otro sujeto).");
            Console.WriteLine();

            ConsolaUI.PasoGuiado("Iniciar Fase 1: Configurar Sujeto y Suscribir Observadores");
            ConectarTodos();
            Console.WriteLine(" Observadores suscritos en GestorProductos:");
            Console.WriteLine("   • NotificadorEmail");
            Console.WriteLine("   • ServicioAuditoria");
            Console.WriteLine("   • AlertaEquipoAlmacen");
            Console.WriteLine();

            ConsolaUI.PasoGuiado("Iniciar Fase 2: Registrar Producto 1 y Propagar Notificaciones");
            DispararProducto(new Producto("Servidor Rack Dell PowerEdge", 4500.00m, 5));
            Console.WriteLine();
            ConsolaUI.Exito("Resultado Fase 2: Los 3 observadores reaccionaron en tiempo y forma.");

            ConsolaUI.PasoGuiado("Iniciar Fase 3: Desuscripción Dinámica en Caliente");
            Console.WriteLine(" [OPERACION] Desuscribiendo NotificadorEmail de GestorProductos...");
            _gestorProductos.desuscribir(_emailObserver);
            ConsolaUI.Linea("  -> NotificadorEmail ha sido desuscrito de la lista de difusión.", ConsoleColor.DarkYellow);
            ConsolaUI.Linea("  -> Los observadores ServicioAuditoria y AlertaEquipoAlmacen CONTINÚAN activos.", ConsoleColor.DarkYellow);

            ConsolaUI.PasoGuiado("Iniciar Fase 4: Registrar Producto 2 y Verificar Notificación Selectiva");
            DispararProducto(new Producto("Switch Gigabit Cisco Catalyst", 850.00m, 12));
            Console.WriteLine();
            ConsolaUI.Exito("Resultado Fase 4: NotificadorEmail NO imprimió mensaje (desuscrito con éxito).");
            Console.WriteLine("   ServicioAuditoria y AlertaEquipoAlmacen respondieron normalmente.");

            ConsolaUI.PasoGuiado("Iniciar Fase 5: Reutilización de Observadores con el Módulo de Tareas");
            Console.WriteLine(" NotificadorEmail y ServicioAuditoria implementan IObservador<Producto>");
            Console.WriteLine(" e IObservador<Tarea> simultáneamente...");
            Console.WriteLine();

            Tarea demoTarea = new Tarea("Auditoría de Seguridad Trimestral", "Revisión de logs y credenciales de acceso", Prioridad.Alta);
            ConsolaUI.Linea($" [ACCION] GestorTareas.crearTarea({demoTarea.titulo})", ConsoleColor.Yellow);
            Console.WriteLine();
            _gestorTareas.crearTarea(demoTarea);

            Console.WriteLine();
            ConsolaUI.Banner(ConsoleColor.Cyan,
                "            DEMOSTRACIÓN COMPLETADA CON ÉXITO",
                "   Se verificó la suscripción dinámica, el desacoplamiento total",
                "   y la reutilización de los observadores en .NET.");

            ConsolaUI.Pausar();
        }

        private static void DispararProducto(Producto producto)
        {
            ConsolaUI.Linea($" [ACCION] GestorProductos.agregarProducto({producto.nombre})", ConsoleColor.Yellow);
            Console.WriteLine();
            _gestorProductos.agregarProducto(producto);
        }
    }
}
