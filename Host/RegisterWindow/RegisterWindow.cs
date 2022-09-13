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
using System.ComponentModel;
using System.IO;

namespace Host
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }
        private Process client;
        private ResponseSocket server;

        private void RegisterWindow_Closing(object sender, CancelEventArgs e)
        {
            if (client != null)
                client.Close();
            if (server != null)
                server.Close();

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
                    case "NameValue":
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

                    case "PhoneValue":
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

                    case "HashValue":
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

                    case "IDValue":
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

            if (InputManager.isAllCorrect())
            {
                string script = @$"{new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName}\Client\main.py", fileName = "";

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

                    using (server = new ResponseSocket())
                    {
                        server.Bind("tcp://*:5555");

                        client = Process.Start(processInfo) ?? throw new System.Exception("Bad client process");
                        client.StandardInput.AutoFlush = true;


                        string msg;
                        while (true)
                        {
                            msg = server.ReceiveFrameString();

                            switch (msg)
                            {
                                case "Name":
                                    Dispatcher.Invoke(() => server.SendFrame(NameValue.Text));
                                    break;
                                case "ID":
                                    Dispatcher.Invoke(() => server.SendFrame(IDValue.Text));
                                    break;
                                case "Hash":
                                    Dispatcher.Invoke(() => server.SendFrame(HashValue.Text));
                                    break;
                                case "Phone":
                                    Dispatcher.Invoke(() =>
                                    {
                                        client.StandardInput.WriteLine(PhoneValue.Text);
                                        server.SendFrame("");
                                    });
                                    break;
                                case "Confirm":
                                    Dispatcher.Invoke(() =>
                                    {
                                        client.StandardInput.WriteLine("y");
                                        server.SendFrame("");
                                    });
                                    break;
                                case "Code":
                                    Dispatcher.Invoke(() =>
                                    {
                                        InputWindow input = new InputWindow();
                                        if (input.ShowDialog() == true)
                                            client.StandardInput.WriteLine(input.CodeValue.Text);
                                        server.SendFrame("");
                                    });
                                    break;
                            }

                        }
                    }

                });
            }
        }
    }
}
