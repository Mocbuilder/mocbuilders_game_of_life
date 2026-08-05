namespace conways_game_of_life
{
    public class Map
    {
        public int sizeX {get; set;} = 10;
        public int sizeY {get; set;} = 10;
        public static List<Cell> _cells {get; set;} = new List<Cell>();

        private Random _random = new Random();

        public Map()
        {
            SetupCells();
            SetupRandomLiveCells();
        }

        public Map(int SizeX, int SizeY, List<Cell> cells)
        {
            sizeX = SizeX;
            sizeY = SizeY;
            _cells = cells;

            SetupRandomLiveCells();
        }

        private void SetupCells()
        {
            for(int x = 0; x < 10; x++)
            {
                for(int y = 0; y < 10; y++)
                {
                    _cells.Add(new Cell(x, y));
                }
            }
        }

        private void SetupRandomLiveCells()
        {
            for(int i= 0; i < 6; i++)
            {
                var randomCoors = GetUnusedRandomCoordinates(0, 9);
                Cell currentCell = _cells.FirstOrDefault(obj => obj.coorX == randomCoors.coorX && obj.coorY == randomCoors.coorY);
                currentCell.isLive = true;
            }
        }

        private (int coorX, int coorY) GetUnusedRandomCoordinates(int min, int max)
        {
            int tempX = 0;
            int tempY = 0;
            while(true)
            {
                tempX = _random.Next(min, max);
                tempY = _random.Next(min, max);
                
                if(_cells.FirstOrDefault(obj => obj.coorX == tempX && obj.coorY == tempY && obj.isLive) == null)
                {
                    break;
                }
            }

            return (tempX, tempY);
        }

        public void Draw()
        {
            for(int y = 0; y < sizeY; y++)
            {
                for(int x = 0; x < sizeX; x++)
                {
                    Cell currentCell = _cells.FirstOrDefault(c => c.coorX == x && c.coorY == y);

                    if(currentCell.isLive)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("███");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("███");
                        Console.ResetColor();
                    }
                }
                Console.WriteLine();
            }
        }

        public void DoNextGeneration()
        {
            foreach(Cell cell in _cells)
            {
                List<Cell> neighbours = GetNeighbourCells(cell);
                ApplyRulesToCell(cell, neighbours);
            }
        }

        private static List<Cell> GetNeighbourCells(Cell currentCell)
        {
            return new List<Cell>
            {
                _cells.FirstOrDefault(obj => obj.coorX == currentCell.coorX     && obj.coorY == currentCell.coorY + 1),
                _cells.FirstOrDefault(obj => obj.coorX == currentCell.coorX + 1 && obj.coorY == currentCell.coorY + 1),
                _cells.FirstOrDefault(obj => obj.coorX == currentCell.coorX - 1 && obj.coorY == currentCell.coorY + 1),
                _cells.FirstOrDefault(obj => obj.coorX == currentCell.coorX     && obj.coorY == currentCell.coorY - 1),
                _cells.FirstOrDefault(obj => obj.coorX == currentCell.coorX + 1 && obj.coorY == currentCell.coorY - 1),
                _cells.FirstOrDefault(obj => obj.coorX == currentCell.coorX - 1 && obj.coorY == currentCell.coorY - 1),
                _cells.FirstOrDefault(obj => obj.coorX == currentCell.coorX + 1 && obj.coorY == currentCell.coorY),
                _cells.FirstOrDefault(obj => obj.coorX == currentCell.coorX - 1 && obj.coorY == currentCell.coorY)
            }
            .Where(cell => cell != null)
            .ToList();
        }

        private void ApplyRulesToCell(Cell currentCell, List<Cell> neighbours)
        {
            /* Rule 1, 2, 3 and 4: 
                1. Any live cell with fewer than two live neighbours dies, as if by underpopulation.
                2. Any live cell with two or three live neighbours lives on to the next generation.
                3. Any live cell with more than three live neighbours dies, as if by overpopulation.
                4. Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
            */

            int liveNeighbours = 0;
            foreach (Cell cell in neighbours)
            {
                if(cell.isLive)
                {
                    liveNeighbours++;
                }
            }

            if(currentCell.isLive && liveNeighbours < 2 || currentCell.isLive && liveNeighbours > 3)
            {
                currentCell.isLive = false;
            }

            if(!currentCell.isLive && liveNeighbours == 2)
            {
                currentCell.isLive = true;
            }
        }
    }
}