using System;
using System.Text.RegularExpressions;

namespace Elevator
{
    class Program
    {
        static void Main(string[] args)
        {
            ElevatorController controller = null;

            Console.WriteLine("Command-line syntax: app.exe <floors number> <floor height> <elevator speed> <doors open time>");
            if (args.Length == 4)
            {
                controller = new ElevatorController(int.Parse(args[0]), decimal.Parse(args[1]), decimal.Parse(args[2]), int.Parse(args[3]));
            }
            else
            {
                int floorsNumber = ReadIntValueFromConsole("Enter floors number in building");
                int doorsOpenTime = ReadIntValueFromConsole("Enter doors open time (in seconds)") * 1000;
                decimal floorHeight = ReadDecimalValueFromConsole("Enter floor height (in meters)");
                decimal speed = ReadDecimalValueFromConsole("Enter elevator's movement speed (in meters per second)");

                controller = new ElevatorController(floorsNumber, floorHeight, speed, doorsOpenTime);
            }

            Regex rex = new Regex("([e|f]{1})([\\d]+)");

            string input;
            do
            {
                input = Console.ReadLine();
                if (input != null)
                {
                    Match m = rex.Match(input);
                    if (m.Success)
                    {
                        Console.WriteLine($"Source: {(m.Groups[1].Value.Equals("e") ? "[elevator]" : "[floor]")}, Requested floor: {m.Groups[2].Value}");
                        controller.CommandInitiated(new UserElevatorCommand(int.Parse(m.Groups[2].Value), m.Groups[1].Value.Equals("e") ? CommandSources.Elevator : CommandSources.Floor));
                    } else 
                        Console.WriteLine($"Unrecognized command \"{input}\"");
                }
            } while (input == null || !input.Equals("Exit", StringComparison.InvariantCultureIgnoreCase));
        }

        #region - private methods -

        /// <summary>
        /// Reads int value from command line
        /// </summary>
        /// <param name="text">Hint text</param>
        /// <returns>Input integer value</returns>
        private static int ReadIntValueFromConsole(string text)
        {
            int intValue;
            do {
                Console.Write($"{text}: ");
                string tmp = Console.ReadLine();
                
                if (!int.TryParse(tmp, out intValue))
                    Console.WriteLine($"\"{tmp}\" is not a number. Please enter numeric value.");
            } while (intValue <= 0);

            return intValue;
        }

        /// <summary>
        /// Reads decimal value from command line
        /// </summary>
        /// <param name="text">Hint text</param>
        /// <returns>Input decimal value</returns>
        private static decimal ReadDecimalValueFromConsole(string text)
        {
            decimal decimalValue;
            do {
                Console.Write($"{text}: ");
                string tmp = Console.ReadLine();
                
                if (!decimal.TryParse(tmp, out decimalValue))
                    Console.WriteLine($"\"{tmp}\" is not a number. Please enter decimal value.");
            } while (decimalValue <= 0);

            return decimalValue;
        }

        #endregion
    }
}
