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
using System.Windows.Shapes;

namespace AHT_Buddy
{
    /// <summary>
    /// Interaction logic for windowTimer.xaml
    /// </summary>
    public partial class windowTimer : Window
    {
        public int Hour;
        public int Minute;
        public int Second;
        public bool hh, mm, ss;
        

        public windowTimer()
        {
            InitializeComponent();
            for (int i = 1; i < 25; i++)
            {
                comboBoxHour.Items.Add(i.ToString());
            }
            for (int i = 0; i < 60; i++)
            {
                comboBoxMinute.Items.Add(i.ToString());
                comboBoxSecond.Items.Add(i.ToString());
            }
        }

        private void rHours_MouseWheel(object sender, MouseWheelEventArgs e)
        {
         
        }

        private void rMinutes_MouseWheel(object sender, MouseWheelEventArgs e)
        {
           
        }

        private void rHours_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            popHour.IsOpen = true;
            popHour.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            hh = true;mm = false; ss = false;
            tbDigits.Text = countdownTimer.Text.Substring(0, 1);
        }

        private void popHour_LostFocus(object sender, RoutedEventArgs e)
        {
            popHour.IsOpen = false;
        }

        private void popHour_MouseLeave(object sender, MouseEventArgs e)
        {
            popHour.IsOpen = false;
        }

        private void tbDigits_LostFocus(object sender, RoutedEventArgs e)
        {
            popHour.IsOpen = false;
        }

        private void tbDigits_GotFocus(object sender, RoutedEventArgs e)
        {
            tbDigits.Clear();
        }

        private void comboBoxHour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            countdownTimer.Text = string.Format("{0}:{1}:{2}", comboBoxHour.Text, Minute, Second);
        }

        private void tbDigits_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                popHour.IsOpen = false;


                if (hh == true & mm == false & ss == false)
                {
                    bool getHour = int.TryParse(tbDigits.Text, out Hour);
                    if (Minute > 10 & Second > 10)
                    {
                        countdownTimer.Text = string.Format("{0}:{1}:{2}", Hour, Minute, Second);
                    }
                    else if (Minute < 10 & Second < 10)
                    {
                        countdownTimer.Text = string.Format("{0}:0{1}:0{2}", Hour, Minute, Second);
                    }
                    else if (Minute > 10 & Second < 10)
                    {
                        countdownTimer.Text = string.Format("{0}:{1}:0{2}", Hour, Minute, Second);
                    }
                }
                if(hh == false & mm == true & ss == false)
                {
                    bool getMinute = int.TryParse(tbDigits.Text, out Minute);
                    if (Minute > 9 & Second > 10)
                    {
                        countdownTimer.Text = string.Format("{0}:{1}:{2}", Hour, Minute, Second);
                    }
                    else if (Minute < 10 & Second < 10)
                    {
                        countdownTimer.Text = string.Format("{0}:0{1}:0{2}", Hour, Minute, Second);
                    }
                    else if (Minute > 9 & Second < 10)
                    {
                        countdownTimer.Text = string.Format("{0}:{1}:0{2}", Hour, Minute, Second);
                    }
                }
                if (hh == false & mm == false & ss == true)
                {
                    bool getSecond = int.TryParse(tbDigits.Text, out Second);
                    if (Minute > 10 & Second > 10)
                    {
                        countdownTimer.Text = string.Format("{0}:{1}:{2}", Hour, Minute, Second);
                    }
                    else if (Minute < 10 & Second < 10)
                    {
                        countdownTimer.Text = string.Format("{0}:0{1}:0{2}", Hour, Minute, Second);
                    }
                    else if (Minute > 10 & Second < 10)
                    {
                        countdownTimer.Text = string.Format("{0}:{1}:0{2}", Hour, Minute, Second);
                    }
                }
                
                
               
            }
        }

        private void rMinutes_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            popHour.IsOpen = true;
            popHour.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            hh = false; mm = true; ss = false;
            tbDigits.Text = Hour.ToString();
        }
    }
}
