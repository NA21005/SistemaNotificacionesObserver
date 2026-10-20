# Sistema de Notificaciones con el Patrón Observer

Proyecto de **Desarrollo y Reutilización de Software** (Ciclo II, Ingeniería en Desarrollo de Software, Universidad de El Salvador, Facultad Multidisciplinaria de Occidente). Tutora: Ing. Dinora Virginia de Melara.

Aplicación de consola en **C# / .NET 10** que implementa el patrón de diseño comportamental **Observer**. Dos módulos de negocio independientes, **Productos** y **Tareas**, comunican sus eventos en tiempo de ejecución a varios observadores. Los emisores no conocen la lógica interna ni la cantidad de receptores.

## Equipo

| Carnet | Integrante |
|---|---|
| NA21005 | Navarro Aguilar, Cristian Armando |
| LG21028 | Lara Guerra, Alfredo Alexander |
| QC07003 | Quezada Campos, Salvador Enrique |
| GV22023 | García Valiente, Josué Adán |
| ML21016 | Melgares Lovo, Jeovanny Alexander |

## Objetivos

- Definir contratos genéricos `ISujeto<T>` e `IObservador<T>` con seguridad de tipos.
- Implementar los sujetos `GestorProductos` y `GestorTareas`, que notifican automáticamente al dar de alta un producto o crear una tarea.
- Crear los observadores `NotificadorEmail`, `ServicioAuditoria` y `AlertaEquipoAlmacen`, reutilizando componentes mediante varias interfaces.
- Permitir suscribir y desuscribir observadores en tiempo de ejecución.

## Arquitectura

```
SistemaNotificaciones/SistemaNotificaciones/
├── Abstracciones/   ISujeto<T>, IObservador<T>
├── Dominio/         Producto, Tarea, Prioridad
├── Sujetos/         GestorProductos, GestorTareas
├── Observadores/    NotificadorEmail, ServicioAuditoria, AlertaEquipoAlmacen
└── Program.cs       Menús de consola y composición de dependencias
```

| Rol en el patrón | Clases |
|---|---|
| Sujetos | `GestorProductos` (`ISujeto<Producto>`), `GestorTareas` (`ISujeto<Tarea>`) |
| Observadores | `NotificadorEmail` y `ServicioAuditoria` (productos **y** tareas), `AlertaEquipoAlmacen` (solo productos) |
| Datos propagados | `Producto`, `Tarea` (entidades sin lógica de notificación) |

**Flujo:** `agregarProducto()` o `crearTarea()` → el gestor guarda la entidad → `notificar()` → llama a `actualizar(datos)` en cada observador suscrito.

## Decisiones clave

1. **Genéricos** en lugar de `object` y casts: errores de tipo detectados al compilar.
2. **Modelo push:** el sujeto envía la entidad completa en `actualizar(T datos)`, lo que reduce el acoplamiento.
3. **Dominio independiente:** `Producto` y `Tarea` solo guardan estado y validan en el constructor.
4. **Observadores multi-interfaz:** una instancia de `NotificadorEmail` o `ServicioAuditoria` atiende a ambos módulos. `AlertaEquipoAlmacen` solo implementa `IObservador<Producto>`, así que el compilador rechaza suscribirla a tareas.
5. **Notificación sobre copia** (`ToList()` / `ToArray()`): un observador puede desuscribirse durante la notificación sin romper la iteración.

## Principios SOLID

| Principio | Aplicación |
|---|---|
| **S** | Cada clase tiene una sola responsabilidad: datos, gestión, un canal de notificación o la interfaz de usuario. |
| **O** | Un nuevo canal (por ejemplo SMS o Teams) es una clase nueva. No se modifican los gestores. |
| **L** | Cualquier `IObservador<T>` es intercambiable para los sujetos. |
| **I** | Interfaces mínimas: `actualizar`, y `suscribir`, `desuscribir` y `notificar`. |
| **D** | Los gestores dependen de `IObservador<T>`. La unión con las clases concretas ocurre en `Program`. |

## Requisitos

- **.NET SDK 10.0** (`dotnet --version`).
- Visual Studio con soporte para .NET 10 y archivos `.slnx`, o la CLI de .NET en cualquier terminal.

## Compilar y ejecutar

**CLI de .NET**

```powershell
cd SistemaNotificaciones/SistemaNotificaciones
dotnet build
dotnet run
```

**Visual Studio**

1. Abrir `SistemaNotificaciones/SistemaNotificaciones.slnx`.
2. Establecer `SistemaNotificaciones` como proyecto de inicio.
3. Compilar con `Ctrl+Shift+B`.
4. Ejecutar con `F5` (con depuración) o `Ctrl+F5` (sin depuración).

## Uso (menú principal)

| Opción | Función |
|---|---|
| 1 | Productos: registrar producto y ver catálogo |
| 2 | Tareas: crear tarea y listar |
| 3 | Suscripciones dinámicas: conectar y desconectar observadores en caliente |
| 4 | Demostración guiada en 5 fases |
| 5 | Salir |

**Suscripciones iniciales:** `GestorProductos` tiene email, auditoría y almacén. `GestorTareas` tiene email y auditoría.

## Pruebas recomendadas

1. **Demostración guiada:** opción **4**, secuencia explicada en cinco fases.
2. **Registro regular:** opción **1**, registrar un producto y comprobar los tres mensajes en colores distintos.
3. **Suscripciones dinámicas:** opción **3**, desuscribir `NotificadorEmail` de productos y registrar otro producto. Auditoría y almacén responden, y el correo se omite.

## Conclusión

El patrón Observer permitió comunicar los módulos sin acoplar los gestores de negocio a los sistemas de notificación o auditoría. Con interfaces genéricas, los observadores se reutilizan para distintos eventos (`Producto` y `Tarea`) sin duplicar clases ni usar conversiones inseguras.
