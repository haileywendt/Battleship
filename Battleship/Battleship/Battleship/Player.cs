using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battleship
{
    public partial class Player : Form
    {
        Battleship battleship;
        Options options;

        Label[] labels = new Label[2];

        // file that keeps track of win count
        String filePath = @"C:\Users\Kento Akazawa\OneDrive\2022 Fall\Software Engineering\Battleship\text.txt";
        
        /*
         * constructor
         * @param options class which tells board size and ship size
         */
        public Player(Options options)
        {
            InitializeComponent();

            this.options = options;
            battleship = new Battleship(this);

            battleship.setBoardSize(options.getGrid());

            // depending on the ships chosen in options class
            // it will set the minimum and maximum ship size in battleship class
            battleship.setMinAndMaxShip(options.getShips());

            battleship.makeBtn(this, false, 1);
            battleship.makeShips(this);
            battleship.makeRotationBtn(this);
            battleship.makeResetBtn(this, 1);
            battleship.makeNextBtn(this);
            
            makeLabel();
            makeWinCountLabel();
        }

        // creates label which shows who's turn it is
        private void makeLabel()
        {
            for (int i = 0; i < 2; i++)
            {
                Label newLabel = new Label();
                newLabel.Name = "label" + i.ToString();
                newLabel.AutoSize = true;
                newLabel.Location = new Point((int)(battleship.getWidth() * options.getGrid() / 2), 20);
                newLabel.Text = "Player " + (i + 1).ToString();
                newLabel.TextAlign = ContentAlignment.MiddleCenter;
                this.Controls.Add(newLabel);
                labels[i] = newLabel;
                if(i == 1)
                {
                    newLabel.Hide();
                }
            }
        }

        /*
         * shows the label at certain index
         * @param index
         */
        public void labelShow(int i)
        {
            labels[i - 1].Show();
        }

        /*
         * hides the label at certain index
         * @param index
         */
        public void labelHide(int i)
        {
            labels[i - 1].Hide();
        }

        /*
         * Creates label which shows the win count of each player
         */
        private void makeWinCountLabel()
        {
            Label newLbl = new Label();
            newLbl.Name = "label";
            newLbl.AutoSize = true;
            newLbl.Location = new Point(battleship.getWidth() * (battleship.getColumns() + 1) + 25, 20);
            newLbl.Text = "Player 1 : Player 2";
            newLbl.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(newLbl);

            Label newLabel = new Label();
            newLabel.Name = "label";
            newLabel.AutoSize = true;
            newLabel.Location = new Point(battleship.getWidth() * (battleship.getColumns() + 1) + 67, 40);
            newLabel.Text = File.ReadLines(filePath).Skip(0).First() + " : " + File.ReadLines(filePath).Skip(1).First();
            newLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(newLabel);
        }
       
        // hides this windows form
        public void hideForm()
        {
            this.Hide();
        }
        private void Player_Load(object sender, EventArgs e)
        {
            this.AutoSize = true;
        }
    }
}
