using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SistemaNotificaciones.Abstracciones;
using SistemaNotificaciones.Dominio;
using SistemaNotificaciones.Observadores;
using SistemaNotificaciones.Sujetos;

namespace SistemaNotificaciones
{
    /// <summary>
    /// Punto de entrada del sistema. Controla la interfaz de consola, coordina la interacción
    /// entre Sujetos y Observadores y gestiona las suscripciones dinámicas en tiempo real.
    /// </summary>
    internal class Program
    {
        // =========================================================================
        // Sujetos del Sistema
        // =========================================================================
        private static readonly GestorProductos _gestorProductos = new GestorProductos();
        private static readonly GestorTareas _gestorTareas = new GestorTareas();

        // =========================================================================
        // Observadores Concretos (Instancias Compartidas Reutilizables)
        // =========================================================================
        private static readonly NotificadorEmail _emailObserver = new NotificadorEmail("notificaciones@empresa.com");
        private static readonly ServicioAuditoria _auditoriaObserver = new ServicioAuditoria();
        private static readonly AlertaEquipoAlmacen _almacenObserver = new AlertaEquipoAlmacen("Bodega Central");

        // =========================================================================
        // Banderas de Estado de Suscripción Dinámica
        // =========================================================================
        private static bool _prodEmailSub = true;
        private static bool _prodAuditoriaSub = true;
        private static bool _prodAlmacenSub = true;

        private static bool _tareaEmailSub = true;
        private static bool _tareaAuditoriaSub = true;

        private static int _siguienteIdProducto = 1;

        private static void Main(string[] args)
        {
            SafeSetTitle("Sistema de Notificaciones - Patrón Observer");
            InicializarSuscripciones();

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
                    // Fin del stream de entrada
                    break;
                }

                switch (opcion.Trim())
                {
                    case "1":
                        SubmenuProductos();
                        break;
                    case "2":
                        SubmenuTareas();
                        break;
                    case "3":
                        MenuSuscripcionesDinamicas();
                        break;
                    case "4":
                        EjecutarDemostracionSecuenciaGuiada();
                        break;
                    case "5":
                        salir = true;
                        MostrarDespedida();
                        break;
                    default:
                        MostrarMensajeError("Opción no válida. Por favor, seleccione un número del 1 al 5.");
                        PausarConsola();
                        break;
                }
            }
        }

        // =========================================================================
        // Inicialización y Sincronización de Suscripciones
        // =========================================================================

        private static void InicializarSuscripciones()
        {
            // Suscripción inicial a Productos
            _gestorProductos.suscribir(_emailObserver);
            _gestorProductos.suscribir(_auditoriaObserver);
            _gestorProductos.suscribir(_almacenObserver);

            // Suscripción inicial a Tareas
            _gestorTareas.suscribir(_emailObserver);
            _gestorTareas.suscribir(_auditoriaObserver);
        }

        // =========================================================================
        // Menú Principal y Vistas de Encabezado
        // =========================================================================

        private static void MostrarEncabezadoPrincipal()
        {
            SafeClear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║            SISTEMA DE NOTIFICACIONES MULTICANAL                        ║");
            Console.WriteLine("║            Patrón de Diseño de Comportamiento: Observer                ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            // Resumen de estado rápido
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(" [Estado Suscripciones Productos] Email: ");
            EscribirEstado(_prodEmailSub);
            Console.Write(" | Auditoría: ");
            EscribirEstado(_prodAuditoriaSub);
            Console.Write(" | Almacén: ");
            EscribirEstado(_prodAlmacenSub);
            Console.WriteLine();

            Console.Write(" [Estado Suscripciones Tareas]    Email: ");
            EscribirEstado(_tareaEmailSub);
            Console.Write(" | Auditoría: ");
            EscribirEstado(_tareaAuditoriaSub);
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
            Console.WriteLine("  [4] Demostración Guiada (Flujo del Diagrama de Secuencia)");
            Console.WriteLine("  [5] Salir del Sistema");
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void EscribirEstado(bool activo)
        {
            if (activo)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("ACTIVO");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("INACTIVO");
            }
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }

        // =========================================================================
        // Módulo de Productos
        // =========================================================================

        private static void SubmenuProductos()
        {
            bool volver = false;
            while (!volver)
            {
                SafeClear();
                MostrarEncabezadoModulo("MÓDULO DE GESTIÓN DE PRODUCTOS", ConsoleColor.Green);
                Console.WriteLine("  [1] Registrar nuevo producto (Disparar Notificaciones)");
                Console.WriteLine("  [2] Ver catálogo actual de productos");
                Console.WriteLine("  [3] Volver al Menú Principal");
                Console.WriteLine();
                Console.Write(" Seleccione una opción: ");

                string? op = Console.ReadLine();
                if (op == null) break;

                switch (op.Trim())
                {
                    case "1":
                        RegistrarProductoInteractiva();
                        break;
                    case "2":
                        MostrarCatalogoProductos();
                        break;
                    case "3":
                        volver = true;
                        break;
                    default:
                        MostrarMensajeError("Opción no válida.");
                        PausarConsola();
                        break;
                }
            }
        }

        private static void RegistrarProductoInteractiva()
        {
            SafeClear();
            MostrarEncabezadoModulo("REGISTRAR NUEVO PRODUCTO", ConsoleColor.Green);

            try
            {
                int id = _siguienteIdProducto;
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine($" ID asignado automáticamente: #{id}");
                Console.ResetColor();

                Console.Write(" Nombre del producto: ");
                string? nombre = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    throw new ArgumentException("El nombre del producto no puede estar vacío.");
                }

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

                Producto nuevoProducto = new Producto(id, nombre, precio, stock);
                _siguienteIdProducto++;

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("────────────────────────────────────────────────────────────────────────");
                Console.WriteLine(" Disparando evento GestorProductos.agregarProducto()...");
                Console.WriteLine(" Observadores suscritos respondiendo a la notificación:");
                Console.WriteLine("────────────────────────────────────────────────────────────────────────");
                Console.ResetColor();

                // Disparo de la notificación a través del Sujeto
                _gestorProductos.agregarProducto(nuevoProducto);

                Console.WriteLine();
                MostrarMensajeExito($"Producto '{nombre}' agregado exitosamente al catálogo.");
            }
            catch (Exception ex)
            {
                MostrarMensajeError($"Error al registrar producto: {ex.Message}");
            }

            PausarConsola();
        }

        private static void MostrarCatalogoProductos()
        {
            SafeClear();
            MostrarEncabezadoModulo("CATÁLOGO DE PRODUCTOS EN ALMACÉN", ConsoleColor.Green);

            var catalogo = _gestorProductos.catalogo;
            if (catalogo.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(" No hay productos registrados aún en el catálogo.");
                Console.ResetColor();
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

            PausarConsola();
        }

        // =========================================================================
        // Módulo de Tareas
        // =========================================================================

        private static void SubmenuTareas()
        {
            bool volver = false;
            while (!volver)
            {
                SafeClear();
                MostrarEncabezadoModulo("MÓDULO DE GESTIÓN DE TAREAS", ConsoleColor.Blue);
                Console.WriteLine("  [1] Crear nueva tarea (Disparar Notificaciones)");
                Console.WriteLine("  [2] Ver listado de tareas registradas");
                Console.WriteLine("  [3] Volver al Menú Principal");
                Console.WriteLine();
                Console.Write(" Seleccione una opción: ");

                string? op = Console.ReadLine();
                if (op == null) break;

                switch (op.Trim())
                {
                    case "1":
                        RegistrarTareaInteractiva();
                        break;
                    case "2":
                        MostrarListadoTareas();
                        break;
                    case "3":
                        volver = true;
                        break;
                    default:
                        MostrarMensajeError("Opción no válida.");
                        PausarConsola();
                        break;
                }
            }
        }

        private static void RegistrarTareaInteractiva()
        {
            SafeClear();
            MostrarEncabezadoModulo("CREAR NUEVA TAREA", ConsoleColor.Blue);

            try
            {
                Console.Write(" Título de la tarea: ");
                string? titulo = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(titulo))
                {
                    throw new ArgumentException("El título de la tarea no puede estar vacío.");
                }

                Console.Write(" Detalle o descripción: ");
                string? detalle = Console.ReadLine()?.Trim() ?? string.Empty;

                Console.WriteLine(" Prioridad [1=Baja, 2=Media, 3=Alta] (Enter para Media): ");
                Console.Write(" Ingrese prioridad (1-3): ");
                string? prioridadInput = Console.ReadLine()?.Trim();

                Prioridad prioridad = Prioridad.Media;
                if (!string.IsNullOrWhiteSpace(prioridadInput))
                {
                    if (int.TryParse(prioridadInput, out int pInt) && Enum.IsDefined(typeof(Prioridad), pInt))
                    {
                        prioridad = (Prioridad)pInt;
                    }
                    else
                    {
                        throw new ArgumentException("La prioridad seleccionada no es válida. Use 1 (Baja), 2 (Media) o 3 (Alta).");
                    }
                }

                Tarea nuevaTarea = new Tarea(titulo, detalle, prioridad);

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("────────────────────────────────────────────────────────────────────────");
                Console.WriteLine(" Disparando evento GestorTareas.crearTarea()...");
                Console.WriteLine(" Observadores suscritos respondiendo a la notificación:");
                Console.WriteLine("────────────────────────────────────────────────────────────────────────");
                Console.ResetColor();

                // Disparo de la notificación a través del Sujeto
                _gestorTareas.crearTarea(nuevaTarea);

                Console.WriteLine();
                MostrarMensajeExito($"Tarea #{nuevaTarea.id} '{nuevaTarea.titulo}' registrada y notificada con éxito.");
            }
            catch (Exception ex)
            {
                MostrarMensajeError($"Error al crear tarea: {ex.Message}");
            }

            PausarConsola();
        }

        private static void MostrarListadoTareas()
        {
            SafeClear();
            MostrarEncabezadoModulo("LISTADO DE TAREAS REGISTRADAS", ConsoleColor.Blue);

            var tareas = _gestorTareas.tareas;
            if (tareas.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(" No hay tareas registradas en el sistema.");
                Console.ResetColor();
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

            PausarConsola();
        }

        // =========================================================================
        // Gestión Dinámica de Suscripciones (Observer en Caliente)
        // =========================================================================

        private static void MenuSuscripcionesDinamicas()
        {
            bool volver = false;
            while (!volver)
            {
                SafeClear();
                MostrarEncabezadoModulo("CONFIGURACIÓN DINÁMICA DE SUSCRIPCIONES (OBSERVER EN CALIENTE)", ConsoleColor.Magenta);

                Console.WriteLine(" Permite conectar o desconectar observadores en tiempo de ejecución.");
                Console.WriteLine(" Los cambios se aplican inmediatamente sin reiniciar el programa.");
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" ─── Suscripciones al Sujeto: GestorProductos ─────────────────────────");
                Console.ResetColor();
                Console.Write("  [1] NotificadorEmail:        "); EscribirBotonEstado(_prodEmailSub);
                Console.Write("  [2] ServicioAuditoria:       "); EscribirBotonEstado(_prodAuditoriaSub);
                Console.Write("  [3] AlertaEquipoAlmacen:     "); EscribirBotonEstado(_prodAlmacenSub);

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" ─── Suscripciones al Sujeto: GestorTareas ────────────────────────────");
                Console.ResetColor();
                Console.Write("  [4] NotificadorEmail:        "); EscribirBotonEstado(_tareaEmailSub);
                Console.Write("  [5] ServicioAuditoria:       "); EscribirBotonEstado(_tareaAuditoriaSub);

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" ─── Opciones Globales ────────────────────────────────────────────────");
                Console.ResetColor();
                Console.WriteLine("  [6] Conectar TODOS los observadores a ambos sujetos");
                Console.WriteLine("  [7] Desconectar TODOS los observadores (Silenciar notificaciones)");
                Console.WriteLine("  [8] Volver al Menú Principal");
                Console.WriteLine();

                Console.Write(" Seleccione una opción para alternar (1-8): ");
                string? op = Console.ReadLine();
                if (op == null) break;

                switch (op.Trim())
                {
                    case "1":
                        AlternarProdEmail();
                        break;
                    case "2":
                        AlternarProdAuditoria();
                        break;
                    case "3":
                        AlternarProdAlmacen();
                        break;
                    case "4":
                        AlternarTareaEmail();
                        break;
                    case "5":
                        AlternarTareaAuditoria();
                        break;
                    case "6":
                        ConectarTodos();
                        break;
                    case "7":
                        DesconectarTodos();
                        break;
                    case "8":
                        volver = true;
                        break;
                    default:
                        MostrarMensajeError("Opción no válida.");
                        PausarConsola();
                        break;
                }
            }
        }

        private static void EscribirBotonEstado(bool activo)
        {
            if (activo)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[SUSCRITO / ACTIVO]      (Presione número para desuscribir)");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[DESUSCRITO / INACTIVO]  (Presione número para suscribir)");
            }
            Console.ResetColor();
        }

        private static void AlternarProdEmail()
        {
            if (_prodEmailSub)
            {
                _gestorProductos.desuscribir(_emailObserver);
                _prodEmailSub = false;
                MostrarAvisoCambio("NotificadorEmail desuscrito de GestorProductos.");
            }
            else
            {
                _gestorProductos.suscribir(_emailObserver);
                _prodEmailSub = true;
                MostrarAvisoCambio("NotificadorEmail suscrito a GestorProductos.");
            }
        }

        private static void AlternarProdAuditoria()
        {
            if (_prodAuditoriaSub)
            {
                _gestorProductos.desuscribir(_auditoriaObserver);
                _prodAuditoriaSub = false;
                MostrarAvisoCambio("ServicioAuditoria desuscrito de GestorProductos.");
            }
            else
            {
                _gestorProductos.suscribir(_auditoriaObserver);
                _prodAuditoriaSub = true;
                MostrarAvisoCambio("ServicioAuditoria suscrito a GestorProductos.");
            }
        }

        private static void AlternarProdAlmacen()
        {
            if (_prodAlmacenSub)
            {
                _gestorProductos.desuscribir(_almacenObserver);
                _prodAlmacenSub = false;
                MostrarAvisoCambio("AlertaEquipoAlmacen desuscrito de GestorProductos.");
            }
            else
            {
                _gestorProductos.suscribir(_almacenObserver);
                _prodAlmacenSub = true;
                MostrarAvisoCambio("AlertaEquipoAlmacen suscrito a GestorProductos.");
            }
        }

        private static void AlternarTareaEmail()
        {
            if (_tareaEmailSub)
            {
                _gestorTareas.desuscribir(_emailObserver);
                _tareaEmailSub = false;
                MostrarAvisoCambio("NotificadorEmail desuscrito de GestorTareas.");
            }
            else
            {
                _gestorTareas.suscribir(_emailObserver);
                _tareaEmailSub = true;
                MostrarAvisoCambio("NotificadorEmail suscrito a GestorTareas.");
            }
        }

        private static void AlternarTareaAuditoria()
        {
            if (_tareaAuditoriaSub)
            {
                _gestorTareas.desuscribir(_auditoriaObserver);
                _tareaAuditoriaSub = false;
                MostrarAvisoCambio("ServicioAuditoria desuscrito de GestorTareas.");
            }
            else
            {
                _gestorTareas.suscribir(_auditoriaObserver);
                _tareaAuditoriaSub = true;
                MostrarAvisoCambio("ServicioAuditoria suscrito a GestorTareas.");
            }
        }

        private static void ConectarTodos()
        {
            if (!_prodEmailSub) AlternarProdEmail();
            if (!_prodAuditoriaSub) AlternarProdAuditoria();
            if (!_prodAlmacenSub) AlternarProdAlmacen();
            if (!_tareaEmailSub) AlternarTareaEmail();
            if (!_tareaAuditoriaSub) AlternarTareaAuditoria();
            MostrarAvisoCambio("Todos los observadores han sido SUSCRITOS exitosamente.");
        }

        private static void DesconectarTodos()
        {
            if (_prodEmailSub) AlternarProdEmail();
            if (_prodAuditoriaSub) AlternarProdAuditoria();
            if (_prodAlmacenSub) AlternarProdAlmacen();
            if (_tareaEmailSub) AlternarTareaEmail();
            if (_tareaAuditoriaSub) AlternarTareaAuditoria();
            MostrarAvisoCambio("Todos los observadores han sido DESUSCRITOS exitosamente.");
        }

        private static void MostrarAvisoCambio(string mensaje)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" ✔ {mensaje}");
            Console.ResetColor();
            try { Thread.Sleep(300); } catch { }
        }

        // =========================================================================
        // Demostración Guiada del Flujo del Diagrama de Secuencia
        // =========================================================================

        private static void EjecutarDemostracionSecuenciaGuiada()
        {
            SafeClear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║        DEMOSTRACIÓN GUIADA DEL FLUJO DEL DIAGRAMA DE SECUENCIA         ║");
            Console.WriteLine("║        Validación en Vivo de Suscripción Dinámica en Observer          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine();
            Console.WriteLine(" Esta demostración ejecuta automáticamente las fases exactas representadas");
            Console.WriteLine(" en el diagrama de secuencia UML del informe formal:");
            Console.WriteLine("   1. Suscripción inicial de todos los observadores.");
            Console.WriteLine("   2. Disparo del evento 1 -> Recepción por 3 observadores.");
            Console.WriteLine("   3. Desuscripción dinámica en caliente de NotificadorEmail.");
            Console.WriteLine("   4. Disparo del evento 2 -> Recepción selectiva (Email ya NO recibe).");
            Console.WriteLine("   5. Demostración de Polimorfismo con Tareas (misma instancia, otro sujeto).");
            Console.WriteLine();

            PasoGuiado("Iniciar Fase 1: Configurar Sujeto y Suscribir Observadores");

            // Aseguramos estado conectado
            ConectarTodos();
            Console.WriteLine(" Observadores suscritos en GestorProductos:");
            Console.WriteLine("   • NotificadorEmail");
            Console.WriteLine("   • ServicioAuditoria");
            Console.WriteLine("   • AlertaEquipoAlmacen");
            Console.WriteLine();

            PasoGuiado("Iniciar Fase 2: Registrar Producto 1 y Propagar Notificaciones");

            Producto demoProd1 = new Producto(_siguienteIdProducto++, "Servidor Rack Dell PowerEdge", 4500.00m, 5);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($" [ACCION] GestorProductos.agregarProducto({demoProd1.nombre})");
            Console.ResetColor();
            Console.WriteLine();

            // Disparo del primer evento
            _gestorProductos.agregarProducto(demoProd1);

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" ✔ Resultado Fase 2: Los 3 observadores reaccionaron en tiempo y forma.");
            Console.ResetColor();

            PasoGuiado("Iniciar Fase 3: Desuscripción Dinámica en Caliente");

            Console.WriteLine(" [OPERACION] Desuscribiendo NotificadorEmail de GestorProductos...");
            _gestorProductos.desuscribir(_emailObserver);
            _prodEmailSub = false;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("  -> NotificadorEmail ha sido desuscrito de la lista de difusión.");
            Console.WriteLine("  -> Los observadores ServicioAuditoria y AlertaEquipoAlmacen CONTINÚAN activos.");
            Console.ResetColor();

            PasoGuiado("Iniciar Fase 4: Registrar Producto 2 y Verificar Notificación Selectiva");

            Producto demoProd2 = new Producto(_siguienteIdProducto++, "Switch Gigabit Cisco Catalyst", 850.00m, 12);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($" [ACCION] GestorProductos.agregarProducto({demoProd2.nombre})");
            Console.ResetColor();
            Console.WriteLine();

            // Disparo del segundo evento
            _gestorProductos.agregarProducto(demoProd2);

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" ✔ Resultado Fase 4: NotificadorEmail NO imprimió mensaje (desuscrito con éxito).");
            Console.WriteLine("   ServicioAuditoria y AlertaEquipoAlmacen respondieron normalmente.");
            Console.ResetColor();

            PasoGuiado("Iniciar Fase 5: Demostración de Polimorfismo con Módulo de Tareas");

            Console.WriteLine(" Demostrando que NotificadorEmail y ServicioAuditoria son polimórficos");
            Console.WriteLine(" implementando IObservador<Producto> e IObservador<Tarea> simultáneamente...");
            Console.WriteLine();

            Tarea demoTarea = new Tarea("Auditoría de Seguridad Trimestral", "Revisión de logs y credenciales de acceso", Prioridad.Alta);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($" [ACCION] GestorTareas.crearTarea({demoTarea.titulo})");
            Console.ResetColor();
            Console.WriteLine();

            _gestorTareas.crearTarea(demoTarea);

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║            DEMOSTRACIÓN DE SECUENCIA COMPLETADA CON ÉXITO              ║");
            Console.WriteLine("║   Se ha verificado la suscripción dinámica, el desacoplamiento total   ║");
            Console.WriteLine("║   y la reutilización polimórfica de los observadores en .NET.          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            PausarConsola();
        }

        private static void PasoGuiado(string descripcionPaso)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($" >> PASO: {descripcionPaso} << ");
            Console.ResetColor();
            Console.WriteLine(" Presione [ENTER] para ejecutar este paso...");
            try
            {
                Console.ReadLine();
            }
            catch
            {
            }
        }

        // =========================================================================
        // Métodos Utilitarios de Consola Seguros
        // =========================================================================

        private static void SafeSetTitle(string title)
        {
            try
            {
                if (!Console.IsOutputRedirected)
                {
                    Console.Title = title;
                }
            }
            catch
            {
            }
        }

        private static void SafeClear()
        {
            try
            {
                if (!Console.IsOutputRedirected)
                {
                    Console.Clear();
                }
            }
            catch
            {
            }
        }

        private static void MostrarEncabezadoModulo(string titulo, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine("┌" + new string('─', 72) + "┐");
            Console.WriteLine("│ " + titulo.PadRight(70) + " │");
            Console.WriteLine("└" + new string('─', 72) + "┘");
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void MostrarMensajeExito(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" ✔ {mensaje}");
            Console.ResetColor();
        }

        private static void MostrarMensajeError(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($" ✖ {mensaje}");
            Console.ResetColor();
        }

        private static void PausarConsola()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(" Presione [ENTER] o cualquier tecla para continuar...");
            Console.ResetColor();
            try
            {
                if (Console.IsInputRedirected)
                {
                    Console.ReadLine();
                }
                else
                {
                    Console.ReadKey(true);
                }
            }
            catch
            {
            }
        }

        private static void MostrarDespedida()
        {
            SafeClear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║    Gracias por utilizar el Sistema de Notificaciones Observer          ║");
            Console.WriteLine("║                    ¡Hasta pronto!                                      ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}
