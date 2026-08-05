namespace conways_game_of_life
{
    public class Cell
    {
        public int coorX = 0;
        public int coorY = 0;
        public bool isLive = false;

        public Cell(int x, int y)
        {
            coorX = x;
            coorY = y;
        }
    }
}