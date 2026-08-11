namespace mocbuilders_game_of_life
{
    public class Cell
    {
        public int coorX { get; set; } = -1;
        public int coorY { get; set; } = -1;
        public bool isLive = false;

        public Cell()
        {
        }

        public Cell(int x, int y)
        {
            coorX = x;
            coorY = y;
        }
    }
}