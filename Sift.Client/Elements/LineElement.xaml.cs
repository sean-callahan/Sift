using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Sift.Common;
using Sift.Common.Network;

namespace Sift.Client.Elements
{
    /// <summary>
    /// Interaction logic for LineElement.xaml
    /// </summary>
    public partial class LineElement : UserControl
    {
        public Line Line { get; }

        public bool ShowBorder
        {
            get { return showingBorder; }
            set
            {
                if (value == showingBorder)
                    return;
                if (value)
                    Border.BorderBrush = Brushes.Orange;
                else
                    Border.BorderBrush = Brushes.Transparent;
                showingBorder = value;
            }
        }

        private MainWindow parent;
        private bool showingBorder = false;
        private DateTime created;
        private DispatcherTimer durationUpdater = new DispatcherTimer();

        private Button Air;
        private Button Screen;
        private Button Hangup;

        public LineElement(MainWindow parent, Line line)
        {
            this.parent = parent;
            Line = line;

            InitializeComponent();
            createButtonBar();

            durationUpdater.Tick += DurationUpdater_Tick;
            durationUpdater.Interval = new TimeSpan(0, 0, 1);

            if (Line != null)
            {
                Index.Content = Line.Index+1;
            }

            Update();

            durationUpdater.Start();
        }

        private void createButtonBar()
        {
            for (int i = 0; i < (parent.Role == Role.Screener ? 3 : 2); i++)
                ButtonBar.RowDefinitions.Add(new RowDefinition());

            Air = new Button
            {
                Content = "AIR",
                IsEnabled = false,
            };
            Air.Click += Air_Click;
            ButtonBar.Children.Add(Air);

            if (parent.Role == Role.Screener)
            {
                Screen = new Button
                {
                    Content = "SCREEN",
                    IsEnabled = false,
                };
                Screen.Click += Screen_Click;
                Grid.SetRow(Screen, 1);
                ButtonBar.Children.Add(Screen);
            }

            Hangup = new Button()
            {
                Content = "HANGUP",
                IsEnabled = false,
            };
            Hangup.Click += Hangup_Click;
            Grid.SetRow(Hangup, (parent.Role == Role.Screener ? 2 : 1));
            ButtonBar.Children.Add(Hangup);
        }

        private void DurationUpdater_Tick(object sender, EventArgs e)
        {
            if (created != DateTime.MinValue)
                Duration.Content = (DateTime.Now - created).ToString("c");
        }

        private string formatNumber(string number)
        {
            if (!number.StartsWith("+1"))
                return number;
            StringBuilder sb = new StringBuilder();
            sb.Append('(');
            sb.Append(number.Substring(2, 3));
            sb.Append(") ");
            sb.Append(number.Substring(5, 3));
            sb.Append('-');
            sb.Append(number.Substring(8, 4));
            return sb.ToString();
        }

        public void Update()
        {
            switch (Line.State)
            {
                case LineState.Empty:
                    Air.IsEnabled = false;
                    if (parent.Role == Role.Screener)
                        Screen.IsEnabled = false;
                    Hangup.IsEnabled = false;
                    break;
                case LineState.Hold:
                    Air.IsEnabled = true;
                    if (parent.Role == Role.Screener)
                        Screen.IsEnabled = false;
                    Hangup.IsEnabled = true;
                    break;
                case LineState.OnAir:
                    Air.IsEnabled = false;
                    if (parent.Role == Role.Screener)
                        Screen.IsEnabled = false;
                    Hangup.IsEnabled = true;
                    break;
                case LineState.Ringing:
                    Air.IsEnabled = true;
                    if (parent.Role == Role.Screener)
                        Screen.IsEnabled = true;
                    Hangup.IsEnabled = true;
                    break;
                case LineState.Screening:
                    Air.IsEnabled = false;
                    if (parent.Role == Role.Screener)
                        Screen.IsEnabled = true;
                    Hangup.IsEnabled = true;
                    break;
            }

            if (parent.Role == Role.Screener)
                Screen.Content = Line.State == LineState.Screening ? "UNSCREEN" : "SCREEN";

            if (Line.Caller == null)
            {
                Clear();
                return;
            }
            SetColors(Line.State);
            CallerName.Content = string.IsNullOrWhiteSpace(Line.Caller.Name) ? formatNumber(Line.Caller.Number) : Line.Caller.Name;
            CallerLocation.Content = Line.Caller.Location;
            Comment.Text = Line.Caller.Comment;
            created = Line.Caller.Created;
            if (!durationUpdater.IsEnabled)
                durationUpdater.Start();
        }

        public void Clear()
        {
            SetColors(LineState.Empty);
            CallerName.Content = "";
            CallerLocation.Content = "";
            Comment.Text = "";
            durationUpdater.Stop();
            Duration.Content = "";
        }

        private static IReadOnlyDictionary<LineState, Color> mainBackground = new Dictionary<LineState, Color>
        {
            { LineState.Empty,     Color.FromRgb(51, 51, 51)   },
            { LineState.Ringing, Color.FromRgb(52, 152, 219) },
            { LineState.Screening, Color.FromRgb(230, 126, 34) },
            { LineState.Hold,  Color.FromRgb(46, 204, 113) },
            { LineState.OnAir,     Color.FromRgb(231, 76, 60)  },
        };

        private static IReadOnlyDictionary<LineState, Color> numberBackground = new Dictionary<LineState, Color>
        {
            { LineState.Empty,     Color.FromRgb(42, 42, 42)   },
            { LineState.Ringing, Color.FromRgb(41, 128, 185) },
            { LineState.Screening, Color.FromRgb(211, 84, 0)   },
            { LineState.Hold,  Color.FromRgb(39, 174, 96)  },
            { LineState.OnAir,     Color.FromRgb(192, 57, 43)  },
        };

        private static IReadOnlyDictionary<LineState, Color> numberForeground = new Dictionary<LineState, Color>
        {
            { LineState.Empty,     Color.FromRgb(0x7A, 0x7A, 0x7A) },
            { LineState.Ringing, Colors.White },
            { LineState.Screening, Colors.White },
            { LineState.Hold,  Colors.White },
            { LineState.OnAir,     Colors.White },
        };

        private static IReadOnlyDictionary<LineState, Color> durationForeground = new Dictionary<LineState, Color>
        {
            { LineState.Empty,     Color.FromRgb(0x95, 0x95, 0x95) },
            { LineState.Ringing, Color.FromRgb(0xFF, 0xFF, 0xFF) },
            { LineState.Screening, Color.FromRgb(0xFF, 0xFF, 0xFF) },
            { LineState.Hold,  Color.FromRgb(0x95, 0x95, 0x95) },
            { LineState.OnAir,     Color.FromRgb(0x22, 0x22, 0x22) },
        };

        public void SetColors(LineState state)
        {
            MainGrid.Background = new SolidColorBrush(mainBackground[state]);
            Index.Background = new SolidColorBrush(numberBackground[state]);
            Index.Foreground = new SolidColorBrush(numberForeground[state]);
            Duration.Foreground = new SolidColorBrush(durationForeground[state]);
        }

        private void Hangup_Click(object sender, RoutedEventArgs e)
        {
            if (Line == null || Line.Caller == null)
                return;
            parent.Client.Send(new RequestDump(Line.Index));
        }

        private void Screen_Click(object sender, RoutedEventArgs e)
        {
            if (Line == null || Line.Caller == null)
                return;
            parent.Client.Send(new RequestScreen(Line.Index));
        }

        private void Air_Click(object sender, RoutedEventArgs e)
        {
            if (Line == null || Line.Caller == null)
                return;
            parent.Client.Send(new RequestAir(Line.Index));
        }
    }
}
