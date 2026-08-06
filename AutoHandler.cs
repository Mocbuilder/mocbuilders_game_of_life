namespace conways_game_of_life
{
    public class AutoHandler
    {
        public static Cell GetBestNewCell(List<Cell> cells, Map map)
        {
            Cell resultCell = null;

            foreach (Cell liveCell in cells.Where(c => c.isLive = true))
            {
                List<Cell> neighbours = CellHandler.GetNeighbourCells(liveCell, cells, new int[] { -1, 0, 1 }, map);
                int liveNeighbours = 0;
                foreach (Cell cell in neighbours)
                {
                    if (cell.isLive)
                    {
                        liveNeighbours++;
                    }
                }

                if (liveNeighbours >= 2)
                {
                    continue;
                }

                Cell neighbourCell = neighbours.FirstOrDefault(c => c.isLive);
                Cell deadNeighbourCell = neighbours.FirstOrDefault(c =>
                    c != neighbourCell &&
                    c != liveCell &&
                    Math.Abs(c.coorX - neighbourCell.coorX) <= 1 &&
                    Math.Abs(c.coorY - neighbourCell.coorY) <= 1
                );

                if (deadNeighbourCell == null)
                {
                    continue;
                }

                resultCell = deadNeighbourCell;
                break;
            }

            return resultCell;
        }
    }
}