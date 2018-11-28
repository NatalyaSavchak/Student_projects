using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game
{

    public partial class Form1 : Form
    {
        Timer timer = new Timer();
        Random rnd = new Random();
        int[] numbers = new int[9];
        int round = 0;
        TableLayoutControlCollection numbers_button;
        public Form1()
        {
            InitializeComponent();
            start.Click += Start_Click;

            timer.Interval = 15000;
            timer.Tick += Timer_Tick;
            
            numbers_button = numberPanel.Controls;
            foreach(Control item in numbers_button)
            {
                item.Click += Item_Click;
            }
        }

        private void Item_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            int number = int.Parse(b.Text);
            Check_number( number);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            MessageBox.Show("Time is over! You lose the game!");
            Clear_buttoms();
        }

        private void Start_Click(object sender, EventArgs e)
        {
            round = 0;
            for (int i = 0; i< numbers.Length; i++)
            {
                numbers[i] = rnd.Next(1, 100);
            }

            int count = 0;
            foreach (Control item in numbers_button)
            {
                item.Text = numbers[count].ToString();
                count++;
            }
            Array.Sort(numbers);
            timer.Start();
        }

        private void Check_number(int number)
        {
            if (number == numbers[round])
            {
                ++round;
            }
            else
            {

                Lose_Game();
            }
            if (round == 9)
            {

                Win_Game();
            }
        }
        private void Lose_Game()
        {
            timer.Stop();
            MessageBox.Show("You lose the game!");
            Clear_buttoms();
        }
        private void Win_Game()
        {
            timer.Stop();
            MessageBox.Show("Congratulations!You win the game!");
            Clear_buttoms();
        }

        private void Clear_buttoms()
        {
            foreach (Control item in numbers_button)
            {
                item.Text = null;
            }
        }
    }
}
