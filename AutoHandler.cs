namespace conways_game_of_life
{
    public class AutoHandler
    {
        public static Cell? GetBestNewCell(List<Cell> cells, Map map)
        {
            // Find live cells that are about to die from underpopulation (only 1 live neighbor)
            foreach (Cell liveCell in cells.Where(c => c.isLive))
            {
                var neighbours = CellHandler.GetNeighbourCells(liveCell, cells, new int[] { -1, 0, 1 }, map);
                var liveNeighbours = neighbours.Where(n => n.isLive).ToList();

                // Rule 1 check: If it has exactly 1 live neighbor, it's about to die next generation
                if (liveNeighbours.Count == 1)
                {
                    Cell neighborCell = liveNeighbours.First();

                    // Find a dead cell that is adjacent to BOTH the dying liveCell and its neighbor
                    // This creates a triangle/cluster that forces them to survive or spawn new life.
                    var sharedDeadNeighbour = cells.FirstOrDefault(c =>
                        !c.isLive &&
                        Math.Abs(c.coorX - liveCell.coorX) <= 1 && Math.Abs(c.coorY - liveCell.coorY) <= 1 &&
                        Math.Abs(c.coorX - neighborCell.coorX) <= 1 && Math.Abs(c.coorY - neighborCell.coorY) <= 1
                    );

                    if (sharedDeadNeighbour != null)
                    {
                        return sharedDeadNeighbour; // Return this spot for our autonomous cell drop!
                    }
                }
            }

            return null; // No rescue targets found this generation
        }
    }
}