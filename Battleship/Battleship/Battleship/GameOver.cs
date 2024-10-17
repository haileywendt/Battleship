using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battleship
{
    public partial class GameOver : Form
    {
        Label label;
        const int WIDTH = 200;
        const int HEIGHT = 30;
        Button[] btns = new Button[2];

        bool vsComputer;

        public GameOver()
        {
            InitializeComponent();
            makeLabel();
            makeBtn();
        }
        private void makeLabel()
        {
            Label newLabel =  new Label();
            newLabel.Name = "label";
            newLabel.AutoSize = true;
            newLabel.Location = new Point(180, 15);
            newLabel.Font = new Font("Times New Roman", 24);
            label = newLabel;
            this.Controls.Add(label);
        }
        public void setLabel(String str)
        {
            label.Text = str;
        }
        private void makeBtn()
        {
            for (int i = 0; i < 2; i++)
            {
                Button newBtn = new Button();
                newBtn.Name = "button" + i.ToString();
                newBtn.Size = new Size(WIDTH, HEIGHT);
                newBtn.Location = new Point(50 + i * (WIDTH + 50), HEIGHT*2);
                newBtn.Click += new EventHandler(btnClicked);
                if(i == 0)
                {
                    newBtn.Text = "New Game";
                }
                else
                {
                    newBtn.Text = "End";
                }
                this.Controls.Add(newBtn);
                btns[i] = newBtn;
            }
        }
        private void btnClicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Equals(btns[0]))
            {
                this.Hide();
                Start start = new Start();
                start.setVsComputer(vsComputer);
                start.Hide();
                Options options = new Options(start);
                options.Show();
            }
            else
            {
                this.Hide();
            }
        }
        public void setVsComputer(bool vsComputer)
        {
            this.vsComputer = vsComputer;
        }
        private void GameOver_Load(object sender, EventArgs e)
        {
        }
    }
}
