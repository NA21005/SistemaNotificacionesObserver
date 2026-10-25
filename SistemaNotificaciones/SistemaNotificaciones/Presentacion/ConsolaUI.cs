namespace SistemaNotificaciones.Presentacion
{
    /// <summary>
    /// Utilidades de presentación para la consola: limpieza segura de pantalla, encabezados,
    /// mensajes de estado y pausas. Mantiene a <see cref="Program"/> centrado en la lógica de menús.
    /// </summary>
    internal static class ConsolaUI
    {
        private const string LineaGruesa = "════════════════════════════════════════════════════════════════════════";
        private const string LineaFina = "────────────────────────────────────────────────────────────────────────";

        /// <summary>Establece el título de la ventana si la salida no está redirigida.</summary>
        public static void EstablecerTitulo(string titulo)
        {
            try
            {
                if (!Console.IsOutputRedirected)
                {
                    Console.Title = titulo;
                }
            }
            catch (IOException) { }
            catch (PlatformNotSupportedException) { }
        }

        /// <summary>Limpia la pantalla si la salida no está redirigida.</summary>
        public static void Limpiar()
        {
            try
            {
                if (!Console.IsOutputRedirected)
                {
                    Console.Clear();
                }
            }
            catch (IOException) { }
        }

        /// <summary>Muestra un recuadro doble con una o más líneas de texto centradas.</summary>
        public static void Banner(ConsoleColor color, params string[] lineas)
        {
            Console.ForegroundColor = color;
            Console.WriteLine("╔" + LineaGruesa + "╗");
            foreach (string linea in lineas)
            {
                Console.WriteLine("║" + linea.PadRight(LineaGruesa.Length) + "║");
            }
            Console.WriteLine("╚" + LineaGruesa + "╝");
            Console.ResetColor();
        }

        /// <summary>Muestra el encabezado de un submódulo.</summary>
        public static void EncabezadoModulo(string titulo, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine("┌" + new string('─', 72) + "┐");
            Console.WriteLine("│ " + titulo.PadRight(70) + " │");
            Console.WriteLine("└" + new string('─', 72) + "┘");
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>Muestra la franja que precede a la respuesta de los observadores.</summary>
        public static void EncabezadoDisparo(string evento)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(LineaFina);
            Console.WriteLine($" Disparando evento {evento}...");
            Console.WriteLine(" Observadores suscritos respondiendo a la notificación:");
            Console.WriteLine(LineaFina);
            Console.ResetColor();
        }

        /// <summary>Escribe ACTIVO o INACTIVO con su color y restaura el gris de la franja de estado.</summary>
        public static void EscribirEstado(bool activo)
        {
            Console.ForegroundColor = activo ? ConsoleColor.Green : ConsoleColor.Red;
            Console.Write(activo ? "ACTIVO" : "INACTIVO");
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }

        /// <summary>Escribe una línea con el color indicado y restaura el color original.</summary>
        public static void Linea(string texto, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(texto);
            Console.ResetColor();
        }

        public static void Exito(string mensaje) => Linea($" ✔ {mensaje}", ConsoleColor.Green);

        public static void Error(string mensaje) => Linea($" ✖ {mensaje}", ConsoleColor.Red);

        /// <summary>Muestra un aviso de cambio de suscripción con una breve pausa visual.</summary>
        public static void AvisoCambio(string mensaje)
        {
            Console.WriteLine();
            Exito(mensaje);
            Thread.Sleep(300);
        }

        /// <summary>Muestra un paso de la demostración y espera ENTER.</summary>
        public static void PasoGuiado(string descripcionPaso)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($" >> PASO: {descripcionPaso} << ");
            Console.ResetColor();
            Console.WriteLine(" Presione [ENTER] para ejecutar este paso...");
            Console.ReadLine();
        }

        /// <summary>Espera a que el usuario presione una tecla (o ENTER si la entrada está redirigida).</summary>
        public static void Pausar()
        {
            Console.WriteLine();
            Linea(" Presione [ENTER] o cualquier tecla para continuar...", ConsoleColor.DarkGray);
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
            catch (InvalidOperationException) { }
        }
    }
}
