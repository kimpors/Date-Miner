using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
using NetMQ;
using NetMQ.Sockets;
using System.Threading;
using System.IO;

namespace Host
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        public static class InputManager
        {
            public static bool isID { get; private set; }
            public static bool isHASH { get; private set; }
            public static bool isName { get; private set; }
            public static bool isPhone { get; private set; }

            public static bool isCorrectPhone(string phone)
            {
                if (phone.Length > 16 || phone.Length < 10 || phone is null)
                    return isPhone = false;

                return isPhone = new Regex(@"\+\d{1,3}\d{9,13}").IsMatch(phone);
            }

            public static bool isCorrectName(string name)
            {
                if (name is null || name.Length == 0)
                    return isName = false;

                return isName = new Regex(@"\w{1,50}").IsMatch(name);
            }

            public static bool isCorrectID(string id)
                => isID = id.Length == 8;

            public static bool isCorrectHASH(string hash)
                => isHASH = hash.Length == 32;

            public static bool isAllCorrect()
                => true ? isID && isHASH && isName && isPhone : false;
        }

        private void TextChangedEventHandler(object sender, TextChangedEventArgs args)
            => ((TextBox)sender).Background = Brushes.Transparent;

        private async void Check_Click(object sender, RoutedEventArgs e)
        {
            ToolTip curr = new ToolTip();
            curr.IsOpen = true;
            foreach (TextBox tb in CheckTextBox.Children.OfType<StackPanel>().Select(x => ((TextBox)x.Children[1])))
            {
                switch (tb.Name)
                {
                    case "BotName":
                        if (!InputManager.isCorrectName(tb.Text))
                        {
                            tb.ToolTip = new ToolTip()
                            {
                                Content = "Please enter right name(max 50 symbols(not numbers))",
                                IsOpen = true
                            };
                            goto default;
                        }
                        break;

                    case "Phone":
                        if (!InputManager.isCorrectPhone(tb.Text))
                        {

                            tb.ToolTip = new ToolTip()
                            {
                                Content = "Please enter right phone number{+{country code}{numbers}}",
                                IsOpen = true,
                            };
                            goto default;
                        }
                        break;

                    case "Hash":
                        if (!InputManager.isCorrectHASH(tb.Text))
                        {
                            tb.ToolTip = new ToolTip()
                            {
                                Content = "Please enter right hash(32 symbold)",
                                IsOpen = true
                            };
                            goto default;
                        }
                        break;

                    case "ID":
                        if (!InputManager.isCorrectID(tb.Text))
                        {
                            tb.ToolTip = new ToolTip()
                            {
                                Content = "Please enter right id(8 numbers)",
                                IsOpen = true
                            };
                            goto default;
                        }
                        break;
                    default:
                        tb.Background = new SolidColorBrush(Color.FromArgb(255, 160, 25, 20));
                        break;
                }
            }

            if (!InputManager.isAllCorrect())
            {
                string script = @$"{new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName}\Client\main.py", fileName = "";
                MessageBox.Show(script);

                await Task.Run(() =>
                {
                    ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe")
                    {
                        CreateNoWindow = false,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        UseShellExecute = false
                    };

                    using (Process getPythonExePath = Process.Start(processInfo))
                    {
                        getPythonExePath.StandardInput.WriteLine(@"py -c ""import sys;print(sys.executable)""");
                        getPythonExePath.StandardInput.Flush();
                        getPythonExePath.StandardInput.Close();
                        getPythonExePath.WaitForExit();
                        fileName = getPythonExePath.StandardOutput.ReadToEnd().Split('\n')[4];
                    }

                    processInfo.FileName = fileName;
                    processInfo.Arguments = @$"""{script}""";

                    using (var server = new ResponseSocket())
                    {
                        server.Bind("tcp://*:5555");

                        using (Process client = Process.Start(processInfo))
                        {

                            string msg;
                            while (true)
                            {
                                msg = server.ReceiveFrameString();
                                MessageBox.Show(msg + " message");

                                switch (msg)
                                {
                                    case "Hello":
                                        server.SendFrame("World");
                                    break;
                                }
                            }
                        }
                    }
                });
            }
        }
    }
}
