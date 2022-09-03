using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.RegularExpressions;

namespace Host
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void TextChangedEventHandler(object sender, TextChangedEventArgs args)
        {
            ((TextBox)sender).Background = Brushes.Transparent;
        } 

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            string text = "";
            foreach (TextBox tb in CheckTextBox.Children.OfType<StackPanel>().Select(x => ((TextBox)x.Children[1])))
            {
                if (tb.Text.Length == 0)
                {
                    text += "Error" + "\n";
                    tb.Background = Brushes.DarkRed;
                    continue;
                }

                switch (tb.Name)
                {
                    case "Phone":
                        if (new Regex(@"\+\d{1,3}\d{9,13}").IsMatch(tb.Text))
                            goto default;
                        tb.Background = Brushes.DarkRed;

                        break;
                    case "BotName":
                        if (new Regex(@"\w{1,50}").IsMatch(tb.Text))
                            goto default;
                        tb.Background = Brushes.DarkRed;


                        break;
                    default:
                        text += tb.Text + "\n";
                        break;
                }
            }

            MessageBox.Show(text);
        }

    }
}
