using System;
using System.IO;

public static class Logger
{
    private static string filePath = "log.txt"; // Ruta predeterminada del archivo de log

    public static void Log(string? message, params object?[] args)
    {
        // Validar la existencia del archivo
        if (!File.Exists(filePath))
        {
            // Si el archivo no existe, crearlo
            using (FileStream fs = File.Create(filePath))
            {
                // El archivo se ha creado
            }
        }

        // Guardar el mensaje en el archivo
        using (StreamWriter sw = new StreamWriter(filePath, true))
        {
            sw.WriteLine($"{DateTime.Now}: {message}"); // Añadir una marca de tiempo
        }

        // Notificar el mensaje en la consola
        Console.WriteLine(message);
    }
}