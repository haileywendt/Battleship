using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Label = System.Windows.Forms.Label;

namespace Battleship
{
    public partial class Computer : Form
    {
        Battleship battleship;
        Options options;

        // file that keeps track of win count
        String filePath = @"C:\Users\Kento Akazawa\OneDrive\2022 Fall\Software Engineering\Battleship\text.txt";
        
        /*
         * constructor
         * @param options class which tells board size, ship size, and computer level
         */
        public Computer(Options options)
        {
            InitializeComponent();

            this.options = options;
            battleship = new Battleship(this);

            battleship.setBoardSize(options.getGrid());
            battleship.setComputerLevel(options.getCompLevel());

            // depending on the ships chosen in options class
            // it will set the minimum and maximum ship size in battleship class
            battleship.setMinAndMaxShip(options.getShips());

            battleship.makeBtn(this, false, 1);
            battleship.makeShips(this);
            battleship.makeBtn(this, true, 2);
            battleship.setShipComp();
            battleship.makeRotationBtn(this);
            battleship.makeResetBtn(this, 1);

            battleship.makeReadyBtn(this);
            battleship.readyBtnShow();
            makeLabel();
            makeWinCountLabel();
        }

        /*
         * makes label which indicates which board is for player and which one is for computer
         */
        private void makeLabel()
        {
            for(int i = 0; i < 2; i++)
            {
                Label newLabel = new Label();
                newLabel.Name = "label" + i.ToString();
                newLabel.AutoSize = true;
                if(i == 0)
                {
                    newLabel.Location = new Point((int)(battleship.getWidth() * options.getGrid() / 2), 20);
                    newLabel.Text = "Player";
                }
                else
                {
                    newLabel.Location = new Point((int)(battleship.getWidth() * (options.getGrid() + 5 + options.getGrid() / 2)), 20);
                    newLabel.Text = "Computer";
                }
                newLabel.TextAlign = ContentAlignment.MiddleCenter;
                this.Controls.Add(newLabel);
            }
        }

        /*
         * makes a label which shows the win count
         */
        private void makeWinCountLabel()
        {
            Label newLbl = new Label();
            newLbl.Name = "label";
            newLbl.AutoSize = true;
            newLbl.Location = new Point(battleship.getWidth() * (battleship.getColumns() + 1) + 25, 20);
            newLbl.Text = "Player : Computer";
            newLbl.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(newLbl);

            Label newLabel = new Label();
            newLabel.Name = "label";
            newLabel.AutoSize = true;
            newLabel.Location = new Point(battleship.getWidth() * (battleship.getColumns() + 1) + 56, 40);
            newLabel.Text = File.ReadLines(filePath).Skip(0).First() + " : " + File.ReadLines(filePath).Skip(1).First();
            newLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(newLabel);
        }

        // hides this windows form
        public void hideForm()
        {
            this.Hide();
        }
        private void Computer_Load(object sender, EventArgs e)
        {
            this.AutoSize = true;
        }
    }
}
