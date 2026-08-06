using System.CommandLine;

namespace conways_game_of_life
{
    public class Program
    {
        public static int Main(string[] args)
        {
            int sizeX = 10;
            int sizeY = 10;
            string dumpBasefolder = "";
            int generationsCount = 10;
            bool enableAutonomousGeneration = false;
            bool enableAutonomousSurvival = false;
            Map? map = null;

            var fileToReadArgument = new Argument<string>("file-to-read")
            {
                Description = "Path of the File to read"
            };

            var dumpFolderPathArgument = new Argument<string>("dumpfolder")
            {
                Description = "Root Folder for Map Save Dumps"
            };

            var generationsCountArgument = new Argument<int>("generations-count")
            {
                Description = "Number of generations to run. '-1' is unlimited."
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

            var sizeXOption = new Option<int>("--sizex")
            {
                Description = "Width of the map",
                Required = false,
                DefaultValueFactory = _ => 10
            };

            var sizeYOption = new Option<int>("--sizey")
            {
                Description = "Height of the map",
                Required = false,
                DefaultValueFactory = _ => 10
            };

            var sandboxCommand = new Command("sandbox", "Run the simulation in sandbox mode");
            sandboxCommand.Options.Add(dumpFolderPathOption);
            sandboxCommand.Options.Add(enableAutonomousSurvivalOption);
            sandboxCommand.Options.Add(sizeXOption);
            sandboxCommand.Options.Add(sizeYOption);

            var autonomousCommand = new Command("autonomous", "Run the simulation in autonomous mode");
            autonomousCommand.Options.Add(enableAutonomousSurvivalOption);
            autonomousCommand.Options.Add(sizeXOption);
            autonomousCommand.Options.Add(sizeYOption);
            autonomousCommand.Arguments.Add(dumpFolderPathArgument);
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
                sizeX = parseResult.GetValue(sizeXOption);
                sizeY = parseResult.GetValue(sizeYOption);

                Console.Clear();
                Console.WriteLine("Sandbox Mode");
                Console.WriteLine($"Settings: \nSizeX: {sizeX}\nSizeY: {sizeY}\nBase Dump folder path: {dumpBasefolder}\nEnable autonomous survival: {enableAutonomousSurvival}");
                Console.WriteLine("Continue ? [Y/N]");
                if (Console.ReadKey().Key != ConsoleKey.Y)
                {
                    Environment.Exit(0);
                }

                Console.Clear();
                map = new Map(sizeX, sizeY, dumpBasefolder, enableAutonomousGeneration, enableAutonomousSurvival, generationsCount);
                map.Draw();
            });

            autonomousCommand.SetAction(parseResult =>
            {
                enableAutonomousSurvival = parseResult.GetValue(enableAutonomousSurvivalOption);
                enableAutonomousGeneration = true;
                dumpBasefolder = parseResult.GetValue(dumpFolderPathArgument)!;
                generationsCount = parseResult.GetValue(generationsCountArgument);
                sizeX = parseResult.GetValue(sizeXOption);
                sizeY = parseResult.GetValue(sizeYOption);

                Console.Clear();
                Console.WriteLine("Autonomous Mode");
                Console.WriteLine($"Settings: \nSizeX: {sizeX}\nSizeY: {sizeY}\nBase Dump folder path: {dumpBasefolder}\nNumber of Generations: {generationsCount}");
                Console.WriteLine("Continue ? [Y/N]");
                if (Console.ReadKey().Key != ConsoleKey.Y)
                {
                    Environment.Exit(0);
                }

                Console.Clear();
                map = new Map(sizeX, sizeY, dumpBasefolder, enableAutonomousGeneration, enableAutonomousSurvival, generationsCount);
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

            // Invoke the command action first so 'map' gets instantiated
            parseResult.Invoke();

            if (map == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You must specify a valid command (sandbox, autonomous, or read).\n");
                Console.ResetColor();
                return 1;
            }

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
                        break;
                    case ConsoleKey.R:
                        map = new Map(sizeX, sizeY, dumpBasefolder, enableAutonomousGeneration, enableAutonomousSurvival, generationsCount);
                        break;
                    case ConsoleKey.S:
                        map.DumpMapState();
                        break;
                    default:
                        break;
                    }
                    Console.Clear();
                    map.Draw();
                Console.WriteLine("Press ENTER for next generation. Press R for Restart. Press S for Save. Press Ctrl+C to exit.");
                }
            }
        }
    }