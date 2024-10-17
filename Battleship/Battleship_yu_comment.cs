using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Net.Mime.MediaTypeNames;

namespace Battleship
{
    internal class Battleship
    {
        Player player;
        Computer computer;
        Random rnd = new Random();

        Color shot = Color.Red;
        Color miss = Color.Cyan;
        private Button rotationBtn;
        private Button nextBtn;
        private Button readyBtn;
        private Button swpBtn;
        private Button resetBtn;
        private Button[,] player1;
        private Button[,] player2;
        private String[,] player1Text;
        private String[,] player2Text;
        List<Button> ships = new List<Button>();

        // function pointers depending on the computer difficulty
        public delegate void funcPtrBtn(Button btn);
        public delegate void funcPtrFire(Button[,] btns);
        funcPtrFire firePtr;
        public delegate void funcPtrFireRandom(Button[,] btns);
        funcPtrFireRandom fireRandomPtr;

        const int WIDTH = 50;               // width of buttons of the board
        const int HEIGHT = 50;              // height of buttons of the board
        const String SHIP = "X";

        private int columns;
        private int rows;
        private int maxShipSize;
        private int maxShip;                // max ship size that hasn't been found (used for computer medium/hard)
        private int minShipSize;
        private int computerLevel;          // 0-easy, 1-medium, 2-hard
        private int size;                   // ship size which was clicked
        private int turn = 1;               // 1 for player1/player, 2 for player2/computer
        private int n = 0;                  // keeps track of how many ships the user has placed
        private int shot1 = 0;              // keeps track of how many times player1/player shot ship
        private int shot2 = 0;              // keeps track of how many times player2/computer shot ship
        
        // numbers that are used for computer (medium/hard) to find ships
        private int count = 0;
        private int shotTemp = 0;
        private int x = 0;
        private int y = 0;
        private int i = 0;
        private int j = 0;
        private int n1 = 0;
        private int n2 = 0;

        private bool vsComputer = false;
        private bool horizontal = true;     // whether the user wants to place the ship horizontally or vertically
        private bool setup = true;          // true during setup(when setting ships), false during the game
        private String errorMessage;        // message that shows when user clicks invalid button
        private bool horizontalCheck = true;
        private bool verticalCheck = true;

        // file that keeps track of win count
        String filePath = @"\Users\yumatsuzawa\Battleship\text.txt";
        String[] winCount = new string[2];

        // constructor when the user chooses to do player vs player
        public Battleship(Player player)
        {
            vsComputer = false;
            this.player = player;
        }
        // constructor when the user chooses to do player vs computer
        public Battleship(Computer computer)
        {
            vsComputer = true;
            this.computer = computer;
        }

        /*
         * creates buttons based on the board size that the user selected
         * and store them in btns array
         */
        public void makeBtn(Form form, bool isComputer, int turn)
        {
            Button[,] btns; //variable to store Btns
            String[,] str;  //variable to store text
            //when turn is 1, variables take player1's information
            if(turn == 1)
            {
                btns = player1;
                str = player1Text;
            }
            //otheriwise. take player2's information 
            else
            {
                btns = player2;
                str = player2Text;
            }
            //create Button widgets and 
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    Button newBtn = new Button();
                    newBtn.Name = "button" + i.ToString() + j.ToString();
                    newBtn.Size = new Size(WIDTH, HEIGHT);
                    // if the buttons for computers are been created
                    // it will display on the right of the player's board
                    if (isComputer)
                    {
                        newBtn.Location = new Point(WIDTH * (i + 3 + columns + maxShipSize - minShipSize), (j + 1) * HEIGHT);   
                        newBtn.Enabled = false; //disable button as user doe not select computer's btn
                    }
                    else
                    {
                        newBtn.Location = new Point((i + 1) * WIDTH, (j + 1) * HEIGHT); 
                    }
                    newBtn.Click += delegate (object sender, EventArgs e) { btnClicked(sender, e, btns, str, ships, isComputer); };
                    btns[i, j] = newBtn;
                    str[i, j] = "";
                    form.Controls.Add(newBtn);
                    newBtn.BackColor = default(Color);
                    // disable all buttons at the start so the user cannot click
                    // until they select a ship
                    newBtn.Enabled = false;
                }
            }
        }

        /*
         * when a button on the board has been clicked
         * it will set ships during setup
         * or it will fire at opponent during the game
         * @param sender - button that has been clicked
         * @param e
         * @param btn - array of buttons for each player
         * @param str - array containing information whether there is a ship or not
         * @param ships - to disable those ships that's been placed if the button clicked is valid
         * @bool isComputer
         */
        private void btnClicked(object sender, EventArgs e, Button[,] btns, String[,] str, List<Button> ships, bool isComputer)
        {
            // here when they are setting the ships at the start
            if (setup)
            {
                if (!isComputer)
                {
                    btnClickedSetup(sender, e, btns, str, ships);
                }
            }
            // here when they are firing to the opponent
            else
            {
                fire(sender, e, btns);
            }
        }

        /*
         * When a button is clicked during setup of the ships
         * it will check if it's valid and if it is
         * it will turn the button to red indicating that the user set the ship there
         */
        private void btnClickedSetup(object sender, EventArgs e, Button[,] btns, String[,] str, List<Button> ships)
        {
            Button btn = (Button)sender;
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    // finds the button from the array list of button to see 
                    // which button has been clicked
                    if (btn.Equals(btns[i, j]))
                    {
                        // check if the clicked button is valid
                        if (isBtnValid(i, j, str))
                        {
                            // sets the ships horizontally or vertically
                            setShip(horizontal, i, j, btns, str, false);

                            // disables the ship button after the ships been set
                            if (size >= 0)
                            {
                                ships[size - minShipSize].Enabled = false;
                            }

                            // increments the count to see if the user has place all the ships
                            n++;
                            
                            // disables all the buttons until the user select next ship
                            //if Computer vs player, diable player1's btns
                            if (vsComputer)
                            {
                                btnEnableOrDisable(player1, false);
                            }
                            //if player vs player, diable a player's btns
                            else
                            {
                                if(turn == 1)
                                {
                                    btnEnableOrDisable(player1, false);
                                }
                                else
                                {
                                    btnEnableOrDisable(player2, false);
                                }
                            }
                        }
                        // here if the clicked button is invalid
                        else
                        {
                            // shows the message box saying that it is invalid with the reasoning
                            MessageBox.Show("Invalid " + errorMessage);
                        }
                        break;
                    }
                }
            }

            // The user must use all the ships
            // Once the user place all the ships, next or ready button will be enabled
            if (!vsComputer)
            {
                //if all the ships does not set yet, enable next btn
                if (n > maxShipSize - minShipSize)
                {
                    nextBtn.Enabled = true;
                }
            }
            //if all the ships does not set, enable ready btn
            if (n > ((maxShipSize - minShipSize) * 2 + 1))
            {
                readyBtn.Enabled = true;
            }
        }

        /*
         * When a button is clicked during the game (shooting at opponent's ships)
         * it will check if it's valid and if it is
         * it will either turn the button to red indicating that there was a ship
         * or turn the button to blue indicating that there was no ship
         */
        private void fire(object sender, EventArgs e, Button[,] btns)
        {
            Button btn = (Button)sender;
            // if the clicked button has been already clicked before
            // it will show the message box saying that it is invalid
            if(btn.BackColor.Equals(shot) || btn.BackColor.Equals(miss))
            {
                MessageBox.Show("Invalid");
            }
            // here if the button clicked is valid
            else
            {
                // here if there is a ship
                if (isShip(btn))
                {
                    btn.BackColor = shot;
                    if(turn == 1)
                    {
                        shot1++;
                    }
                    else
                    {
                        shot2++;
                    }
                }
                else
                {
                    btn.BackColor = miss;
                }
                btn.ForeColor = Color.Black;

                // disable all buttons until next turn
                btnEnableOrDisable(btns, false);

                // if it's player vs computer
                // the computer will fire and it will the user's turn
                if (vsComputer)
                {
                    turn = 2;

                    // computer fire player1's board depending on comuter's difficulty
                    firePtr(player1);

                    // enables computer's buttons so that the user can shoot
                    btnEnableOrDisable(player2, true);
                    turn = 1;
                }
                // if it's player vs player
                // swap button will be enabled so that they can swap turns
                // It won't automatically swap turns until the user clicked the swap button
                else
                {
                    swpBtn.Enabled = true;
                }
            }
            // if one of the players has sank all the opponent's ships
            if(shot1 == totalShips() || shot2 == totalShips())
            {
                GameOver gameOver = new GameOver();
                gameOver.setVsComputer(vsComputer);

                // it will read the win count from a file
                // so that it can be updated
                for (int i = 0; i < 2; i++)
                {
                    winCount[i] = File.ReadAllLines(filePath).Skip(i).First();
                }

                // determines the winner
                if (shot1 >= shot2)
                {
                    // updates the win count in the array so that it can be updated in a file
                    winCount[0] = (int.Parse(winCount[0]) + 1).ToString();
                    if (vsComputer)
                    {
                        gameOver.setLabel("Player won!");
                    }
                    else
                    {
                        gameOver.setLabel("Player1 won!");
                    }
                }
                else
                {
                    winCount[1] = (int.Parse(winCount[1]) + 1).ToString();
                    if (vsComputer)
                    {
                        gameOver.setLabel("Computer won!");
                    }
                    else
                    {
                        gameOver.setLabel("Player2 won!");
                    }
                }
                // updates the win count on the file
                File.WriteAllLines(filePath, winCount);

                gameOver.Show();

                if (vsComputer)
                {
                    computer.hideForm();
                }
                else
                {
                    player.hideForm();
                }
            }
        }

        /*
         * If the computer level is easy
         * the computer will shoot randomly
         */
        private void fireCompEasy(Button[,] playerBtns)
        {
            fireRandomPtr(playerBtns);
            // when there was a ship
            if (isShip(playerBtns[x, y]))
            {
                playerBtns[x, y].BackColor = shot;
                shot2++;
            }
            // when there was no ship
            else
            {
                playerBtns[x, y].BackColor = miss;
            }
        }

        /*
         * If the computer level is medium or hard
         * it will shoot at surrounding buttons once a ship has been found
         */
        private void fireCompMedium(Button[,] playerBtns)
        {
            // here when the computer hasn't found a ship
            // so the computer will randomly shoot
            if(count == 0)
            {
                fireRandomPtr(playerBtns);
            }
            // need commenting
            else if (count == 1)
            {
                int countLoop = 0;
                // need commenting
                do
                {
                    // need commenting
                    int r1;
                    int r2 = rnd.Next() % 2;

                    // need commenting
                    if (horizontalCheck == false && verticalCheck == false)
                    {
                        r1 = rnd.Next() % 2;
                    }
                    else if (horizontalCheck == false)
                    {
                        r1 = 0;
                    }
                    else if (verticalCheck == false)
                    {
                        r1 = 1;
                    }
                    else
                    {
                        r1 = rnd.Next() % 2;
                    }
                    if (r1 == 0)
                    {
                        i = 0;
                        j = 2 * r2 - 1;
                    }
                    else
                    {
                        i = 2 * r2 - 1;
                        j = 0;
                    }
                    countLoop++;
                } while (!isValidComp(playerBtns, x + i, y + j) && countLoop < 20);

                // need commenting
                if (countLoop > 10)
                {
                    fireRandomPtr(playerBtns);
                }
                // need commenting
                else
                {
                    n1 = i;
                    n2 = j;
                    shotTemp = 1;
                }
            }
            // need commenting
            else if (count == 2)
            {
                // need commenting
                i += n1;
                j += n2;

                // need commenting
                if (!isValidComp(playerBtns, x + i, y + j))
                {
                    // need commenting
                    if (isValidComp(playerBtns, x - n1, y - n2))
                    {
                        // need commenting
                        i = -n1;
                        j = -n2;
                        n1 = i;
                        n2 = j;
                        count++;
                    }
                    // need commenting
                    else
                    {
                        count = 0;
                        shotTemp = 0;
                        fireRandomPtr(playerBtns);
                    }
                }
            }
            // need commenting
            else
            {
                // need commenting
                i += n1;
                j += n2;
                // need commenting
                if (!isValidComp(playerBtns, x + i, y + j))
                {
                    count = 0;
                    shotTemp = 0;
                    fireRandomPtr(playerBtns);
                }
            }

            // need commenting
            if (isShip(playerBtns[x + i,y + j]))
            {
                playerBtns[x + i, y + j].BackColor = shot;
                shot2++;
                shotTemp++;

                // need commenting
                if (count < 2)
                {
                    count++;
                }
                // need commenting
                if (shotTemp >= maxShip)
                {
                    count = 0;
                    shotTemp = 0;
                    maxShip -= 1;
                }
            }
            else
            {
                playerBtns[x + i, y + j].BackColor = miss;
                // need commenting
                if (count == 2)
                {
                    count++;
                    n1 = -n1;
                    n2 = -n2;
                    i = 0;
                    j = 0;
                }
                // need commenting
                else if (count == 3)
                {
                    count = 0;
                }
                // need commenting
                else
                {
                    shotTemp = 0;
                }
            }
        }

        // randomly chooses a position to shoot (for computer)
        private void randomFire(Button[,] playerBtns)
        {
            // keeps randomly chooses posion in 2d array(x,y) until it is valid
            do
            {
                x = rnd.Next(0, columns);
                y = rnd.Next(0, rows);
            } while (!isValidComp(playerBtns, x, y));
            i = 0;
            j = 0;
        }

        // somewhat randomly chooses position to shoot when the computer level is hard
        // the difference is that it won't choose a position where there is 100% no ship
        private void randomFireHard(Button[,] playerBtns)   
        {
            // need commenting
            int tempHorizontal;
            int tempVertical;
            int innerLoop;

            // need commenting
            do
            {
                innerLoop = 0;
                tempHorizontal = 0;
                tempVertical = 0;
                x = rnd.Next(0, columns);
                y = rnd.Next(0, rows);

                // need commenting
                for (int i = 0; i < maxShip; i++)
                {
                    // need commenting
                    if (isValidComp(playerBtns, x + i, y))
                    {
                        tempHorizontal++;
                    }
                    else
                    {
                        break;
                    }
                }

                // need commenting
                for (int i = 0; i < maxShip; i++)
                {
                    // need commenting
                    if (isValidComp(playerBtns, x - i, y))
                    {
                        tempHorizontal++;
                    }
                    else
                    {
                        break;
                    }
                }

                // need commenting
                for (int i = 0; i < maxShip; i++)
                {
                    if (isValidComp(playerBtns, x, y + i))
                    {
                        tempVertical++;
                    }
                    else
                    {
                        break;
                    }
                }

                // need commenting
                for (int i = 0; i < maxShip; i++)
                {
                    if (isValidComp(playerBtns, x, y - i))
                    {
                        tempVertical++;
                    }
                    else
                    {
                        break;
                    }
                }
                innerLoop++;
            } while ((tempHorizontal < maxShip && tempVertical < maxShip && !isValidComp(playerBtns, x, y)) || innerLoop >= 20);
            
            // need commenting
            if(tempHorizontal < maxShip && tempVertical < maxShip)
            {
                fireRandomPtr(playerBtns);
                horizontalCheck = false;
                verticalCheck = false;
            }

            // need commenting
            if(tempHorizontal >= maxShip)
            {
                horizontalCheck = true;
            }

            // need commenting
            if(tempVertical >= maxShip)
            {
                verticalCheck = true;
            }

            // need commenting
            i = 0;
            j = 0;
        }

        /*
         * checks if the button the computer is trying to shoot is out of board
         * and also if that button is already shot
         * @param player's buttons
         * @param index of x
         * @param index of y
         * return true if the button is valid, return false otherwise
         */
        private bool isValidComp(Button[,] playerBtns, int x, int y)
        {
            // check if the button is out of the board
            if(outOfGridComp(x, y))
            {
                return false;
            }
            else
            {
                // check if the button has been already shot
                if(playerBtns[x, y].BackColor.Equals(shot) || playerBtns[x, y].BackColor.Equals(miss))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        // returns true if it's outside of the grid
        // used during the game not setup
        private bool outOfGridComp(int x, int y)
        {
            if(x < columns && x >= 0 && y < rows && y >= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /*
         * checks if there is a ship where it's been clicked
         * @param button that's been clicked
         * return true if there is a ship, false otherwise
         */
        private bool isShip(Button btn)
        {
            Button[,] btns;
            String[,] str;
            if(turn == 1)
            {
                btns = player2;
                str = player2Text;
            }
            else
            {
                btns = player1;
                str = player1Text;
            }

            // find index of the button clicked from 2d array
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (btn.Equals(btns[i, j]))
                    {
                        // checks if the string at the same index is ship
                        // meaning there is a ship there
                        if (str[i, j].Equals(SHIP))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return false;
        }

        /*
         * check if the button clicked by the user is valid during the setup
         * @param index of x where the buttons was clicked
         * @param index of y
         * @param 2d array of string (whether there is a ship or not)
         * return true if the button clicked is valid, false otherwise
         */
        private bool isBtnValid(int x, int y, String[,] str)
        {
            // checks if the ship will go outside of the board
            if (!outOfGrid(x, y))
            {
                // check if the ship overlaps with other ships
                if (!overlap(x, y, str))
                {
                    return true;
                }
                else
                {
                    errorMessage = "(Overlap with other ships)";
                    return false;
                }
            }
            else
            {
                errorMessage = "(Out of the board)";
                return false;
            }
        }

        /*
         * checks if the ship will overlap
         * @param index of x where the user clicked
         * @param index of y
         * @param 2d array of string (whether there is a ship or not)
         * return true if it will overlap with other ships, false otherwise
         */
        private bool overlap(int x, int y, String[,] str)
        {
            // if the user is trying to place the ship horizontally
            if (horizontal)
            {
                // traverse horizontaly from whre the user clicked by size of ship
                // and if any places are overlapped, return true, otherwise return false
                for (int i = 0; i < size; i++)
                {
                    if (str[x + i, y].Equals(SHIP))
                    {
                        return true;
                    }
                }
                return false;
            }
            // if the user is trying to place the ship vertically
            else
            {
                // traverse vertically from whre the user clicked by size of ship
                // and if any places are overlapped, return true, otherwise return false
                for (int i = 0; i < size; i++)
                {
                    
                    if (str[x, y + i].Equals(SHIP))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /*
         * checks if the ship will go outside of the board
         * @param index of x where the user clicked
         * @param index of y
         * return true if it will go out of the board, false otherwise
         */
        private bool outOfGrid(int x, int y)
        {
            // instantiate n to take x or y of where the user clicked
            // and check to take columns or rows of board
            int n;
            int check;
            // if the ship is horizontally set, n takes x and check takes columns
            if (horizontal)
            {
                n = x;
                check = columns;
            }
            // if the ship is vertically set, n takes y and check takes rows
            else
            {
                n = y;
                check = rows;
            }
            //  if remainders of where the user clicked over check and where the end of ship
            //  is set over check are same, retrun false, otherwise return true 
            if ((n / check) == ((n + size - 1) / check))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /*
         * sets the ship and turn the buttons where there is a ship to red
         * This function will be called after checking if the button clicked is valid
         * @param horizontal - true if the user wants to place it horizontally, false for vertically
         * @param index of x where the button was clicked
         * @param index of y
         * @param 2d array of buttons of the player who is setting the ship
         * @param 2d array of string whether there is a ship or not
         * @param isComputer - if the computer is setting the ship, the button should not be turned red
         */
        private void setShip(bool horizontal, int x, int y, Button[,] btns, String[,] str, bool isComputer)
        {
            // here if the user is trying to place the ship horizontally 
            if (horizontal)
            {
                // need commenting
                for (int i = 0; i < size; i++)
                {
                    str[x + i, y] = SHIP;
                    if (!isComputer)
                    {
                        btns[x + i, y].BackColor = Color.Red;
                    }
                }
            }
            // here if the user is trying to place the ship vertically
            else
            {
                for (int i = 0; i < size; i++)
                {
                    str[x, y + i] = SHIP;
                    if (!isComputer)
                    {
                        btns[x, y + i].BackColor = Color.Red;
                    }
                }
            }
        }

        /*
         * this function will be called when the computer placed ships
         * @param buttons for the computer
         * @param 2d array of string whether there is a ship or not
         */
        public void setShipComp()
        {
            Button[,] btns = player2;
            String[,] str = player2Text;

            // need commenting
            int x = rnd.Next(0, columns - 1);
            int y = rnd.Next(0, rows - 1);

            // need commenting
            for(int i = minShipSize; i <= maxShipSize; i++)
            {
                size = i;
                // need commenting
                if (rnd.Next() % 2 == 0)
                {
                    horizontal = true;
                }
                else
                {
                    horizontal = false;
                }
                // need commenting
                while (!isBtnValid(x, y, str))
                {
                    x = rnd.Next(0, columns - 1);
                    y = rnd.Next(0, rows - 1);
                }
                setShip(horizontal, x, y, btns, player2Text, true);
            }
        }

        // created buttons for ships (text indicates the size of the ship)
        public void makeShips(Form form)
        {
            size = 0;
            for (int i = minShipSize; i <= maxShipSize; i++)
            {
                Button newBtn = new Button();
                newBtn.Name = "ships" + i.ToString();
                newBtn.Size = new Size(WIDTH, HEIGHT);
                newBtn.Location = new Point(WIDTH * (columns + i - minShipSize + 1) + 25, 200);
                newBtn.Click += new EventHandler(shipsClicked);
                newBtn.Text = i.ToString();
                form.Controls.Add(newBtn);
                ships.Add(newBtn);
            }
        }

        // when the button for a ship is clicked, it sets "size"
        private void shipsClicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            // find the button from array
            // by checking the name of the button
            for (int i = minShipSize; i <= maxShipSize; i++)
            {
                if (btn.Name.Equals("ships" + i.ToString()))
                {
                    size = i;
                    break;
                }
            }

            // Since all the buttons on the board are disabled until the user select a ship to place
            // once the user chooses ship, all the button on the board will be enabled
            if (vsComputer)
            {
                btnEnableOrDisable(player1, true);
            }
            else
            {
                if(turn == 1)
                {
                    btnEnableOrDisable(player1, true);
                }
                else
                {
                    btnEnableOrDisable(player2, true);
                }
            }
        }

        // enabled the ship buttons
        public void enableShips()
        {
            foreach (Button btn in ships)
            {
                btn.Enabled = true;
            }
        }

        // makes a button to choose rotation of the ship to place (horizontal or vertical)
        public void makeRotationBtn(Form form)
        {
            Button newBtn = new Button();
            newBtn.Name = "rotationBtn";
            newBtn.Size = new Size(100, 35);
            newBtn.Location = new Point(WIDTH * (columns + 1) + 25, 150);
            newBtn.Click += new EventHandler(rotationBtnClicked);
            newBtn.Text = "Horizontal";
            rotationBtn = newBtn;
            form.Controls.Add(newBtn);
            horizontal = true;
        }

        // when rotationBtn is clicked
        // decide if the btn is horizontal or not
        private void rotationBtnClicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (horizontal)
            {
                btn.Text = "Vertical";
                horizontal = false;
            }
            else
            {
                btn.Text = "Horizontal";
                horizontal = true;
            }
        }

        public void makeNextBtn(Form form)
        {
            Button newBtn = new Button();
            newBtn.Name = "nextBtn";
            newBtn.Size = new Size(100, 35);
            newBtn.Location = new Point((int)(WIDTH * columns / 2), HEIGHT * (rows + 1) + 10);
            newBtn.Click += delegate (object sender, EventArgs e) { nextBtnClicked(sender, e, form); };
            newBtn.Text = "Ready";
            form.Controls.Add(newBtn);
            nextBtn = newBtn;
            newBtn.Enabled = false;
        }

        private void nextBtnClicked(object sender, EventArgs e, Form form)
        {
            Button btn = (Button)sender;
            btnShowOrHide(player1, hideBtn);
            // creates player 2's buttons/board
            makeBtn(form, false, 2);
            enableShips();
            btn.Hide();
            makeReadyBtn(form);
            hideResetBtn();
            makeResetBtn(form, 2);
            turn = 2;
            player.labelHide(1);
            player.labelShow(2);
        }

        // ready button allows the user to go from setting the ships to actual game
        public void makeReadyBtn(Form form)
        {
            Button newBtn = new Button();
            newBtn.Name = "readyBtn";
            newBtn.Size = new Size(100, 35);
            newBtn.Location = new Point((int)(WIDTH * columns / 2), HEIGHT * (rows + 1) + 10);
            newBtn.Click += delegate(object sender, EventArgs e) { readyBtnClicked(sender, e, form); };
            newBtn.Text = "Ready";
            readyBtn = newBtn;
            form.Controls.Add(newBtn);
            readyBtn.Enabled = false;
        }

        /*
         * resets all the color (indicating there is a ship)
         * hide all the buttons of ships
         * player1/player's buttons will be disabled because they should be shooting at opponent
         * player2/computer's buttons where they get shot will be enabled
         * @param sender - button clicked
         * @param e
         * @param windows form that's been used
         */
        private void readyBtnClicked(object sender, EventArgs e, Form form)
        {
            Button btn = (Button)sender;
            if (vsComputer)
            {
                btnEnableOrDisable(player1, false);
                btnEnableOrDisable(player2, true);
            }
            else
            {
                btnShowOrHide(player1, hideBtn);
                btnShowOrHide(player2, showBtn);
                btnEnableOrDisable(player2, true);
                swapBtn(form);
                player.labelShow(1);
                player.labelHide(2);
            }
            hideColor(player1);
            hideColor(player2);
            shipsHide(ships);
            btn.Hide();
            rotationBtn.Hide();
            resetBtn.Hide();
            turn = 1;
            setup = false;
        }

        // shows the ready button
        public void readyBtnShow()
        {
            readyBtn.Show();
            // set n by the number of ships plced in a board
            n = maxShipSize - minShipSize + 1;
        }

        // makes the button for swaping the turns during the game
        private void swapBtn(Form form)
        {
            Button newBtn = new Button();
            newBtn.Name = "swapBtn";
            newBtn.Size = new Size(100, 35);
            newBtn.Location = new Point((int)(WIDTH * columns / 2), HEIGHT * (rows + 1) + 10);
            newBtn.Click += new EventHandler(swapBtnClicked);
            newBtn.Text = "Next";
            form.Controls.Add(newBtn);
            newBtn.Enabled = false;
            swpBtn = newBtn;
        }

        // it will be other player's turn
        // all the buttons and label showing the turn will be updated
        private void swapBtnClicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            player.labelHide(turn);
            if (turn == 1)
            {
                btnShowOrHide(player2, hideBtn);
                btnShowOrHide(player1, showBtn);
                btnEnableOrDisable(player1, true);
            }
            else
            {
                btnShowOrHide(player1, hideBtn);
                btnShowOrHide(player2, showBtn);
                btnEnableOrDisable(player2, true);
            }
            turn = (turn % 2) + 1;
            player.labelShow(turn);
            btn.Enabled = false;
        }

        // reset button allows the user to redo the placing of ships
        // it reset all the ships that the user has placed on the board
        public void makeResetBtn(Form form, int turn)
        {
            Button[,] btns;
            String[,] str;
            if(turn == 1)
            {
                btns = player1;
                str = player1Text;
            }
            else
            {
                btns = player2;
                str = player2Text;
            }

            Button newBtn = new Button();
            newBtn.Name = "resetBtn";
            newBtn.Size = new Size(100, 35);
            newBtn.Location = new Point(WIDTH * (columns + 1) + 25, 100);
            newBtn.Click += delegate (object sender, EventArgs e) { resetBtnClicked(sender, e, btns, str, ships); };
            newBtn.Text = "Reset";
            form.Controls.Add(newBtn);
            resetBtn = newBtn;
        }

        // it reset all the buttons and string
        private void resetBtnClicked(object sender, EventArgs e, Button[,] btns, String[,] str, List<Button> ships)
        {
            for(int i = 0; i < columns; i++)
            {
                for(int j = 0; j < rows; j++)
                {
                    str[i, j] = "";
                    btns[i, j].BackColor = default(Color);
                }
            }

            // reenables the buttons for the ship
            enableShips();
            // disables all the buttons on the board until the user selects a ship
            btnEnableOrDisable(btns, false);

            // if player vs player and it is player1's turn
            // set the number of ships which the user has placed to 0
            if (!vsComputer && turn == 1)
            {
                n = 0;
            }
            else
            {
                n = maxShipSize - minShipSize + 1;
            }

            /*
             * if player vs player and it is player1's turn
             * next button will be disabled until player1 places all the ships
             * Otherwise, ready button will be disabled until the player places all the ships
             */
            if (!vsComputer && turn == 1)
            {
                nextBtn.Enabled = false;
            }
            else
            {
                readyBtn.Enabled = false;
            }
        }

        /*
         * enables or disables buttons of a certain player on the board
         * @param button of a player
         * @param true to enable, false to disable
         */
        public void btnEnableOrDisable(Button[,] btns, bool enable)
        {
            foreach (Button btn in btns)
            {
                btn.Enabled = enable;
            }
        }

        /*
         * shows or hides all the button in the 2d array
         * @param buttons to show/hide
         * @param function pointer to show or hide
         */
        public void btnShowOrHide(Button[,] btns, funcPtrBtn ptr)
        {
            foreach(Button btn in btns)
            {
                ptr(btn);
            }
        }

        // shows the button
        // @param button
        private void showBtn(Button btn)
        {
            btn.Show();
        }

        // hides the button
        // @param button
        private void hideBtn(Button btn)
        {
            btn.Hide();
        }
        
        /*
         * resets the fore color of buttons so when the game starts
         * where the ships are won't be shown
         * @param buttons of a player
         */
        public void hideColor(Button[,] btns)
        {
            foreach (Button btn in btns)
            {
                btn.BackColor = default(Color);
            }
        }

        // hides reset button
        // this function was created so that it can be accessed from other class
        public void hideResetBtn()
        {
            resetBtn.Hide();
        }

        // hides all the button of ships
        private void shipsHide(List<Button> ships)
        {
            foreach(Button btn in ships)
            {
                btn.Hide();
            }
        }

        /*
         * returns total number of ships
         * once the number of successful shot equal the total number of ships
         * that means the player has sank all the ships
         */
        private int totalShips()
        {
            return (maxShipSize + minShipSize) * (maxShipSize - minShipSize + 1)/2;
        }

        /*
         * setter for board size
         * @param number of columns and rows
         */
        public void setBoardSize(int i)
        {
            columns = i;
            rows = i;
            player1 = new Button[i, i];
            player1Text = new String[i, i];
            player2 = new Button[i, i];
            player2Text = new String[i, i];
        }

        /*
         * setter for minimum and maximum size of ships
         * @param ship size option that the user chose in options class
         */
        public void setMinAndMaxShip(int option)
        {
            if(option/2 == 0)
            {
                minShipSize = option + 2;
                maxShipSize = option + 5;
            }
            else
            {
                minShipSize = option + 1;
                maxShipSize = option + 3;
            }
            maxShip = maxShipSize;
        }

        // setter for computer level
        // @param computer level (0-easy, 1-medium, 2-hard)
        public void setComputerLevel(int computerLevel)
        {
            this.computerLevel = computerLevel;
            if(computerLevel == 0)
            {
                firePtr = fireCompEasy;
                fireRandomPtr = randomFire;
            }
            else
            {
                firePtr = fireCompMedium;
                if(computerLevel == 1)
                {
                    fireRandomPtr = randomFire;
                }
                else
                {
                    fireRandomPtr = randomFireHard;
                }
            }
        }

        // sets the turn to player 2
        // this funtion was created so that it can be accessed from other class
        public void setTurn2()
        {
            turn = 2;
        }

        // getter for number of columns of the board
        // return columns
        public int getColumns()
        {
            return columns;
        }

        // getter for number of rows of the board
        // return rows
        public int getRows()
        {
            return rows;
        }

        // getter for width of each button on the board
        // return WIDTH
        public int getWidth()
        {
            return WIDTH;
        }

        // getter for height of each button on the board
        // return HEIGHT
        public int getHeight()
        {
            return HEIGHT;
        }
    }
}