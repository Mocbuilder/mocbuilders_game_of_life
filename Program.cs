// See https://aka.ms/new-console-template for more information
namespace conways_game_of_life
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Conway's Game of Life");
            Console.WriteLine("Press ENTER for Sandbox. Press A for Autonomous Mode. Press R to read a map from file.");

            string folder = "";
            int generations = 10;
            bool enableAutonomousGeneration = false;

            Map map = new Map();

            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.Enter:
                    map.Draw();
                    break;
                case ConsoleKey.A:
                    Console.Clear();
                    enableAutonomousGeneration = true;
                    Console.WriteLine("Autonomous Mode");
                    Console.WriteLine("Enter folder for dumps, leave blank for default (Desktop):");
                    string folderTemp = Console.ReadLine();
                    if(!string.IsNullOrEmpty(folderTemp) && Directory.Exists(folderTemp))
                    {
                        folder = folderTemp;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Using default (Desktop).");
                        folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ConwaysGameOfLifeDumps");
                    }
                    Console.WriteLine("Enter number of generations to run, leave blank for default (10):");
                    string generationsInput = Console.ReadLine();
                    int generationsTemp = 10;
                    if(!string.IsNullOrEmpty(generationsInput))
                    {
                        if(!int.TryParse(generationsInput, out generations))
                        {
                            Console.WriteLine("Invalid input. Using default (10).");
                            generations = 10;
                        }
                    }

                    Console.WriteLine("Press ENTER to start Autonomous Mode.");
                    Console.ReadLine();
                    map = new Map(folder, enableAutonomousGeneration, generations);
                    map.Draw();
                    break;
                case ConsoleKey.R:
                    Console.Clear();
                    Console.WriteLine("Enter file to read map from:");
                    string folderInput = Console.ReadLine();
                    if(!string.IsNullOrEmpty(folderInput) && Path.Exists(folderInput))
                    {
                        Map loadingMap = new Map();
                        loadingMap.LoadFromFile(folderInput);
                        map = loadingMap;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input.");
                    }
                    break;
                default:
                    Console.WriteLine("Invalid input. Press ENTER for Sandbox. Press A for Autonomous Mode.");
                    Console.ReadLine();
                    break;
            }

            Console.WriteLine("Press ENTER for next generation. Press R for Restart. Press S for Save. Press Ctrl+C to exit.");

            while (true)
            {
                if(map.enableAutonomousGeneration)
                {
                    map.DoNextGeneration();
                    Console.Clear();
                    map.Draw();
                    continue;
                }

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.Enter:
                        map.DoNextGeneration();
                        Console.Clear();
                        map.Draw();
                        break;
                    case ConsoleKey.R:
                        map = new Map();
                        Console.Clear();
                        map.Draw();
                        break;
                    case ConsoleKey.S:
                        map.DumpMapState();
                        Console.Clear();
                        map.Draw();
                        break;
                    default:
                        Console.Clear();
                        map.Draw();
                        break;
                }
                Console.WriteLine("Press ENTER for next generation. Press R for Restart. Press S for Save. Press Ctrl+C to exit.");
            }
        }
    }
}
