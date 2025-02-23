using System;
using System.Collections.Generic;

class Program
{
    // Diccionario para almacenar la tarifa por hora de cada empleado
    static Dictionary<string, double> empleadosTarifas = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

    // Diccionario para almacenar las horas trabajadas por empleado (por día de la semana)
    static Dictionary<string, List<int>> empleadosHorasDiarias = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);

    static void Main(string[] args)
    {
        // Menú principal
        while (true)
        {
            Console.Clear();
            Console.WriteLine("\n--- Sistema de Cálculo de Pago ---");
            Console.WriteLine("1. Registrar empleado y jornada laboral");
            Console.WriteLine("2. Calcular salario y mostrar resumen");
            Console.WriteLine("3. Ver lista de trabajadores registrados");
            Console.WriteLine("4. Calcular horas semanales de un empleado");
            Console.WriteLine("5. Salir");
            Console.Write("Seleccione una opción: ");

            int opcion = int.Parse(Console.ReadLine());
            switch (opcion)
            {
                case 1:
                    RegistrarEmpleado();
                    break;
                case 2:
                    CalcularYMostrarPago();
                    break;
                case 3:
                    MostrarListaEmpleados();
                    break;
                case 4:
                    CalcularHorasSemanales();
                    break;
                case 5:
                    Console.WriteLine("Saliendo del sistema...");
                    return;
                default:
                    Console.WriteLine("Opción no válida. Intente de nuevo.");
                    break;
            }
        }
    }

    static void RegistrarEmpleado()
    {
        Console.Clear();
        Console.WriteLine("\n--- Registro de Empleado ---");
        Console.Write("Nombre del empleado: ");
        string nombre = Console.ReadLine();

        if (!empleadosTarifas.ContainsKey(nombre))
        {
            Console.WriteLine("Seleccione el puesto:");
            Console.WriteLine("1. Facturador (tarifa base: ₡2000/hora)");
            Console.WriteLine("2. Operario (tarifa base: ₡1500/hora)");
            Console.WriteLine("3. Gerente (tarifa base: ₡3000/hora)");
            int puesto = int.Parse(Console.ReadLine());

            double tarifaBase = 0;
            switch (puesto)
            {
                case 1:
                    tarifaBase = 2000;
                    break;
                case 2:
                    tarifaBase = 1500;
                    break;
                case 3:
                    tarifaBase = 3000;
                    break;
                default:
                    Console.WriteLine("Puesto no válido. Regresando al menú principal.");
                    return;
            }

            empleadosTarifas[nombre] = tarifaBase;
            Console.WriteLine($"Tarifa registrada para {nombre}: ₡{tarifaBase}/hora.");
        }
        else
        {
            Console.WriteLine($"El empleado {nombre} ya tiene una tarifa registrada: ₡{empleadosTarifas[nombre]}/hora.");
        }

        // Registro de las horas trabajadas durante la semana (de lunes a viernes)
        List<int> horasDiarias = new List<int>();

        for (int i = 1; i <= 5; i++) // Con jornada de 5 días de trabajo a la semana
        {
            Console.WriteLine($"Ingrese las horas trabajadas el día {i} (lunes=1, martes=2,.... viernes=5):");
            Console.Write("Hora de entrada (formato 24h) o 0 si no trabajó: ");
            int horaEntrada = int.Parse(Console.ReadLine());

            if (horaEntrada == 0)
            {
                horasDiarias.Add(0); // No trabajó ese día
                Console.WriteLine("Este día fue marcado como día libre.");
                continue;
            }

            Console.Write("Hora de salida (formato 24h): ");
            int horaSalida = int.Parse(Console.ReadLine());

            if (!ValidarHoras(horaEntrada, horaSalida))
            {
                Console.WriteLine("Error: La hora de salida no puede ser anterior a la hora de entrada.");
                return;
            }

            // Penalización por llegar tarde (si la entrada es posterior a las 8 AM)
            int horasRetraso = 0;
            if (horaEntrada > 8)
            {
                horasRetraso = horaEntrada - 8;
                Console.WriteLine($"¡Penalización! Llegaste tarde por {horasRetraso} hora(s).");
            }

            int horasTrabajadas = CalcularHorasDiarias(horaEntrada, horaSalida);
            horasDiarias.Add(horasTrabajadas);

            // Penalización por retraso (si hubo retraso, descontamos del salario)
            if (horasRetraso > 0)
            {
                Console.WriteLine($"Se aplicará una penalización por llegada tarde de {horasRetraso} horas.");
            }

            Console.WriteLine($"Horas trabajadas el día {i}: {horasTrabajadas}");
        }

        // Almacenar las horas de la semana en el diccionario
        empleadosHorasDiarias[nombre] = horasDiarias;

        Console.WriteLine("Registro completado. Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    static bool ValidarHoras(int entrada, int salida)
    {
        return salida > entrada;
    }

    static int CalcularHorasDiarias(int entrada, int salida)
    {
        return salida - entrada;
    }

    static void CalcularYMostrarPago()
    {
        Console.Clear();
        Console.WriteLine("\n--- Cálculo de Salario ---");

        Console.Write("Ingrese el nombre del empleado: ");
        string nombre = Console.ReadLine();

        if (!empleadosTarifas.ContainsKey(nombre))
        {
            Console.WriteLine($"Error: El empleado {nombre} no está registrado.");
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            return;
        }

        double tarifa = empleadosTarifas[nombre];
        double salarioBruto = 0;
        double deducciones = 0;

        // Obtener las horas trabajadas durante la semana
        List<int> horasDiarias = empleadosHorasDiarias[nombre];
        int horasSemanales = 0;

        foreach (int horas in horasDiarias)
        {
            horasSemanales += horas;
        }

        // Calcular horas extras
        int horasExtras = 0;
        if (horasSemanales > 40)
        {
            horasExtras = horasSemanales - 40;
            horasSemanales = 40; // Solo se consideran 40 horas normales
        }

        // Cálculo del salario bruto
        salarioBruto = horasSemanales * tarifa + horasExtras * tarifa * 1.5; // Las horas extras se pagan con un recargo del 50%

        // Penalización por no cumplir con el mínimo de 40 horas semanales
        if (horasSemanales < 40)
        {
            double penalizacionMinimo = (40 - horasSemanales) * tarifa;
            Console.WriteLine($"El empleado no cumplió con las 40 horas mínimas. Penalización: ₡{penalizacionMinimo}");
            salarioBruto -= penalizacionMinimo;
        }

        // Calcular deducciones y salario neto
        deducciones = AplicarDeducciones(salarioBruto);
        double salarioNeto = salarioBruto - deducciones;

        Console.WriteLine($"\nSalario Bruto: ₡{salarioBruto}");
        Console.WriteLine($"Horas extras: {horasExtras} horas.");
        Console.WriteLine($"Deducciones: ₡{deducciones}");
        Console.WriteLine($"Salario Neto: ₡{salarioNeto}");
        Console.WriteLine("Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    static double AplicarDeducciones(double salarioBruto)
    {
        const double porcentajeDeduccion = 0.1; // 10% de deducciones obligatorias
        return salarioBruto * porcentajeDeduccion;
    }

    static void MostrarListaEmpleados()
    {
        Console.Clear();
        Console.WriteLine("\n--- Lista de Trabajadores Registrados ---");

        if (empleadosTarifas.Count == 0)
        {
            Console.WriteLine("No hay empleados registrados.");
        }
        else
        {
            foreach (var empleado in empleadosTarifas)
            {
                Console.WriteLine($"Nombre: {empleado.Key}, Tarifa: ₡{empleado.Value}/hora");
            }
        }

        Console.WriteLine("Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }

    // Función para calcular las horas semanales de un empleado
    static void CalcularHorasSemanales()
    {
        Console.Clear();
        Console.WriteLine("\n--- Cálculo de Horas Semanales ---");

        Console.Write("Ingrese el nombre del empleado: ");
        string nombre = Console.ReadLine();

        if (!empleadosHorasDiarias.ContainsKey(nombre))
        {
            Console.WriteLine($"Error: El empleado {nombre} no tiene horas registradas.");
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            return;
        }

        List<int> horasDiarias = empleadosHorasDiarias[nombre];
        int horasSemanales = 0;

        // Sumar las horas trabajadas durante la semana, solo si no es 0
        foreach (int horas in horasDiarias)
        {
            horasSemanales += horas;
        }

        Console.WriteLine($"Total de horas trabajadas por {nombre} durante la semana: {horasSemanales} horas.");
        Console.WriteLine("Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }
}