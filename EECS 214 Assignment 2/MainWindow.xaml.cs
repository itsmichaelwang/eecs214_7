//Assignment 7 for EECS 214
//Out: May 30rd, 2014
//Due: June 6th, 2014
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Assignment_7
{
    /// <summary>
    /// THIS SHOULD REMAIN VERY SIMILAR TO ASSIGNMENT 6, INSTEAD OF CALLING BFS for SHORTEST PATH, CALL DIJKSTRA's 
    /// ALGORITHM 
    /// </summary>
    ///Don't worry about this being defined as a partial class, a partial class just allows you to split up code in 
    ///different files while telling the compiler to splice them together during compilation
    public partial class MainWindow : Window
    {
        // Initialize the Graph
        Graph flightPaths = new Graph();

        public MainWindow()
        {
            InitializeComponent();

            //Registering callback for the route finding button
            searchRouteBtn.Click += searchRouteBtn_Click;
            drawStructure();
        }

        void searchRouteBtn_Click(object sender, RoutedEventArgs e)
        {
            //Get the to and from airports from the fromInput and toInput textboxes in MainWindow.xaml.cs
            //Find the cheapest flight path and draw it using drawstructure
            Graph.GraphNode fromNode = flightPaths.Search(fromInput.Text);
            Graph.GraphNode toNode = flightPaths.Search(toInput.Text);

            // Textbox input error checking
            if (fromNode == null || toNode == null)
            {
                MessageBoxResult message = MessageBox.Show("Invalid search terms");
                return;
            }

            List<Graph.GraphNode> path = flightPaths.DijkstraShortestPath(fromNode, toNode);

            // Path not found error checking
            if (path == null)
            {
                MessageBoxResult message = MessageBox.Show("Path not found");
                return;
            }
            else
            {
                // Clear highlight preferences and replace with traversal results
                foreach (Graph.GraphNode node in flightPaths.Nodes)
                {
                    node.IsHighlighted = false;
                }
                foreach (Graph.GraphNode node in path)
                {
                    node.IsHighlighted = true;
                }
            }

            drawStructure();
        }

        //ALL YOUR OLD DRAWSTRUCTURE CODE
        //Draw graph for the flight paths
        private void drawStructure()
        {
            SolidColorBrush red = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            canvas.Children.Clear();

            //Feel free to use whatever colours you want
            //I've iterated through the position and name lists here just to give an example
            //you'll iterate through the graphnodes in your graph
            foreach (Graph.GraphNode node in flightPaths.Nodes)
            {
                // Draw the paths first
                foreach (Graph.GraphNode neighbor in node.Neighbors)
                {
                    Line connector = new Line();
                    connector.X1 = node.Position.X + 30 / 2; //30 is the width of the circle/node drawn
                    connector.Y1 = node.Position.Y + 30 / 2; //30 is the height of the circle/node drawn;
                    connector.X2 = neighbor.Position.X + 30 / 2;
                    connector.Y2 = neighbor.Position.Y + 30 / 2;
                    connector.StrokeThickness = 2;

                    // If the source and destination of the connector is highlighted, highlight the connector
                    if (node.IsHighlighted == true && neighbor.IsHighlighted == true)
                    {
                        connector.Stroke = red;
                    }
                    else
                    {
                        connector.Stroke = new SolidColorBrush(Color.FromRgb(150, 150, 175));
                    }

                    canvas.Children.Add(connector);
                }
            }

            // Draw Nodes representing each city - do this last to keep connectors under the nodes
            foreach (Graph.GraphNode node in flightPaths.Nodes)
            {
                Ellipse e = new Ellipse();
                e.Height = 30;
                e.Width = 30;
                e.Fill = new SolidColorBrush(Color.FromRgb(255, 200, 175));
                e.StrokeThickness = 3;

                // Highlight nodes
                if (node.IsHighlighted == true)
                {
                    e.Stroke = red;
                }
                else
                {
                    e.Stroke = new SolidColorBrush(Color.FromRgb(175, 175, 175));
                }

                Canvas.SetLeft(e, node.Position.X);
                Canvas.SetTop(e, node.Position.Y);
                canvas.Children.Add(e);

                Label l = new Label();
                l.Content = node.Name;
                Canvas.SetLeft(l, node.Position.X);
                Canvas.SetTop(l, node.Position.Y - 20);

                canvas.Children.Add(l);
            }
        }

    }
}
