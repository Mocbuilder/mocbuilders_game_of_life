namespace mocbuilders_game_of_life
{
    public class CellHandler
    {
        public static List<Cell> GetNeighbourCells(Cell currentCell, List<Cell> cells, int[] offsets, Map map)
        {
            //int[] offsets = { -1, 0, 1 };
            var neighbours = new List<Cell>();

            foreach (int dx in offsets)
            {
                foreach (int dy in offsets)
                {
                    if (dx == 0 && dy == 0) continue;

                    int neighborX = (currentCell.coorX + dx + map.sizeX) % map.sizeX;
                    int neighborY = (currentCell.coorY + dy + map.sizeY) % map.sizeY;

                    var neighbor = cells.FirstOrDefault(obj => obj.coorX == neighborX && obj.coorY == neighborY);
                    if (neighbor != null)
                    {
                        neighbours.Add(neighbor);
                    }
                }
            }

            return neighbours;
        }
    }
}