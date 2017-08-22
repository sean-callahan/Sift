﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Sift.Client.Elements;
using Sift.Common;
using Sift.Common.Network;

namespace Sift.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Line> Lines { get; }

        public LineElement SelectedLine { get; private set; }

        public ScreenerElement Screener { get; }

        private IDictionary<Line, LineElement> elements;

        public SdpClient Client { get; }

        public MainWindow(SdpClient client, int lines)
        {
            Client = client;
            Client.UpdateLineState += Client_UpdateLineState;

            InitializeComponent();

            Screener = new ScreenerElement(Client);
            Grid.SetRow(Screener, 0);
            Grid.SetColumn(Screener, 0);
            ScreenerFrame.Content = Screener;

            Lines = new List<Line>();
            for (int i = 0; i < lines; i++)
                Lines.Add(new Line(i));

            ConstructLineGrid(LineGrid, Lines, out elements);
        }

        private void Client_UpdateLineState(object sender, UpdateLineState e)
        {
            if (e.Index >= Lines.Count)
                return;

            Line line = Lines[e.Index];
            line.State = e.State;

            if (e.State == LineState.Empty)
            {
                line.Caller = null;
                elements[line].Update();
                Screener.Line = null;
                return;
            }

            if (line.Caller == null)
                line.Caller = new Caller(e.Id);

            line.Caller.Number = e.Number;
            line.Caller.Name = e.Name;
            line.Caller.Location = e.Location;
            line.Caller.Comment = e.Comment;

            elements[line].Update();
        }

        private void lineElementClick(object sender, MouseButtonEventArgs e)
        {
            LineElement line = (LineElement)sender;
            foreach (LineElement el in elements.Values)
            {
                if (el != line)
                    el.ShowBorder = false;
            }
            SelectedLine = line;
            if (SelectedLine != null)
            {
                line.ShowBorder = true;
            }
            Screener.Line = SelectedLine.Line;
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
                        RowDefinition rowDef = new RowDefinition();
                        rowDef.MaxHeight = maxHeight;
                        g.RowDefinitions.Add(rowDef);
                    }

                    if (col * rows + row >= lines.Count)
                        continue;

                    Line line = lines[col * rows + row];

                    LineElement el = new LineElement(this, line);
                    el.Margin = new Thickness(5, 5, 0, 0);
                    el.HorizontalAlignment = HorizontalAlignment.Stretch;
                    el.VerticalAlignment = VerticalAlignment.Stretch;
                    el.MaxHeight = maxHeight;
                    el.MouseLeftButtonUp += lineElementClick;

                    Grid.SetRow(el, row);
                    Grid.SetColumn(el, col);

                    g.Children.Add(el);
                    elements[line] = el;
                }
            }
            g.RowDefinitions.Add(new RowDefinition());
        }
    }
}
