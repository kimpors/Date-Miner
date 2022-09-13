using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.IO;
using Microsoft.VisualBasic;
using System;

namespace Host
{
    public partial class InputWindow : Window
    {
        public InputWindow()
        {
            InitializeComponent();
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CodeValue_Input(object sender, TextCompositionEventArgs e)
        {
            e.Handled = false;
            if(!char.IsDigit(e.Text[0]))
                e.Handled= true;
        }
    }
}
