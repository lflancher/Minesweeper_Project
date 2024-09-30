using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InClassChallenge4._5
{
    public class SpecialButton : Button
    {
        int row;
        int col;

        public int Row { get => row; set => row = value; }
        public int Col { get => col; set => col = value; }

        public void HideAdjacent(int row, int col)
        {
            
        }
        
    }

   
}
