using System.CommandLine;

namespace conways_game_of_life
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Map map = new Map();
            string dumpBasefolder = "";
            int generationsCount = 10;
            bool enableAutonomousGeneration = false;
            bool enableAutonomousSurvival = false;

            var fileToReadArgument = new Argument<string>("file-to-read")
            {
                Description = "Path of the File to read"
            };

            var dumpFolderPathArgument = new Argument<string>("dumpfolder")
            {
                Description = "Root Folder for Map Save Dumps"
            };

            var dumpFolderPathOption = new Option<string>("--dumpfolder")
            {
                Description = "Root Folder for Map Save Dumps",
                Required = false,
                DefaultValueFactory = _ => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ConwaysGameOfLifeDumps")
            };

            var enableAutonomousSurvivalOption = new Option<bool>("--enable-autonomous-survival")
            {
                Description = "Enable autonomous survival mode. If enabled, the simulation will add one cell every generation in a autonomously chosen best location to try and survive as long as possible.",
                Required = false,
                DefaultValueFactory = _ => false
            };

            var generationsCountArgument = new Argument<int>("generations-count")
            {
                Description = "Number of generations to run. '-1' is unlimited."
            };

            var sandboxCommand = new Command("sandbox", "Run the simulation in sandbox mode");
            sandboxCommand.Options.Add(dumpFolderPathOption);
            sandboxCommand.Options.Add(enableAutonomousSurvivalOption);

            var autonomousCommand = new Command("autonomous", "Run the simulation in autonomous mode");
            autonomousCommand.Arguments.Add(dumpFolderPathArgument);
            autonomousCommand.Options.Add(enableAutonomousSurvivalOption);
            autonomousCommand.Arguments.Add(generationsCountArgument);

            var readCommand = new Command("read", "Read a Map Save Dump file");
            readCommand.Arguments.Add(fileToReadArgument);

            var rootCommand = new RootCommand("Conway's Game of Life")
            {
                sandboxCommand,
                autonomousCommand,
                readCommand
            };

            sandboxCommand.SetAction(parseResult =>
            {
                enableAutonomousSurvival = parseResult.GetValue(enableAutonomousSurvivalOption);
                dumpBasefolder = parseResult.GetValue(dumpFolderPathOption)!;

                Console.Clear();
                Console.WriteLine("Sandbox Mode");
                Console.WriteLine($"Settings: \nBase Dump folder path: {dumpBasefolder}\nEnable autonomous survival: {enableAutonomousSurvival}");
                Console.WriteLine("Continue ? [Y/N]");
                if (Console.ReadKey().Key != ConsoleKey.Y)
                {
                    Environment.Exit(0);
                }

                Console.Clear();
                map.Draw();
            });

            autonomousCommand.SetAction(parseResult =>
            {
                enableAutonomousSurvival = parseResult.GetValue(enableAutonomousSurvivalOption);
                enableAutonomousGeneration = true;
                dumpBasefolder = parseResult.GetValue(dumpFolderPathArgument)!;
                generationsCount = parseResult.GetValue(generationsCountArgument);

                Console.Clear();
                Console.WriteLine("Autonomous Mode");
                Console.WriteLine($"Settings: \nBase Dump folder path: {dumpBasefolder}\nNumber of Generations: {generationsCount}");
                Console.WriteLine("Continue ? [Y/N]");
                if (Console.ReadKey().Key != ConsoleKey.Y)
                {
                    Environment.Exit(0);
                }

                Console.Clear();
                map = new Map(dumpBasefolder, enableAutonomousGeneration, enableAutonomousSurvival, generationsCount);
                map.Draw();
            });

            readCommand.SetAction(parseResult =>
            {
                Console.Clear();
                string filePath = parseResult.GetValue(fileToReadArgument)!;
                map.LoadFromFile(filePath);
                map.Draw();
            });

            var parseResult = rootCommand.Parse(args);

            if (parseResult.Errors.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("This executable needs to be run via cmd or powershell. You also need to specify a valid mode.\n");
                Console.ResetColor();

                rootCommand.Parse(new[] { "--help" }).Invoke();
                return 1;
            }

            parseResult.Invoke();

            Console.WriteLine("Press ENTER for next generation. Press R for Restart. Press S for Save. Press Ctrl+C to exit.");

            while (true)
            {
                if (map.enableAutonomousGeneration)
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