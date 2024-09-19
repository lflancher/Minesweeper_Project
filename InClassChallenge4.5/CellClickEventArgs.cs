using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InClassChallenge4._5
{
    public class CellClickEventArgs: EventArgs
    {
        int col;
        int row;

        public CellClickEventArgs()
        {

        }

        public CellClickEventArgs(int col, int row)
        {
            Col = col;
            Row = row;
        }

        public int Col { get => col; set => col = value; }
        public int Row { get => row; set => row = value; }
    }
}
