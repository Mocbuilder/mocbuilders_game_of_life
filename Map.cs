using System.Text.Json;

namespace conways_game_of_life
{
    public class Map
    {
        public int sizeX { get; set; } = 10;
        public int sizeY { get; set; } = 10;
        public int numStartLiveCells { get; set; } = 6;
        public List<Cell> _cells { get; set; } = new List<Cell>();
        private List<Cell> _nextGenCells { get; set; } = new List<Cell>();
        private List<Cell> _oldGenCells { get; set; } = new List<Cell>();
        private string baseDumpFolderPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ConwaysGameOfLifeDumps");
        public string dumpFolderPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ConwaysGameOfLifeDumps");
        public bool enableAutonomousGeneration { get; set; } = false;
        public bool enableAutonomousSurvival { get; set; } = false;
        public int autonomousGenerationRuns { get; set; } = 10;

        private Random _random = new Random();

        public Map()
        {
            dumpFolderPath = Path.Combine(dumpFolderPath, _random.Next().ToString());

            SetupCells();
            SetupRandomLiveCells();
        }

        public Map(int SizeX, int SizeY, string DumpFolderPath, bool EnableAutonomousGeneration, bool EnableAutonomousSurvival, int AutonomousGenerationRuns)
        {
            sizeX = SizeX;
            sizeY = SizeY;
            dumpFolderPath = Path.Combine(baseDumpFolderPath, _random.Next().ToString());
            enableAutonomousGeneration = EnableAutonomousGeneration;
            enableAutonomousSurvival = EnableAutonomousSurvival;
            autonomousGenerationRuns = AutonomousGenerationRuns;

            SetupCells();
            SetupRandomLiveCells();
        }

        public Map(int SizeX, int SizeY, List<Cell> cells)
        {
            sizeX = SizeX;
            sizeY = SizeY;
            _cells = cells;
            dumpFolderPath = Path.Combine(dumpFolderPath, _random.Next().ToString());

            SetupRandomLiveCells();
        }

        private void SetupCells()
        {
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    _cells.Add(new Cell(x, y));
                }
            }
        }

        private void SetupRandomLiveCells()
        {
            // Pick a completely random starting anchor anywhere on the map,
            // leaving a 1-cell buffer so the cluster offsets don't immediately crash into boundaries.
            int centerX = _random.Next(1, sizeX - 1);
            int centerY = _random.Next(1, sizeY - 1);

            // Turn on the random anchor cell
            var centerCell = _cells.FirstOrDefault(obj => obj.coorX == centerX && obj.coorY == centerY);
            if (centerCell != null) centerCell.isLive = true;

            // Spawn the remaining live cells tightly clustered within a 3x3 area around that anchor
            int spawned = 1;
            while (spawned < numStartLiveCells)
            {
                int offsetX = _random.Next(-1, 2); // -1, 0, or 1
                int offsetY = _random.Next(-1, 2);

                int targetX = (centerX + offsetX + sizeX) % sizeX; // Handles wrapping safely if it hits an edge
                int targetY = (centerY + offsetY + sizeY) % sizeY;

                var targetCell = _cells.FirstOrDefault(obj => obj.coorX == targetX && obj.coorY == targetY);
                if (targetCell != null && !targetCell.isLive)
                {
                    targetCell.isLive = true;
                    spawned++;
                }
            }
        }

        public void Draw()
        {
            if (enableAutonomousGeneration)
            {
                if (autonomousGenerationRuns > 0)
                {
                    autonomousGenerationRuns--;
                    DumpMapState();
                }
                else if (autonomousGenerationRuns == 0)
                {
                    Environment.Exit(0);
                }
                else if (autonomousGenerationRuns == -1)
                {
                    if (CheckMapHealth())
                    {
                        DumpMapState();
                    }
                    else
                    {
                        _cells.Clear();
                        SetupCells();
                        SetupRandomLiveCells();
                        dumpFolderPath = Path.Combine(baseDumpFolderPath, _random.Next().ToString());
                    }
                }
            }

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    Cell currentCell = _cells.FirstOrDefault(c => c.coorX == x && c.coorY == y);

                    if (currentCell.isLive)
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
            _nextGenCells = _cells.Select(c => new Cell(c.coorX, c.coorY) { isLive = c.isLive }).ToList();
            _oldGenCells = _cells.Select(c => new Cell(c.coorX, c.coorY) { isLive = c.isLive }).ToList();

            foreach (Cell cell in _cells)
            {
                List<Cell> neighbours = CellHandler.GetNeighbourCells(cell, _cells, new int[] { -1, 0, 1 }, this);
                ApplyRulesToCell(cell, neighbours);
            }
            _cells = _nextGenCells;

            if (enableAutonomousSurvival)
            {
                Cell? bestNewCell = AutoHandler.GetBestNewCell(_cells, this);
                if (bestNewCell != null)
                {
                    bestNewCell.isLive = true;
                }
            }
        }

        /*
                private List<Cell> GetNeighbourCells(Cell currentCell)
                {
                    int[] offsets = { -1, 0, 1 };
                    var neighbours = new List<Cell>();

                    foreach (int dx in offsets)
                    {
                        foreach (int dy in offsets)
                        {
                            if (dx == 0 && dy == 0) continue;

                            int neighborX = (currentCell.coorX + dx + sizeX) % sizeX;
                            int neighborY = (currentCell.coorY + dy + sizeY) % sizeY;

                            var neighbor = _cells.FirstOrDefault(obj => obj.coorX == neighborX && obj.coorY == neighborY);
                            if (neighbor != null)
                            {
                                neighbours.Add(neighbor);
                            }
                        }
                    }

                    return neighbours;
                }
        */

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
                if (cell.isLive)
                {
                    liveNeighbours++;
                }
            }

            if (currentCell.isLive && liveNeighbours < 2 || currentCell.isLive && liveNeighbours > 3)
            {
                _nextGenCells.FirstOrDefault(c => c.coorX == currentCell.coorX && c.coorY == currentCell.coorY).isLive = false;
            }

            if (!currentCell.isLive && liveNeighbours == 3)
            {
                _nextGenCells.FirstOrDefault(c => c.coorX == currentCell.coorX && c.coorY == currentCell.coorY).isLive = true;
            }
        }

        public void DumpMapState()
        {
            if (!Directory.Exists(dumpFolderPath))
            {
                Directory.CreateDirectory(dumpFolderPath);
            }
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string dumpFilePath = Path.Combine(dumpFolderPath, $"MapDump_{timestamp}_{_random.Next(0, 999999999)}.json");

            var saveData = new MapSaveData
            {
                SizeX = this.sizeX,
                SizeY = this.sizeY,
                LiveCells = _cells.Where(c => c.isLive)
                          .Select(c => new Coordinate { X = c.coorX, Y = c.coorY })
                          .ToList()
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(saveData, options);
            File.WriteAllText(dumpFilePath, jsonString);
        }

        public void LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }
            string jsonString = File.ReadAllText(filePath);
            var saveData = JsonSerializer.Deserialize<MapSaveData>(jsonString);
            if (saveData != null)
            {
                sizeX = saveData.SizeX;
                sizeY = saveData.SizeY;
                _cells.Clear();
                for (int x = 0; x < sizeX; x++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        _cells.Add(new Cell(x, y));
                    }
                }
                foreach (var liveCell in saveData.LiveCells)
                {
                    var cell = _cells.FirstOrDefault(c => c.coorX == liveCell.X && c.coorY == liveCell.Y);
                    if (cell != null)
                    {
                        cell.isLive = true;
                    }
                }
            }

            Draw();
        }

        private bool CheckMapHealth()
        {
            // If everything died, map is unhealthy
            if (!_cells.Any(c => c.isLive))
            {
                return false;
            }

            // Check if the board has frozen (no cells changed state from the old generation)
            bool hasChanged = false;
            for (int i = 0; i < _cells.Count; i++)
            {
                var current = _cells[i];
                var old = _oldGenCells.FirstOrDefault(c => c.coorX == current.coorX && c.coorY == current.coorY);

                if (old != null && current.isLive != old.isLive)
                {
                    hasChanged = true;
                    break;
                }
            }

            // If nothing changed, it's a frozen still life/oscillator loop -> unhealthy
            if (!hasChanged)
            {
                return false;
            }

            return true;
        }
    }
}