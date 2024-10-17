using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battleship
{
    public partial class Start : Form
    {
        Options options;

        Button rdyBtn;
        const int WIDTH = 200;      // width of buttons
        const int HEIGHT = 30;      // height of buttons
        bool vsComputer = false;
        
        String filePath = @"C:\Users\Kento Akazawa\OneDrive\2022 Fall\Software Engineering\Battleship\text.txt";
        String[] initialWinCount = { "0", "0" };

        // constructor
        public Start()
        {
            InitializeComponent();
            makeBtn();
            readyBtn();
        }

        /*
         * creates 2 buttons
         * 1 for player vs player and the other for player vs computer
         */
        private void makeBtn()
        {
            for (int i = 0; i < 2; i++)
            {
                Button newBtn = new Button();
                newBtn.Name = "button" + i.ToString();
                newBtn.Size = new Size(WIDTH, HEIGHT);
                newBtn.Location = new Point(50 + i * (WIDTH + 50), HEIGHT);
                newBtn.Click += new EventHandler(btnClicked);
                newBtn.Text = getText(i);
                this.Controls.Add(newBtn);
            }
        }

        /*
         * When the button is clicked to choose mode
         * the bool, vsComputer changes which determines next windows form
         */
        private void btnClicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Name.Equals("button0"))
            {
                vsComputer = false;
            }
            else
            {
                vsComputer = true;
            }
            // Once the user chooses the mode, ready button will be enabled
            // which allows them to move to next windows form
            rdyBtn.Enabled = true;
        }

        /*
         * button that allows the user to move to next windows form
         */
        private void readyBtn()
        {
            Button newBtn = new Button();
            newBtn.Name = "readyBtn";
            newBtn.Size = new Size(100, 30);
            newBtn.Location = new Point(225, 80);
            newBtn.Click += new EventHandler(readyBtnClicked);
            newBtn.Text = "Ready";
            this.Controls.Add(newBtn);
            newBtn.Enabled = false;
            rdyBtn = newBtn;
        }

        /*
         * Once ready button is clicked, it will show next windows form 
         * depending whether the user choose vs player or vs computer
         */
        private void readyBtnClicked(object sender, EventArgs e)
        {
            options = new Options(this);
            options.Show();
            this.Hide();
            File.WriteAllText(filePath, string.Empty);
            File.WriteAllLines(filePath, initialWinCount);
        }

        /*
         * setter for vsComputer
         * This is used when the user chooses to play again
         * the mode must be the same
         */
        public void setVsComputer(bool vsComputer)
        {
            this.vsComputer = vsComputer;
        }

        /*
         * getter for the mode
         * @param index to tell whether the text should be vs player or vs computer
         * This is used to set text of buttons
         */
        private String getText(int i)
        {
            if (i == 0)
            {
                return "Player vs Player";
            }
            else
            {
                return "Player vs Computer";
            }
        }

        /*
         * getter for vsComputer
         * returns true if vs computer and false if vs player
         * return vsComputer
         */
        public bool getVsComputer()
        {
            return vsComputer;
        }

        private void Start_Load(object sender, EventArgs e)
        {
            this.AutoSize = true;
        }
    }
}
