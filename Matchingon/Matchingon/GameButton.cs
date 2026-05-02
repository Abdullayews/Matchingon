using System.Drawing;
using System.Windows.Forms;

namespace Matchingon
{
    public class GameButton : Button
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int CardIndex { get; set; }
        public Image FrontImage { get; set; }
        public bool IsFlipped { get; set; }
        public bool IsMatched { get; set; }

        public GameButton(int row, int col)
        {
            Row = row;
            Col = col;
            IsFlipped = false;
            IsMatched = false;
        }
    }
}