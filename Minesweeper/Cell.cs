using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InClassChallenge4._5
{
    public partial class Cell : UserControl
    {

        public event EventHandler<CellClickEventArgs> CellClicked;
        public event EventHandler<CellMClickEventArgs> CellMClicked;

        Button cellButton;
        Panel cellPanel;
        TextBox cellTextBox;
        Color cellColor;
        int row;
        int column;
        public Cell()
        {
            InitializeComponent();
            InstantiateCell();
        }


        public void InstantiateCell()
        {
            cellButton = new Button();
            cellTextBox = new TextBox();
            cellButton.Size = new Size(this.Size.Width, this.Size.Height);
            CellButton.Click += CellButtonClickHandler;
            CellButton.MouseDown += CellButtonMouseDownHandler;
            CellTextBox.MouseDown += CellTextBoxMouseDownHandler;
            this.Controls.Add(cellButton);
            this.Controls.Add(cellTextBox);
            cellPanel = new Panel();
            cellPanel.Size = new Size(this.Size.Width, this.Size.Height);
            this.Controls.Add(cellPanel);
            
        }

        public void SetColor(Color newColor)
        {
            cellColor = newColor;
        }
        public void CellButtonMouseDownHandler(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (cellButton.BackColor == Color.OrangeRed) {
                    cellButton.BackColor = Color.Gray;
                }
                else
                {
                    cellButton.BackColor = Color.OrangeRed;
                }

            }

            else if (e.Button == MouseButtons.Left && cellButton.BackColor != Color.OrangeRed)
            {
                cellButton.BackColor = Color.SlateGray;
            }



        }
        public void CellButtonClickHandler(object sender, EventArgs e) 
        {
            if (cellButton.BackColor != Color.OrangeRed)
            {
                cellButton.Visible = false;
                CellClickEventArgs args = new CellClickEventArgs(Col, Row);
                OnCellClicked(this, args);
            }
        }

        public void CellTextBoxMouseDownHandler(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                cellTextBox.ContextMenuStrip = new ContextMenuStrip();
            }
            if (e.Button == MouseButtons.Middle || (e.Button == MouseButtons.Left && e.Button == MouseButtons.Right))
            {
                CellMClickEventArgs args = new CellMClickEventArgs(Col, Row);
                OnCellMClicked(this, args);
            }
        }


        protected virtual void OnCellClicked(object sender, CellClickEventArgs e)
        {
            if(CellClicked != null)
            {
                CellClicked(this, e);
            }
        }

        protected virtual void OnCellMClicked(object sender, CellMClickEventArgs e)
        {
            if(CellMClicked != null)
            {
                CellMClicked(this, e);
            }
        }
        public Button CellButton { get => cellButton; set => cellButton = value; }
        public int Row { get => row; set => row = value; }
        public int Col { get => column; set => column = value; }
        public TextBox CellTextBox { get => cellTextBox; set => cellTextBox = value; }

        public Color CellColor
        {
            get
            {
                return cellColor;
            }
            set
            {
                this.cellPanel.BackColor = value;
                cellColor = value;
            }
        }
    }
}
