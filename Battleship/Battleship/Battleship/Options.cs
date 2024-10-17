using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Battleship
{
    public partial class Options : Form
    {
        Start start;

        Button[] grid = new Button[NUM_OF_OPTIONS];     // 4 options for board size
        Button[] ships = new Button[4];                 // 4 options for ship size
        Button[] compLevel = new Button[3];             // 3 options for computer level
        Button readyBtn;
        const int WIDTH = 50;                           // width of buttons
        const int HEIGHT = 35;                          // height of buttons
        const int NUM_OF_OPTIONS = 4;
        const int MIN_GRID_SIZE = 7;
        int ship;
        int size;
        int numOfLabels = 2;
        int level;                                      // computer level (0-easy, 1-medium, 2-hard)
        bool vsComputer = true;
        // all of 3 bool below must become true to move to next windows form
        bool shipClicked = false;
        bool gridClicked = false;
        bool compLevelClicked = true;
        
        /*
         * constructor
         */
        public Options(Start start)
        {
            InitializeComponent();
            this.start = start;
            setVsComputer(start.getVsComputer());

            // there is extra label and buttons for computer level if vs computer
            if (vsComputer)
            {
                numOfLabels++;
                makeBtnComp();
            }

            makeLabel();
            makeBtnGrid();
            makeBtnShips();
            makeReadyBtn();
        }

        /*
         * makes label showing which what buttons are for
         */
        private void makeLabel()
        {
            for(int i = 0; i < numOfLabels; i++)
            {
                Label newLabel = new Label();
                newLabel.Name = "label" + i.ToString();
                newLabel.AutoSize = true;
                newLabel.Location = new Point(50, (2 * i + 1) * HEIGHT + 10);
                if(i == 0)
                {
                    newLabel.Text = "Board Size: ";
                }
                else if(i == 1)
                {
                    newLabel.Text = "Ships: ";
                }
                // here only if i == 2, and only for vs computer
                else
                {
                    newLabel.Text = "Computer Level: ";
                }
                newLabel.TextAlign = ContentAlignment.MiddleLeft;
                this.Controls.Add(newLabel);
            }
        }

        // make buttons for different board size
        private void makeBtnGrid()
        {
            for (int i = 0; i < NUM_OF_OPTIONS; i++)
            {
                Button newBtn = new Button();
                newBtn.Name = "grid" + i.ToString();
                newBtn.Size = new Size(WIDTH, HEIGHT);
                newBtn.Location = new Point(150 + WIDTH * i + 30*(numOfLabels - 2), HEIGHT);
                newBtn.Click += new EventHandler(btnClickedGrid);
                newBtn.Text = (i + MIN_GRID_SIZE).ToString();
                this.Controls.Add(newBtn);
                grid[i] = newBtn;
            }
        }

        /*
         * Depending on which button is clicked
         * it will determine the board size
         */
        private void btnClickedGrid(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int i;
            // since all buttons are saved in an array
            // it will first find the button that has been clicked from the array
            for (i = 0; i < NUM_OF_OPTIONS; i++)
            {
                if (btn.Equals(grid[i]))
                {
                    break;
                }
            }
            size = i + MIN_GRID_SIZE;
            gridClicked = true;
            // if all the other buttons are clicked already, it will enable ready button
            if(shipClicked == true && compLevelClicked == true)
            {
                readyBtn.Enabled = true;
            }
        }

        // makes buttons for different ship size
        private void makeBtnShips()
        {
            for(int i = 0; i < 4; i++)
            {
                Button newBtn = new Button();
                newBtn.Name = "ship" + i.ToString();
                newBtn.Size = new Size(3*WIDTH/2, HEIGHT);
                newBtn.Location = new Point(150 + 3*WIDTH*i/2 + 30 * (numOfLabels - 2), HEIGHT * 3);
                newBtn.Click += new EventHandler(btnClickedShips);
                if(i == 0)
                {
                    newBtn.Text = "2,3,4,5";
                }
                else if(i == 1)
                {
                    newBtn.Text = "3,4,5,6";
                }
                else if(i == 2)
                {
                    newBtn.Text = "3,4,5";
                }
                else
                {
                    newBtn.Text = "4,5,6";
                }
                this.Controls.Add(newBtn);
                ships[i] = newBtn;
            }
        }

        // it will determine the ship size
        private void btnClickedShips(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int i;
            // finding clicked button from array where all buttons are saved
            for(i = 0; i < 4; i++)
            {
                if (btn.Equals(ships[i]))
                {
                    break;
                }
            }
            ship = i;
            shipClicked = true;
            // if all other buttons are clicked already, ready button will be enabled
            if(gridClicked == true && compLevelClicked == true)
            {
                readyBtn.Enabled = true;
            }
        }

        // buttons for different computer level
        private void makeBtnComp()
        {
            for(int i = 0; i < 3; i++)
            {
                Button newBtn = new Button();
                newBtn.Name = "compBtn" + i.ToString() ;
                newBtn.Size = new Size(3 * WIDTH / 2, HEIGHT);
                newBtn.Location = new Point(180 + 3 * WIDTH * i / 2, HEIGHT * 5);
                newBtn.Click += new EventHandler(compBtnClicked);
                if(i == 0)
                {
                    newBtn.Text = "Easy";
                }
                else if(i == 1)
                {
                    newBtn.Text = "Medium";
                }
                else
                {
                    newBtn.Text = "Hard";
                }
                this.Controls.Add(newBtn);
                compLevel[i] = newBtn;
            }
            compLevelClicked = false;
        }

        // sets the computer level depending on the clicked button
        private void compBtnClicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            // finding clicked button from array
            for(int i = 0; i < 3; i++)
            {
                if (btn.Equals(compLevel[i]))
                {
                    level = i;
                    break;
                }
            }
            compLevelClicked = true;
            // if all the other buttons are clicked, ready button will be enabled
            if(shipClicked == true && gridClicked == true)
            {
                readyBtn.Enabled = true;
            }
        }

        // ready buttons allows the user to go to next windows form
        private void makeReadyBtn()
        {
            Button newBtn = new Button();
            newBtn.Name = "readyBtn";
            newBtn.Size = new Size(2*WIDTH, HEIGHT);
            newBtn.Location = new Point(180, HEIGHT * (2 * numOfLabels + 1));
            newBtn.Click += new EventHandler(readyBtnClicked);
            newBtn.Text = "Ready";
            this.Controls.Add(newBtn);
            newBtn.Enabled = false;
            readyBtn = newBtn;
        }

        /*
         * Next windows form will be created and shown
         */
        private void readyBtnClicked(object sender, EventArgs e)
        {
            // if the user chooses vs computer, computer windows form will show
            if (vsComputer)
            {
                Computer computer = new Computer(this);
                computer.Show();
            }
            // if the user chooses vs player, player windows form will show
            else
            {
                Player player = new Player(this);
                player.Show();
            }
            this.Hide();
        }

        // 
        public int getGrid()
        {
            return size;
        }
        public int getShips()
        {
            return ship;
        }
        public int getCompLevel()
        {
            return level;
        }
        private void setVsComputer(bool vsComputer)
        {
            this.vsComputer = vsComputer;
        }
        private void Options_Load(object sender, EventArgs e)
        {
            this.AutoSize = true;
        }
    }
}
