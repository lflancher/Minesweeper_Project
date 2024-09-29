using System.Diagnostics.Metrics;
using System.Drawing;
using System.Security.AccessControl;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace InClassChallenge4._5
{
    
    public partial class Form1 : Form
    {
        /// <summary>
        /// This is where a majority of the variables used by the whole program are stored
        /// the winslosses string takes information from a file in order to find out
        /// how many times the user has won, lost and the average amount of time they
        /// spent on each game
        /// </summary>
        string winslosses = File.ReadAllText("winslosses.txt");
        Random r = new Random();
        Color bombColor = Color.Orange;
        Cell[,] guessButtons = new Cell[16, 16];
        TextBox textBox1 = new TextBox();
        TextBox textBox2 = new TextBox();
        TextBox textBox3 = new TextBox();
        StatusStrip timerStatus = new StatusStrip();
        Size buttonSize = new Size(24, 24);
        int verticalOffset = 40;
        int horizontalOffset = 35;
        int timesClicked = 0;
        int safeSquares = 0;
        int bombs = 0;
        int seconds = 0;
        int gametime;
        int gametimeaverage;
        Color color1 = Color.White;
        Color color2 = Color.Black;
        String bombIndicator = " B";
        String zeroIndicator = " 0";

        public Form1()
        {
            /// <summary>
            /// this begins the program,
            /// calls the create buttons function
            /// and prepares for the program for user 
            /// clicking on the menu
            /// </summary>
            InitializeComponent();
            CreateButtons();
            menuStrip1.Click += onMenuClick;
        }

        private void CreateButtons()
        {
            ///<summary>
            ///This is the function that creates the buttons in the game
            ///and also creates the textbox that stores the information
            ///on how many bombs and safe buttons remain.
            ///First it finds out how many cells it needs to create
            ///then it goes through row by row to create them
            ///and prepares them for being clicked on
            ///it also creates the textbox that informs the user
            ///of how many bombs and safe cells are remaining.
            ///The second text box is created in order to prevent the status strip
            ///from blocking the important text box
            /// </summary>
            for (int row = 0; row < guessButtons.GetLength(1); row++)
            {
                for (int col = 0; col < guessButtons.GetLength(0); col++)
                {
                    //Go through the grid and create new cells, new rows, change the cells' color to white
                    //And adjust the grid so that it does not overlap with the menu
                    guessButtons[col, row] = new Cell();
                    guessButtons[col, row].Row = row;
                    guessButtons[col, row].Col = col;
                    guessButtons[col, row].CellColor = Color.LightSlateGray;
                    guessButtons[col, row].BackColor = Color.Gray;
                    guessButtons[col, row].CellTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                    guessButtons[col, row].CellTextBox.SelectionStart = guessButtons[col, row].Text.Length;
                    guessButtons[col, row].CellTextBox.SelectionStart = 0;
                    //guessButtons[col, row].CellTextBox.SelectionStart = guessButtons[col, row].Text.Length;
                    //guessButtons[col, row].Font = new Font(guessButtons[col, row].Font, FontStyle.Bold);
                    guessButtons[col, row].Location = new Point(col * buttonSize.Width+horizontalOffset, row * buttonSize.Height + verticalOffset);
                    guessButtons[col, row].CellClicked += OnCellClickHandler;
                    guessButtons[col, row].CellMClicked += OnCellMClickHandler;
                    this.Controls.Add(guessButtons[col, row]);
                }
            }
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            textBox1.Location = new Point(0, 430);
            textBox2.Location = new Point(0, 450);
            textBox1.Size = new System.Drawing.Size(450, 27);
            textBox2.Size = new System.Drawing.Size(0, 0);
            this.Controls.Add(textBox1);
            this.Controls.Add(textBox2);
        }

        public void OnCellClickHandler(object sender, CellClickEventArgs e)
        {
            
            string[] winslossesratio = winslosses.Split(' ');
            int totalSquares = guessButtons.GetLength(0) * guessButtons.GetLength(1);
            int wins = Int32.Parse(winslossesratio[0]);
            int losses = Int32.Parse(winslossesratio[1]);

            

            //If the user is clicking for the first time
            if (timesClicked == 0)
            {
                
                timer1.Enabled = true;
                timer1.Start();
                int clickedRow = e.Row;
                int clickedCol = e.Col;
                //call the place bombs function
                placeBombs(clickedRow, clickedCol);
                for (int row = 0; row < guessButtons.GetLength(1); row++)
                {
                    for (int col = 0; col < guessButtons.GetLength(0); col++)
                    {
                        //Check each and every cell for bombs around it.
                        CheckBombs(col, row);
                        //Then go through and count how many bombs there are
                        if (guessButtons[col, row].CellTextBox.Text == bombIndicator)
                        {
                            bombs++;
                        }
                        //If there are no bombs around, make the cell read out
                        //nothing therefore the grid is a bit eaesier to read
                        if (guessButtons[col, row].CellTextBox.Text == zeroIndicator)
                        {
                            guessButtons[col, row].CellTextBox.ForeColor = Color.LightGray;
                        }
                    }
                }
            }
            //increment the amount of times the grid has been clicked on (this includes by the perform click function)
            timesClicked++;



            ///<summary>
            ///Make sure everything is in bounds
            ///Automatically eliminate the cells that don't have bombs around them
            ///if the user has clicked on a cell without any bombs around
            ///</summary>

                if (e.Row < guessButtons.GetLength(1) - 1 && e.Col < guessButtons.GetLength(0) - 1)
                {
                    if (guessButtons[e.Col + 1, e.Row + 1].CellTextBox.Text != bombIndicator && guessButtons[e.Col, e.Row].CellTextBox.Text == zeroIndicator)
                    {
                        guessButtons[e.Col + 1, e.Row + 1].CellButton.PerformClick();
                    }
                }

                if (e.Col > 0 && e.Row > 0)
                {
                    if (guessButtons[e.Col - 1, e.Row - 1].CellTextBox.Text != bombIndicator && guessButtons[e.Col, e.Row].CellTextBox.Text == zeroIndicator)
                    {
                        guessButtons[e.Col - 1, e.Row - 1].CellButton.PerformClick();
                    }
                }

                if (e.Row < guessButtons.GetLength(1) - 1 && e.Col > 0)
                {
                    if (guessButtons[e.Col - 1, e.Row + 1].CellTextBox.Text != bombIndicator && guessButtons[e.Col, e.Row].CellTextBox.Text == zeroIndicator)
                    {
                        guessButtons[e.Col - 1, e.Row + 1].CellButton.PerformClick();
                    }
                }

                if (e.Col < guessButtons.GetLength(0) - 1 && e.Row > 0)
                {
                    if (guessButtons[e.Col + 1, e.Row - 1].CellTextBox.Text != bombIndicator && guessButtons[e.Col, e.Row].CellTextBox.Text == zeroIndicator)
                    {
                        guessButtons[e.Col + 1, e.Row - 1].CellButton.PerformClick();
                    }
                }


                if (e.Row < guessButtons.GetLength(1) - 1)
                    {

                        if (guessButtons[e.Col, e.Row + 1].CellTextBox.Text != bombIndicator && guessButtons[e.Col, e.Row].CellTextBox.Text == zeroIndicator)
                        {
                            guessButtons[e.Col, e.Row + 1].CellButton.PerformClick();
                        }
                    }


                if (e.Col < guessButtons.GetLength(0) - 1)
                {

                    if (guessButtons[e.Col + 1, e.Row].CellTextBox.Text != bombIndicator && guessButtons[e.Col, e.Row].CellTextBox.Text == zeroIndicator)
                    {
                        guessButtons[e.Col + 1, e.Row].CellButton.PerformClick();
                    }

                }

                if (e.Row > 0)
                {
                    if (guessButtons[e.Col, e.Row - 1].CellTextBox.Text != bombIndicator && guessButtons[e.Col, e.Row].CellTextBox.Text == zeroIndicator)
                    {
                        guessButtons[e.Col, e.Row - 1].CellButton.PerformClick();

                    }
                }

                

                if (e.Col > 0)
                {
                    if (guessButtons[e.Col - 1, e.Row].CellTextBox.Text != bombIndicator && guessButtons[e.Col, e.Row].CellTextBox.Text == zeroIndicator)
                    {
                        guessButtons[e.Col - 1, e.Row].CellButton.PerformClick();

                    }
                }

                
                //lose condition
                ///<summary>
                ///If the user uncovers a cell with a bomb underneath
                ///go through the grid and show all the bombs,
                ///if the user has incorrectly flagged a safe cell
                ///then the computer also lets them know with
                ///an "X". The program will then
                ///lock the other cells, to preven the user from
                ///clicking on them and display in the textbox that 
                ///the user has lost
                ///In addition, the time is recorded and the average
                ///time is put into the file the program reads from
                ///</summary>
                if (guessButtons[e.Col, e.Row].CellTextBox.Text == bombIndicator)
                {
                    gametime = seconds;
                    timer1.Stop();
                    losses++;
                    gametimeaverage = (Int32.Parse(winslossesratio[2]) + gametime) / (wins + losses);
                    File.WriteAllText("winslosses.txt", "" + wins + " " + losses + " " + gametimeaverage);

                    for (int row = 0; row < guessButtons.GetLength(1); row++)
                    {
                        for (int col = 0; col < guessButtons.GetLength(0); col++)
                        {
                            if (guessButtons[col, row].CellTextBox.Text != bombIndicator && guessButtons[col, row].CellButton.BackColor == Color.OrangeRed)
                            {
                                guessButtons[col, row].CellButton.BackColor = Color.Gray;
                                guessButtons[col, row].CellButton.Text = "X";
                            }
                            if (guessButtons[col, row].CellTextBox.Text == bombIndicator)
                            {
                                guessButtons[col, row].CellButton.PerformClick();
                            }

                            guessButtons[col, row].CellButton.Enabled = false;
                            textBox1.Text = "Lose.";
                        }
                    }
                }
                ///<summary>
                ///if the user clicks on a cell that is not a bomb, display
                ///how many bombs are left and how many safesquares the user has found
                /// </summary>

                if (guessButtons[e.Col, e.Row].CellTextBox.Text != bombIndicator)
                {
                    safeSquares++;
                    textBox1.Text = "Safesquares found: " + safeSquares + "\n Total bombs: " + bombs;
                }
                /// <summary>
                ///win condition(s)
                ///once the user has found all the safesquares
                ///display that the user has one on the textbox below
                ///along with change all the cells with bombs under them to the
                ///color red
                ///It also disables all the cells in order
                ///to prevent the user from gaining both a win
                ///and a loss in the same game
                ///the time it took the user to complete the game
                ///is recorded and average with the time in the file.
                /// </summary>
                if (safeSquares == totalSquares - bombs)
                {
                    gametime = seconds;
                    timer1.Stop();
                    textBox1.Text = "Win! :3";
                    wins++;
                    
                    gametimeaverage = (Int32.Parse(winslossesratio[2]) + gametime) / (wins + losses);
                    File.WriteAllText("winslosses.txt", "" + wins + " " + losses + " " + gametimeaverage);
                    for (int row = 0; row < guessButtons.GetLength(0); row++)
                    {
                        for (int col = 0; col < guessButtons.GetLength(1); col++)
                        {
                            if (guessButtons[col, row].CellTextBox.Text == bombIndicator)
                            {
                                guessButtons[col, row].Enabled = false;
                                guessButtons[col, row].BackColor = Color.OrangeRed;
                            }
                        }
                    }

                }

        }

        
        public void OnCellMClickHandler(object sender, CellMClickEventArgs e)
        {
            //<summary>
            //This is the method to do what Minesweepers typically refer to as "Chording"
            //When this method is run, it means that the middle button the mouse has been pressed down
            //and it checks to make sure that there are the proper amount of flags, and then checks
            //to make sure all its future clickings are in bounds before clicking
            //</summary>
            if (guessButtons[e.Col, e.Row].CellButton.Visible == false)
            {
                if (guessButtons[e.Col, e.Row].CellTextBox.Text == " " + checkFlags(e.Col, e.Row).ToString())
                {
                    

                    if (e.Col > 0 && e.Row > 0)
                    {
                        guessButtons[e.Col - 1, e.Row - 1].CellButton.PerformClick();
                    }
                    if (e.Row > 0 && e.Col < guessButtons.GetLength(1) - 1)
                    {
                        guessButtons[e.Col + 1, e.Row - 1].CellButton.PerformClick();
                    }
                    if (e.Row < guessButtons.GetLength(1) - 1 && e.Col > 0)
                    {
                        guessButtons[e.Col - 1, e.Row + 1].CellButton.PerformClick();
                    }
                    if (e.Row < guessButtons.GetLength(1) - 1 && e.Col < guessButtons.GetLength(0) - 1)
                    {
                        guessButtons[e.Col + 1, e.Row + 1].CellButton.PerformClick();
                    }


                    if (e.Row > 0)
                    {
                        guessButtons[e.Col, e.Row - 1].CellButton.PerformClick();
                    }

                    if (e.Col > 0)
                    {
                        guessButtons[e.Col - 1, e.Row].CellButton.PerformClick();
                    }

                    if (e.Col < guessButtons.GetLength(0) - 1)
                    {
                        guessButtons[e.Col + 1, e.Row].CellButton.PerformClick();
                    }

                    if (e.Row < guessButtons.GetLength(1) - 1)
                    {
                        guessButtons[e.Col, e.Row + 1].CellButton.PerformClick();
                    }

                    
                }
            }
        }

        public void CheckBombs(int col, int row)
        {
            Color numcolor;
            ///<summary>
            ///if the cell is not a bomb itself
            ///then go around all eight spaces and check if 
            ///there is a bomb present in any of them
            ///if there is, increment the bombs around integer
            ///by one and then at the end, display that integer
            ///in the cell
            /// </summary>
            int bombsAround = 0;
            //Go around the cell and check if there are bombs around.
            if (guessButtons[col, row].CellTextBox.Text != bombIndicator)
            {

                if (row < guessButtons.GetLength(1) - 1 && col < guessButtons.GetLength(0) - 1)
                {
                    if (guessButtons[col + 1, row + 1].CellTextBox.Text == bombIndicator && guessButtons[col, row].CellTextBox.Text != bombIndicator)
                    {
                        bombsAround++;
                    }
                }

                if (row < guessButtons.GetLength(1) - 1 && col > 0)
                {
                    if (guessButtons[col - 1, row + 1].CellTextBox.Text == bombIndicator && guessButtons[col, row].CellTextBox.Text != bombIndicator)
                    {
                        bombsAround++;
                    }
                }
                if (col < guessButtons.GetLength(0) - 1 && row > 0)
                {
                    if (guessButtons[col + 1, row - 1].CellTextBox.Text == bombIndicator && guessButtons[col, row].CellTextBox.Text != bombIndicator)
                    {
                        bombsAround++;
                    }
                }

                if (col > 0 && row > 0)
                {
                    if (guessButtons[col - 1, row - 1].CellTextBox.Text == bombIndicator && guessButtons[col, row].CellTextBox.Text != bombIndicator)
                    {
                        bombsAround++;
                    }
                }

                if (row < guessButtons.GetLength(1) - 1)
                {
                    if (guessButtons[col, row + 1].CellTextBox.Text == bombIndicator && guessButtons[col, row].CellTextBox.Text != bombIndicator)
                    {
                        bombsAround++;
                    }
                }


                if (col < guessButtons.GetLength(0) - 1)
                {
                    if (guessButtons[col + 1, row].CellTextBox.Text == bombIndicator && guessButtons[col, row].CellTextBox.Text != bombIndicator)
                    {
                        bombsAround++;
                    }

                }

                if (row > 0)
                {
                    if (guessButtons[col, row - 1].CellTextBox.Text == bombIndicator && guessButtons[col, row].CellTextBox.Text != bombIndicator)
                    {
                        bombsAround++;
                    }
                }
                if (col > 0)
                {
                    if (guessButtons[col - 1, row].CellTextBox.Text == bombIndicator && guessButtons[col, row].CellTextBox.Text != bombIndicator)
                    {
                        bombsAround++;
                    }
                }
                //Make sure the numbers are the correct color
                //Space the numbers in their textboxes
                //Assign the number to their color
                numcolor = Choosecolor(bombsAround);
                guessButtons[col, row].CellTextBox.Text = " " + bombsAround.ToString();
                guessButtons[col, row].CellTextBox.ForeColor = numcolor;
                guessButtons[col, row].CellTextBox.BackColor = Color.LightGray;
                
            }

        }

        
        public int checkFlags(int col, int row)
        {
            //This is an algorithm that checks how many flags are around a clicked box and returns them
            //It does this by checking each one of the flags (within bounds) of whatever cell is passed
            int flagsaround = 0;

                if (row > 0)
                {
                    if (col > 0)
                    {
                        if (guessButtons[col - 1, row - 1].CellButton.BackColor == Color.OrangeRed)
                        {
                            flagsaround++;
                        }
                    }

                    if (col < guessButtons.GetLength(0) - 1)
                    {
                        if (guessButtons[col + 1, row - 1].CellButton.BackColor == Color.OrangeRed)
                        {
                            flagsaround++;
                        }
                    }

                    if (guessButtons[col, row - 1].CellButton.BackColor == Color.OrangeRed)
                    {
                        flagsaround++;
                    }
                }


                if (col > 0)
                {
                    if (row < guessButtons.GetLength(0) - 1)
                    {
                        if (guessButtons[col - 1, row + 1].CellButton.BackColor == Color.OrangeRed)
                        {
                            flagsaround++;
                        }
                    }

                    if (guessButtons[col - 1, row].CellButton.BackColor == Color.OrangeRed)
                    {
                        flagsaround++;
                    }
                }

                if (row < guessButtons.GetLength(1) - 1)
                {
                    if (col < guessButtons.GetLength(0) - 1)
                    {
                        if (guessButtons[col + 1, row + 1].CellButton.BackColor == Color.OrangeRed)
                        {
                            flagsaround++;
                        }
                        
                    }

                    if (guessButtons[col, row + 1].CellButton.BackColor == Color.OrangeRed)
                    {
                        flagsaround++;
                    }
                }

                if (col < guessButtons.GetLength(0) - 1)
                {
                    if (guessButtons[col + 1, row].CellButton.BackColor == Color.OrangeRed)
                    {
                        flagsaround++;
                    }
                }

            return flagsaround;
        }
        public void placeBombs(int clickedRow, int clickedCol)
        {
            ///<summary>
            ///This method places all the bombs
            ///first it sees how many bombs to place
            ///and then it randomly decides the row and column to 
            ///place each one. However, if there is already a bomb there
            ///or that is the first place the user clicked (the arguments to the method)
            ///then place the bomb somewhere else
            /// </summary>
            Random r = new Random();
            int maxbombs;
            maxbombs = 40;
            for (int bombsPlaced = 0; bombsPlaced < maxbombs; bombsPlaced++)
            {

                int bombRow = r.Next(0, 15);
                int bombCol = r.Next(0, 15);

                //Check if the bombs intersect!

                for (int i = -1; i > 2; i++)
                {
                    for (int j = -1; j > 2; j++)
                    {
                        if (bombCol + i == clickedCol + i || bombRow + j == clickedRow + j)
                        {
                            bombsPlaced--;
                        }

                    }
                }
                if (guessButtons[bombCol, bombRow].CellTextBox.Text == bombIndicator ||
                    (bombCol == clickedCol && bombRow == clickedRow))
                {
                    bombsPlaced--;
                }

                else
                {
                    guessButtons[bombCol, bombRow].CellTextBox.Text = bombIndicator;
                    guessButtons[bombCol, bombRow].CellTextBox.ForeColor = Color.Maroon;
                    guessButtons[bombCol, bombRow].CellTextBox.BackColor = Color.LightGray;
                }
            }
        }
        //A function that decides the color and that I don't have to fight with it every
        //3 seconds. Orange is a bad color for 3,so it was darkened 
        public Color Choosecolor(int danumber)
        {
            Color choosecolorcolor = Color.Fuchsia;
            switch (danumber)
            {
                case 1:
                    choosecolorcolor = Color.Blue;
                    break;
                case 2:
                    choosecolorcolor = Color.Green;
                    break;
                case 3:
                    choosecolorcolor = Color.Olive;
                    break;
                case 4:
                    choosecolorcolor = Color.DarkBlue;
                    break;
                case 5:
                    choosecolorcolor = Color.Brown;
                    break;
                case 6:
                    choosecolorcolor = Color.Teal;
                    break;
                case 7:
                    choosecolorcolor = Color.Black;
                    break;
                case 8:
                    choosecolorcolor = Color.DarkGray;
                    break;
                default:
                    break;
            }

            return choosecolorcolor;
        }


        /// <summary>
        /// These are the menu click items
        /// they handle what happens when
        /// the user selects them while they play
        /// There is also an event handler for when 
        /// the timer is ticking. It counts the seconds
        /// the user has been playing the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        public void onMenuClick(object? sender, EventArgs e)
        {
            
        }

        private void GameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        

        private void helpToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            
        }

        private void gameRestartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timesClicked = 0;
            gametime = 0;
            safeSquares = 0;
            bombs = 0;

            for (int row = 0; row < guessButtons.GetLength(1); row++)
            {
                for (int col = 0; col < guessButtons.GetLength(0); col++)
                {
                    if (guessButtons[col, row].CellTextBox.Text == bombIndicator)
                    {
                        guessButtons[col, row].CellTextBox.Clear();
                    }

                    if (guessButtons[col, row].CellButton.Visible == false)
                    {
                        guessButtons[col, row].CellButton.Visible = true;
                    }

                    guessButtons[col, row].CellButton.Enabled = true;
                }
            }
        }
            

        private void quitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("This was coded by Luke Flancher");
        }

        private void instructionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("To begin, click any button on the grid.\n" +
                "The numbers displayed indicate how many bombs surround that cell.\n" +
                "Clicking on a tile with a bomb underneath will result in a loss but clicking" +
                " on all safe buttons will result in a win.");
        }

        private void showLifetimeStatisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] winslossesratio = winslosses.Split(' ');
            System.Windows.Forms.MessageBox.Show("Wins: " + winslosses[0] +"\n" +
                "Losses: " + winslossesratio[1] + "\n" + "Average time: " + winslossesratio[2]);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            seconds++;
            toolStripStatusLabel1.Text = seconds.ToString();
        }
    }
}