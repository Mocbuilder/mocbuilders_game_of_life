// See https://aka.ms/new-console-template for more information
namespace conways_game_of_life
{
    public class Program
    {
        public static void Main()
        {
            Map map = new Map();
            map.Draw();

            Console.WriteLine("Press ENTER for next generation. Press R for Restart. Press Ctrl+C to exit.");

            while (true)
            {
               
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if(keyInfo.Key == ConsoleKey.Enter)
                {
                    map.DoNextGeneration();
                    Console.Clear();
                    map.Draw();
                }
                if (keyInfo.Key == ConsoleKey.R)
                {
                    map = new Map();
                    Console.Clear();
                    map.Draw();
                }
                Console.WriteLine("Press ENTER for next generation. Press Ctrl+C to exit.");
            }
        }
    }
}
