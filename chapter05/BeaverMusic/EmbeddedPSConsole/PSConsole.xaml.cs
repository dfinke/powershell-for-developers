using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace EmbeddedPSConsole
{
    public partial class PSConsole : Window
    {
        public PSConsole()
        {
            InitializeComponent();

            Console.Focus();

            PSConfig.AddVariable("Window", this);
            PSConfig.AddVariable("CommandPane", this.Results);
            PSConfig.AddVariable("ScriptPane", this.Console);
            PSConfig.AddVariable("Name", "My PowerShell Instance");

            DisplayPSResults(PSConfig.RunCustomProfile("BeaverMusicProfile.ps1"));
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F5:
                    ExecutePSScript();
                    break;
                case Key.F12:
                    ClearResults();
                    break;
            }
        }

        private void ClearResults()
        {
            Results.Text = "";
        }

        private void ExecutePSScript()
        {
            DisplayPSResults(Console.Text.ExecutePS());
        }

        private void DisplayPSResults(string psResults)
        {
            Results.Text += psResults;
            Results.ScrollToEnd();
        }

        private void ConsoleMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (e.Delta > 0)
                {
                    Results.FontSize = Console.FontSize += 1;
                }
                else
                {
                    Results.FontSize = Console.FontSize -= 1;
                    if (Console.FontSize < 9)
                    {
                        Results.FontSize = Console.FontSize = 9;
                    }
                }
            }
        }

        private void Run(object sender, RoutedEventArgs e)
        {
            ExecutePSScript();
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            ClearResults();
        }

        private void Artist_PreviewKeyDown(
            object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var script = "Get-ChinookData "
                    + Artist.Text + " | Import-BeaverMusic";

                script.ExecutePS();
                e.Handled = true;
            }
        }
    }
}