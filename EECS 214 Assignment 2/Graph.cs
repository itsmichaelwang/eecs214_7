//Assignment 7 for EECS 214
//Out: May 30rd, 2014
//Due: June 6th, 2014

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Assignment_7
{
    //public class Graph : IEnumerable<int>
    public class Graph
    {
        //ENSURE THAT YOU HAVE THE ":IComparable<GraphNode>" below
        public class GraphNode : IComparable<GraphNode>
        {
            // GraphNode value and weighted edges
            public int Value { get; set; }
            public List<GraphNode> Neighbors = new List<GraphNode>();
            public List<int> Weights = new List<int>();
            
            // Details for displaying in UI
            public Point Position { get; set; }         // X and Y components in UI
            public String Name { get; set; }
            public bool IsHighlighted { get; set; }     // Should be drawn in red or not

            // Stores traversal path data
            private int cost;                           // Stores cumulative distance for Dijkstra's Algorithm
            public int Distance { get; set; }           // Stores distance for DFS/BFS traversal
            public GraphNode Predecessor { get; set; }

            //Property to get and set cost
            public int Cost
            {
                get { return cost; }
                set { cost = value; }
            }

            //YOU AVE TO INCLUDE THIS COMPARETO FUNCTION BECAUSE 
            //THE GRAPHNODE IMPLEMENTS THE ICOMPARABLE INTERFACE
            //DONT WORRY ABOUT IT, ITS A C# WAY OF COMPARING OBJECTS
            //TO RETURN IF AN OBJECT IS GREATER THAN OR LESS THAN ANOTHER OBJECT
            public int CompareTo(GraphNode other)
            {
                if (this.cost < other.cost) return -1;
                else if (this.cost > other.cost) return 1;
                else return 0;
            }
            
            // CONSTRUCTORS
            public GraphNode(int value)
            {
                Value = value;
                Position = new Point(0, 0);
                Name = null;
                IsHighlighted = false;
                Cost = int.MaxValue;
                Predecessor = null;
            }

            public GraphNode(int value, Point position, String name)
            {
                Value = value;
                Position = position;
                Name = name;
                IsHighlighted = false;
                Cost = int.MaxValue;
                Predecessor = null;
            }
        }

        //YOU SHOULD HAVE ALL THE STUFF FROM ASSIGNMENT 6 and ADD DIJKSTRA'S ALGORITHM to the GRAPH CLASS
        /// Implementation of Djikstra's Algorithm. 
        // Graph Constructor - Hardcoding a Map of America
        public Graph()
        {
            // Cities
            AddNode(new Graph.GraphNode(0, new Point(110, 50), "San Francisco"));
            AddNode(new Graph.GraphNode(1, new Point(290, 100), "Denver"));
            AddNode(new Graph.GraphNode(2, new Point(395, 75), "Chicago"));
            AddNode(new Graph.GraphNode(3, new Point(550, 100), "New York"));
            AddNode(new Graph.GraphNode(4, new Point(110, 200), "Los Angeles"));
            AddNode(new Graph.GraphNode(5, new Point(135, 280), "San Diego"));
            AddNode(new Graph.GraphNode(6, new Point(357, 281), "Dallas"));
            AddNode(new Graph.GraphNode(7, new Point(524, 339), "Miami"));

            // Flight Paths
            AddDirectedEdge(Nodes[0], Nodes[4], 45);    // SF to LA
            AddDirectedEdge(Nodes[1], Nodes[0], 90);    // Denver to SF
            AddDirectedEdge(Nodes[1], Nodes[4], 100);   // Denver to LA
            AddDirectedEdge(Nodes[2], Nodes[0], 60);    // Chicago to SF
            AddDirectedEdge(Nodes[2], Nodes[1], 25);    // Chicago to Denver
            AddDirectedEdge(Nodes[3], Nodes[1], 100);   // NY to Denver
            AddDirectedEdge(Nodes[3], Nodes[2], 80);    // NY to Chicago
            AddDirectedEdge(Nodes[3], Nodes[6], 125);   // NY to Dallas
            AddDirectedEdge(Nodes[3], Nodes[7], 90);    // NY to Miami
            AddDirectedEdge(Nodes[5], Nodes[4], 50);    // San Diego to LA
            AddDirectedEdge(Nodes[6], Nodes[4], 80);    // Dallas to LA
            AddDirectedEdge(Nodes[6], Nodes[5], 80);    // Dallas to San Diego
            AddDirectedEdge(Nodes[7], Nodes[6], 50);    // Miami to Dallas
        }

        //the Graph class should have a list of graphnodes  
        //something like private List<GraphNode> nodes;
        private List<GraphNode> nodes = new List<GraphNode>();

        /// <summary>
        /// Should add the node to the list of nodes
        /// </summary>
        public void AddNode(GraphNode node)
        {
            // adds a node to the graph, if it doesn't already exist
            if (!Contains(node.Value))
            {
                nodes.Add(node);
            }
        }

        /// <summary>
        /// Creates a new graphnode from an input value and adds it to the list of graph nodes
        /// </summary>
        public void AddNode(int value)
        {
            if (!Contains(value))
            {
                GraphNode node = new GraphNode(value);
                nodes.Add(node);
            }
        }

        /// <summary>
        /// Adds a directed edge from one GraphNode to another. 
        /// Also insert the corresponding weight 
        /// </summary>
        public void AddDirectedEdge(GraphNode from, GraphNode to, int weight)
        {
            from.Neighbors.Add(to);
            from.Weights.Add(weight);
        }

        /// <summary>
        /// Adds an undirected edge from one GraphNode to another. 
        /// Remember this means that both the from and two are neighbors of each other
        /// Also insert the corresponding weight 
        /// </summary>
        public void AddUndirectedEdge(GraphNode from, GraphNode to, int cost)
        {
            from.Neighbors.Add(to);
            from.Weights.Add(cost);
            to.Neighbors.Add(from);
            to.Weights.Add(cost);
        }

        // Check if a value exists in the graph or not
        public bool Contains(int value)
        {
            return nodes.Any(x => x.Value == value);
        }

        /// <summary>
        /// Should remove a node and all its edges and return true if successful
        /// </summary>
        public bool Remove(int value)
        {
            // If the value does not exist, return false
            if (!Contains(value))
            {
                return false;
            }

            foreach (GraphNode node in Nodes)
            {
                // Iterate through all the edges, and remove references to the value being removed
                for (int i = 0; i < node.Neighbors.Count; i++)
                {
                    if (node.Neighbors[i].Value == value)
                    {
                        node.Neighbors.Remove(node.Neighbors[i]);
                        node.Weights.Remove(node.Weights[i]);
                    }
                }

                // Remove the value itself from the Graph
                if (node.Value == value)
                {
                    nodes.Remove(node);
                    break;
                }
            }

            return true;
        }

        // Search an return a GraphNode by name
        public GraphNode Search(String searchStr)
        {
            foreach (GraphNode node in Nodes)
            {
                if (String.Equals(node.Name, searchStr))
                {
                    return node;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds the shortest path between connected components in a graph
        /// </summary>
        public List<GraphNode> DijkstraShortestPath(GraphNode start, GraphNode finish)
        {
            List<GraphNode> returnList = new List<GraphNode>(); //constains the list of nodes to go from start to finish
            PriorityQueue<GraphNode> PQ = new PriorityQueue<GraphNode>();

            // Prepare to insert nodes in the priority queue
            foreach (GraphNode node in Nodes)
            {
                node.Cost = int.MaxValue;
            }
            start.Cost = 0;

            // Insert them
            foreach (GraphNode node in Nodes)
            {
                PQ.Insert(node);
            }
            
            // The traversal algorithm
            GraphNode currentNode = null;
            while (PQ.Count() != 0)
            {
                currentNode = PQ.ExtractMin();

                // Break if we find the GraphNode we are looking for
                if (currentNode == finish)
                {
                    break;
                }

                // Error handling in case path was not found
                if (currentNode.Cost == int.MaxValue)
                {
                    return null;
                }

                // Iterate through neighbors to find cheapest path
                for (int i = 0; i < currentNode.Neighbors.Count; i++)
                {
                    int w = currentNode.Weights[i];     // Store traversal cost
                    int newCost = w + currentNode.Cost;

                    if (newCost < currentNode.Neighbors[i].Cost)
                    {
                        PQ.DecreaseKey(currentNode.Neighbors[i], newCost);
                        currentNode.Neighbors[i].Cost = newCost;
                        currentNode.Neighbors[i].Predecessor = currentNode;
                    }
                }
            }

            // At this point, finish should contain our destination Node, with the ability to trace it back
            while (finish != null)
            {
                returnList.Add(finish);
                finish = finish.Predecessor;
            }

            return returnList;
        }

        /// <summary>
        /// Prints out a list of connected components in a graph
        /// </summary>
        public void FindConnectedComponents()
        {
            // Keep track of what has been visited, and the # of subgroups
            bool[] VisitNodes = new bool[Nodes[Nodes.Count - 1].Value + 1];
            int subGroups = 0;

            foreach (GraphNode n in Nodes)
            {
                if (VisitNodes[n.Value] == false)
                {
                    System.Diagnostics.Debug.WriteLine("Group " + subGroups + ":");
                    VisitNodes[n.Value] = true;
                    DFS(n, VisitNodes);
                    subGroups++;
                }
            }

            System.Diagnostics.Debug.WriteLine("\n" + subGroups + " connected components total.\n");
        }

        private void DFS(GraphNode startNode, bool[] visitNodes)
        {
            System.Diagnostics.Debug.WriteLine(startNode.Value);        // Print Node
            foreach (GraphNode n in startNode.Neighbors)
            {
                if (visitNodes[n.Value] == false)
                {
                    visitNodes[n.Value] = true;                 // Mark as visited
                    DFS(n, visitNodes);                         // Move on
                }
            }
        }

        /// <summary>
        /// Is a property and returns the list of nodes, should be a one-liner
        /// </summary>
        public List<GraphNode> Nodes
        {
            get { return nodes; }
        }

        /// <summary>
        /// Is a property and returns the number of nodes in the graph, should be one-liner
        /// </summary>
        public int Count
        {
            get { return nodes.Count; }
        }

        // Iterate through the nodes the Nodes and returns integer values
        public IEnumerator<int> GetEnumerator()
        {
            foreach (GraphNode node in Nodes)
            {
                yield return node.Value;
            }
        }
    }
}
