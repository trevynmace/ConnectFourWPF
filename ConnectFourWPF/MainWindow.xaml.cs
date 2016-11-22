using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ConnectFourWPF
{
    public partial class MainWindow : Window
    {
        private static int[,] board = new int[6, 7];
        bool gameOver = false;
        DispatcherTimer timer = new DispatcherTimer();
        Label label = new Label();
        TextBox box = new TextBox();
        int which = 0;
        string player1;
        string player2;

        string currentPlayer;
        Brush currentPlayerColor;

        public MainWindow()
        {
            InitializeComponent();
            EnableAllButtons(false);
            AddLabelAndTextBox();
        }

        private void EnableAllButtons(bool isEnabled)
        {
            Column1.IsEnabled = isEnabled;
            Column2.IsEnabled = isEnabled;
            Column3.IsEnabled = isEnabled;
            Column4.IsEnabled = isEnabled;
            Column5.IsEnabled = isEnabled;
            Column6.IsEnabled = isEnabled;
            Column7.IsEnabled = isEnabled;
        }

        void box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (which == 0)
                {
                    player1 = box.Text;
                    box.Text = string.Empty;
                    which = 1;

                    MyCanvas.Children.Remove(label);
                    MyCanvas.Children.Remove(box);
                    AddLabelAndTextBox();
                }
                else if (which == 1)
                {
                    player2 = box.Text;

                    MyCanvas.Children.Remove(box);

                    label.Content = string.Format("{0}, please select a column.", player1);
                    currentPlayer = player1;
                    currentPlayerColor = Brushes.Red;

                    EnableAllButtons(true);
                }
            }
        }

        private void AddLabelAndTextBox()
        {
            if (which == 0)
            {
                label.Content = "Please enter the name of the first player and press enter: ";
            }
            else if (which == 1)
            {
                label.Content = "Please enter the name of the second player and press enter: ";
            }
            Canvas.SetLeft(label, 20);
            Canvas.SetTop(label, 275);

            box.Width = 200;
            box.KeyDown += new KeyEventHandler(box_KeyDown);
            Canvas.SetLeft(box, 30);
            Canvas.SetTop(box, 300);

            MyCanvas.Children.Add(label);
            MyCanvas.Children.Add(box);
        }

        private void DrawCircle(int leftPosition, int topPosition, Brush playerColor)
        {
            Ellipse circle = new Ellipse();
            circle.Height = 33;
            circle.Width = 33;
            circle.Fill = playerColor;
            Canvas.SetLeft(circle, leftPosition);
            Canvas.SetTop(circle, topPosition);

            MyCanvas.Children.Add(circle);
        }

        private void SwitchPlayers()
        {
            if(currentPlayer.Equals(player1))
            {
                currentPlayer = player2;
                currentPlayerColor = Brushes.ForestGreen;
            }
            else
            {
                currentPlayer = player1;
                currentPlayerColor = Brushes.Red;
            }

            label.Content = string.Format("{0}, please select a column.", currentPlayer);
        }

        private void UpdateBoard(int column)
        {
            for (int i = board.GetLength(0) - 1; i >= 0; i--)
            {
                if (board[i, column] == 0)
                {
                    board[i, column] = (currentPlayer.Equals(player1)) ? 1 : 2;
                    currentRow = i;
                    currentColumn = column;
                    //gameOver = WinnerFound(i, column);
                    break;
                }
            }
        }

        int currentRow;
        int currentColumn;

        private void RejectDrop()
        {
            label.Content = "Please select a different column.";
        }

        private void DroppingMotion()
        {
            GenerateTickTarget();
            timer = new DispatcherTimer();
            timer.Interval = new System.TimeSpan(0, 0, 0, 0, 1);
            timer.Tick += timer_Tick;
            EnableAllButtons(false);
            timer.Start();
        }

        private void GenerateTickTarget()
        {
            switch(currentRow)
            {
                case 0:
                    tickTarget = 7;
                    break;
                case 1:
                    tickTarget = 20;
                    break;
                case 2:
                    tickTarget = 33;
                    break;
                case 3:
                    tickTarget = 45;
                    break;
                case 4:
                    tickTarget = 58;
                    break;
                case 5:
                    tickTarget = 71;
                    break;
            }
        }

        int top = -15;
        int tickCount = 0;
        int tickTarget;

        void timer_Tick(object sender, System.EventArgs e)
        {
            top += 3;
            Canvas.SetTop(MyCanvas.Children[MyCanvas.Children.Count - 1], top);
            tickCount++;

            if (tickCount == tickTarget)
            {
                top = -15;
                tickCount = 0;

                
                gameOver = WinnerFound(currentRow, currentColumn);
                if (gameOver)
                {
                    label.Content = string.Format("Congratulations {0}, you won!!", currentPlayer);
                    Button exitButton = new Button();
                    exitButton.Name = "Exit";
                    exitButton.Content = "Exit";
                    exitButton.Click += Exit_Click;
                    MyCanvas.Children.Add(exitButton);
                    Canvas.SetLeft(exitButton, 30);
                    Canvas.SetTop(exitButton, 300);
                }
                else
                {
                    SwitchPlayers();
                    EnableAllButtons(true);
                }
                //TODO: check for winner, if yes, change label to specify who,
                // disable all column buttons, add close button after label
                timer.Stop();
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private bool WinnerFound(int row, int col)
        {
            int playerNumber = board[row, col];
            int count = 1;
            int changeableRow = row;
            int changeableCol = col;

            //check up and down
            while (--changeableRow >= 0)
            {
                if (board[changeableRow, col] == playerNumber)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            changeableRow = row;
            while (++changeableRow < board.GetLength(0))
            {
                if (board[changeableRow, col] == playerNumber)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            if (count >= 4)
            {
                return true;
            }

            //reset values changed in above code
            count = 1;
            changeableRow = row;

            //check left and right
            while (--changeableCol >= 0)
            {
                if (board[row, changeableCol] == playerNumber)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            changeableCol = col;
            while (++changeableCol < board.GetLength(1))
            {
                if (board[row, changeableCol] == playerNumber)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            if (count >= 4)
            {
                return true;
            }

            //reset values changed in above code
            count = 1;
            changeableCol = col;

            //check bottom-right and top-left
            while (--changeableRow >= 0 && --changeableCol >= 0)
            {
                if (board[changeableRow, changeableCol] == playerNumber)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            changeableCol = col;
            changeableRow = row;
            while (++changeableRow < board.GetLength(0) && ++changeableCol < board.GetLength(1))
            {
                if (board[changeableRow, changeableCol] == playerNumber)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            if (count >= 4)
            {
                return true;
            }

            //reset values changed by above code
            count = 1;
            changeableRow = row;
            changeableCol = col;

            //check bottom-left and top-right
            while (++changeableRow < board.GetLength(0) && --changeableCol >= 0)
            {
                if (board[changeableRow, changeableCol] == playerNumber)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            changeableCol = col;
            changeableRow = row;
            while (--changeableRow >= 0 && ++changeableCol < board.GetLength(1))
            {
                if (board[changeableRow, changeableCol] == playerNumber)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            if (count >= 4)
            {
                return true;
            }

            return false;
        }

        private void ColumnClickAction(int column, int leftPosition)
        {
            if (board[0, column - 1] == 0)
            {
                UpdateBoard(column - 1);
                DrawCircle(leftPosition, -20, currentPlayerColor);
                DroppingMotion();
            }
            else
            {
                RejectDrop();
            }
        }

        #region
        private void Column1_Click(object sender, RoutedEventArgs e)
        {
            ColumnClickAction(1, 120);
        }

        private void Column2_Click(object sender, RoutedEventArgs e)
        {
            ColumnClickAction(2, 162);
        }

        private void Column3_Click(object sender, RoutedEventArgs e)
        {
            ColumnClickAction(3, 204);
        }

        private void Column4_Click(object sender, RoutedEventArgs e)
        {
            ColumnClickAction(4, 246);
        }

        private void Column5_Click(object sender, RoutedEventArgs e)
        {
            ColumnClickAction(5, 288);
        }

        private void Column6_Click(object sender, RoutedEventArgs e)
        {
            ColumnClickAction(6, 330);
        }

        private void Column7_Click(object sender, RoutedEventArgs e)
        {
            ColumnClickAction(7, 372);
        }
        #endregion
    }
}
