using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Sift.Client.Elements;
using Sift.Common;
using Sift.Common.Net;

namespace Sift.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Line> Lines { get; }

        public Line SelectedLine { get; private set; }

        public ScreenerElement Screener { get; }
        public IList<HybridElement> Hybrids { get; private set; }
        
        public SdpClient Client { get; }

        public bool HasConnection
        {
            set
            {
                ConnectionStatus.Content = value ? "CONNECTED" : "DISCONNECTED";
                ConnectionStatus.Foreground = new SolidColorBrush(value ? Colors.DarkGreen : Colors.DarkRed);
            }
        }

        public Role Role { get; } 

        private VoipProviders provider;
        private IDictionary<Line, LineElement> elements;

        private bool hybridsCreated = false;

        public MainWindow(SdpClient client, VoipProviders provider, int lines, Role role)
        {
            this.provider = provider;
            Role = role;
            Client = client;

            InitializeComponent();

            Lines = new List<Line>();
            for (byte i = 0; i < lines; i++)
                Lines.Add(new Line(i));

            if (Role == Role.Screener)
            {
                Screener = new ScreenerElement(Client);
                Grid.SetRow(Screener, 0);
                Grid.SetColumn(Screener, 0);
                ScreenerFrame.Content = Screener;
            }

            if (Lines.Count > 0)
                SelectLine(Lines[0]);

            ConstructLineGrid(LineGrid, Lines, out elements);
            
            Client.Manager.InitializeLine += Client_InitializeLine;
            Client.Manager.LineMetadata += Client_LineMetadata;
            Client.Manager.LineStateChanged += Client_LineStateChanged;
            Client.Manager.Settings += Client_Settings;
            /*Client.Send(new Settings()
            {
                Key = "asterisk_hybrid_extensions",
                Category = "asterisk",
            });*/

            HasConnection = true;
        }

        private void Client_LineStateChanged(string id, LineStateChanged e)
        {
            if (e.Index >= Lines.Count)
                return;

            Line line = Lines[e.Index];
            line.State = e.State;

            if (e.State == LineState.Empty)
            {
                line.Caller = null;
                if (Role == Role.Screener)
                    Screener.Line = null;
            }

            elements[line].Update();
        }

        private void Client_Settings(string id, Settings e)
        {
            if (hybridsCreated || e.Key != "asterisk_hybrid_extensions" || e.Items.Count < 1)
                return;

            hybridsCreated = true;

            NetworkSetting setting = e.Items.First();
            int[] hybrids = (int[])setting.Value;

            Grid grid = CreateHybridGrid(hybrids);
            HybridsFrame.Content = grid;
        }

        private Grid CreateHybridGrid(int[] hybrids)
        {
            Grid grid = new Grid();

            Array.Sort(hybrids);

            for (int i = 0; i < hybrids.Length; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(40),
                });
                HybridElement el = new HybridElement(hybrids[i].ToString());
                Grid.SetRow(el, i);
                grid.Children.Add(el);
            }

            grid.RowDefinitions.Add(new RowDefinition());

            return grid;
        }

        private void Client_InitializeLine(string id, InitializeLine e)
        {
            if (e.Index >= Lines.Count)
                return;

            Line line = Lines[e.Index];
            line.State = e.State;
            if (line.Caller == null)
                line.Caller = new Caller(e.Id);

            line.Caller.Number = e.Number;
            
            elements[line].Update();

            if (line == SelectedLine)
                SelectLine(line);
        }

        private void Client_LineMetadata(string id, LineMetadata e)
        {
            if (e.Index >= Lines.Count)
                return;

            Line line = Lines[e.Index];
            if (line.Caller == null)
                return;

            line.Caller.Name = e.Name;
            line.Caller.Location = e.Location;
            line.Caller.Comment = e.Comment;

            elements[line].Update();
        }

        private void LineElementClick(object sender, MouseButtonEventArgs e)
        {
            LineElement line = (LineElement)sender;
            foreach (LineElement el in elements.Values)
            {
                if (el != line)
                    el.ShowBorder = false;
            }
            SelectLine(line.Line);
            if (line != null)
            {
                line.ShowBorder = true;
            }
        }

        private void SelectLine(Line line)
        {
            SelectedLine = line;
            if (Role == Role.Screener)
                Screener.Line = line;
        }

        private void ConstructLineGrid(Grid g, IList<Line> lines, out IDictionary<Line, LineElement> elements)
        {
            const int maxHeight = 250;
            int cols = 2;
            int rows = lines.Count / 2;
            elements = new Dictionary<Line, LineElement>();

            for (int col = 0; col < cols; col++)
            {
                g.ColumnDefinitions.Add(new ColumnDefinition());
                for (int row = 0; row < rows; row++)
                {
                    if (col == 0)
                    {
                        g.RowDefinitions.Add(new RowDefinition
                        {
                            MaxHeight = maxHeight
                        });
                    }

                    if (col * rows + row >= lines.Count)
                        continue;

                    Line line = lines[col * rows + row];

                    LineElement el = new LineElement(this, line)
                    {
                        Margin = new Thickness(5, 5, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        MaxHeight = maxHeight
                    };
                    el.MouseLeftButtonUp += LineElementClick;

                    Grid.SetRow(el, row);
                    Grid.SetColumn(el, col);

                    g.Children.Add(el);
                    elements[line] = el;
                }
            }
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            Client.Disconnect();
            Application.Current.Shutdown();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            new VoipProviderSettingsWindow(Client, provider).Show();
        }

        private void NetStatistics_Click(object sender, RoutedEventArgs e) { } /*new NetworkStatisticsWindow(Client.Client).Show()*/

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
