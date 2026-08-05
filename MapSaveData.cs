using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace conways_game_of_life
{
    public class MapSaveData
    {
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public List<Coordinate> LiveCells { get; set; } = new();
    }
}
