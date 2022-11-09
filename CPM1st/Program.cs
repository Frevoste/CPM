
using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;

namespace CPMAlgorithm
{
    public class Node
    {
        public int? eS;
        public int? eF;
        public int? lS;
        public int? lF;
        public int? slack;

        public string[]? immediatePredecessors;
        public List<string>? nextNodes = new List<string>();
        public int expectedTime;

        public bool isStartingNode = false;
        public bool isEndingNode = true;
        public bool isCalculated = false;
        public bool canCalculate = false;

        public string nodeKey;

        public void CheckIfStartingNode()
        {
            if(this.immediatePredecessors.Length == 0)
            {
                this.isStartingNode = true;
                this.eS = 0;
                this.eF = this.eS + this.expectedTime;
                this.isCalculated = true;
            }
        }
        

    }
    class NodeDictionary
    {
        public static Dictionary<string,Node> nodes = new Dictionary<string,Node>();
        public static Dictionary<string, Node> criticalNodes = new Dictionary<string, Node>();

       
        public static void AddNode(string letter, string[]? iP,int eT)
        {
            try
            {
                nodes.Add(letter, new Node() { immediatePredecessors = iP, expectedTime = eT });
                
            }
            catch(ArgumentException)
            {
                Console.WriteLine("An element with Key {0} already exist.",letter);
            }
            nodes[letter].CheckIfStartingNode();
            
            //If previous key appear in iP list, change it isEndingNode to false.
            foreach (var immediatePredecessor in iP)
            {
                try
                {
                    nodes[immediatePredecessor].isEndingNode = false;
                    nodes[immediatePredecessor].nextNodes.Add(letter);
                }
                catch(KeyNotFoundException)
                {
                    Console.WriteLine("An element with Key {0} is not a node.", immediatePredecessor);
                }
            }
        }
        public static void PrintNodeInformation(Dictionary<string,Node> nodes)
        {
            foreach (var item in nodes)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("____________________________");
                if (item.Value.isStartingNode)
                {
                    Console.WriteLine("______________");
                    Console.WriteLine("                             STARTING NODE");
                }
                    
                else if (item.Value.isEndingNode)
                {
                    Console.WriteLine("______________");
                    Console.WriteLine("                              ENDING NODE");
                }
                else
                    Console.WriteLine("");
                Console.WriteLine(" Key:");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  {0}              ", item.Key);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" Immiediate predecessors: ");
                Console.ForegroundColor = ConsoleColor.Blue;
                foreach (var predecessor in item.Value.immediatePredecessors)
                {
                    
                    Console.Write("  {0} ", predecessor);
             
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("");
                Console.WriteLine(" Next nodes:              ");
                Console.ForegroundColor = ConsoleColor.Green;
                foreach (var nextNodes in item.Value.nextNodes)
                {
                    
                    Console.Write("  {0} ", nextNodes);
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("");
                Console.WriteLine(" Values:              ");
                Console.WriteLine(" ES:{0}   EF:{1}  \n LS:{2}   LF:{3}", item.Value.eS, item.Value.eF, item.Value.lS, item.Value.lF);
                Console.WriteLine(" Slack:              ");
                Console.WriteLine(" {0} ", item.Value.slack);
            }
           
        }

        public static void PrintCriticalNodeInformation(Dictionary<string, Node> nodes)
        {
            foreach (var item in nodes)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Write("        Critical Path       ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine();
                Console.Write("____________________________");
                if (item.Value.isStartingNode)
                {
                    Console.WriteLine("______________");
                    Console.WriteLine("                             STARTING NODE");
                }

                else if (item.Value.isEndingNode)
                {
                    Console.WriteLine("______________");
                    Console.WriteLine("                              ENDING NODE");
                }
                else
                    Console.WriteLine("");
                Console.WriteLine(" Key:");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  {0}              ", item.Key);
                Console.ForegroundColor = ConsoleColor.White;

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("");
                Console.WriteLine(" Next critical node:              ");
                Console.ForegroundColor = ConsoleColor.Green;
                foreach (var nextNodes in item.Value.nextNodes)
                {
                    foreach (var criticalNode in criticalNodes)
                    {
                        if(criticalNode.Key == nextNodes)
                            Console.Write("  {0} ", nextNodes);
                    }
                    
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("");
                Console.WriteLine(" Values:              ");
                Console.WriteLine(" ES:{0}   EF:{1}  \n LS:{2}   LF:{3}", item.Value.eS, item.Value.eF, item.Value.lS, item.Value.lF);
                Console.WriteLine("vvvvvvvvvvvvvvvvvvvvvvvvvvvv");


            }

        }
        public static void CalculateEF()
        {
            List<string> listOfPossibleKeysToCalculate = new List<string>();
            List<string> listOfCalculatedKeys = new List<string>();
            List<string> listOfNewPossibleKeysToCalculate = new List<string>();
            foreach (var node in nodes)
            {
                if(node.Value.isStartingNode)
                {
                    foreach(var nextNode in node.Value.nextNodes)
                    {
                        listOfPossibleKeysToCalculate.Add(nextNode);
                    }
                }
            }

            while(true)
            {
                foreach(var possibleKeyToCalculate in listOfPossibleKeysToCalculate)
                {
                    bool canCalculate = true;
                    List<int?> possibleOutcome = new List<int?>();
                    foreach(var immediatePredecessor in nodes[possibleKeyToCalculate].immediatePredecessors)
                    {
                        if (!nodes[immediatePredecessor].isCalculated)
                            canCalculate = false;
                        else
                            possibleOutcome.Add(nodes[immediatePredecessor].eF);
                    }
                    if(canCalculate)
                    {
                        nodes[possibleKeyToCalculate].eS = possibleOutcome.Max();
                        nodes[possibleKeyToCalculate].eF = nodes[possibleKeyToCalculate].eS + nodes[possibleKeyToCalculate].expectedTime;
                        nodes[possibleKeyToCalculate].isCalculated = true;
                        foreach(var newPossibleNode in nodes[possibleKeyToCalculate].nextNodes)
                        {
                            listOfNewPossibleKeysToCalculate.Add(newPossibleNode);
                        }
                        listOfCalculatedKeys.Add(possibleKeyToCalculate);
                    }
                    
                }
                foreach (var newPossibleKeyToCalculate in listOfNewPossibleKeysToCalculate)
                {
                    listOfPossibleKeysToCalculate.Add(newPossibleKeyToCalculate);
                }
                foreach (var calculatedKey in listOfCalculatedKeys)
                {
                    listOfPossibleKeysToCalculate.Remove(calculatedKey);
                }
                if (listOfPossibleKeysToCalculate.Count == 0)
                    break;

            }
        }
        public static int? FindBiggestEF()
        {
            int? max = 0;
            foreach(var node in nodes)
            {
                if (node.Value.eF >= max)
                    max = node.Value.eF;
            }
            return max;
        }

        public static void FillLFInEndingNodes(int? max)
        {
            foreach(var node in nodes)
            {
                if (node.Value.isEndingNode)
                {
                    node.Value.lF = max;
                    node.Value.lS = node.Value.lF - node.Value.expectedTime;
                } 
                    
                    
            }
        }
        public static void CalculateLS()
        {
            int? max = FindBiggestEF();
            FillLFInEndingNodes(max);
            List<string> listOfPossibleKeysToCalculate = new List<string>();
            List<string> listOfCalculatedKeys = new List<string>();
            List<string> listOfNewPossibleKeysToCalculate = new List<string>();
            foreach (var node in nodes)
            {
                if (node.Value.isEndingNode)
                {
                    foreach (var immediatePredecessor in node.Value.immediatePredecessors)
                    {
                        listOfPossibleKeysToCalculate.Add(immediatePredecessor);
                    }
                }
            }
            while (true)
            {
                foreach (var possibleKeyToCalculate in listOfPossibleKeysToCalculate)
                {
                    bool canCalculate = true;
                    List<int?> possibleOutcome = new List<int?>();
                    foreach (var nextNode in nodes[possibleKeyToCalculate].nextNodes)
                    {
                        if (!nodes[nextNode].isCalculated)
                            canCalculate = false;
                        else
                            possibleOutcome.Add(nodes[nextNode].lS);
                    }
                    if (canCalculate)
                    {
                        nodes[possibleKeyToCalculate].lF = possibleOutcome.Min();
                        nodes[possibleKeyToCalculate].lS = nodes[possibleKeyToCalculate].lF - nodes[possibleKeyToCalculate].expectedTime;
                        nodes[possibleKeyToCalculate].isCalculated = true;
                        foreach (var newPossibleNode in nodes[possibleKeyToCalculate].immediatePredecessors)
                        {
                            listOfNewPossibleKeysToCalculate.Add(newPossibleNode);
                        }
                        listOfCalculatedKeys.Add(possibleKeyToCalculate);
                    }

                }
                foreach (var newPossibleKeyToCalculate in listOfNewPossibleKeysToCalculate)
                {
                    listOfPossibleKeysToCalculate.Add(newPossibleKeyToCalculate);
                }
                foreach (var calculatedKey in listOfCalculatedKeys)
                {
                    listOfPossibleKeysToCalculate.Remove(calculatedKey);
                }
                if (listOfPossibleKeysToCalculate.Count == 0)
                    break;

            }
        }

        public static void CalculateSlack()
        {
            foreach(var node in nodes)
            {
                node.Value.slack = node.Value.lS - node.Value.eS;
                node.Value.nodeKey = node.Key;
            }
        }

        public static void CriticalPath()
        {
            foreach (var node in nodes)
            {
                if (node.Value.slack.Equals(0))
                    criticalNodes.Add(node.Key, node.Value);
            }
        }

        public static void GantChartConsole()
        {
            Console.WriteLine();
            Console.WriteLine("Gantt Chart");
            Console.WriteLine();
            Console.Write(" Task |");
            for(int i=0; i< FindBiggestEF() ; i++)
            {
                if (i < 9)
                    Console.Write("0{0}|", i+1);
                else
                    Console.Write("{0}|",i+1);
            }
            Console.WriteLine("");
            foreach(var node in nodes)
            {
                Console.Write("  ");
                Console.Write(node.Key);
                Console.Write("   |");
                for(int i=0; i< FindBiggestEF(); i++)
                {
                    string key = "  ";
                    if (i >= node.Value.eS && i <= node.Value.lF)
                    {
                        key = "■■";
                        foreach(var critNode in criticalNodes)
                        {
                            if(critNode.Key.Equals(node.Key))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                            }
                        }
                    }
                        
                    else
                        key = "  ";

                    Console.Write("{0}|",key);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine("");
            }


        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //Sample data
            string[]? znakiA = { } ;
            NodeDictionary.AddNode("A",znakiA,7);
            string[]? znakiB = { };
            NodeDictionary.AddNode("B", znakiB, 9);
            string[]? znakiC = { "A" };
            NodeDictionary.AddNode("C", znakiC, 12);
            string[]? znakiD = { "A","B" };
            NodeDictionary.AddNode("D", znakiD, 8);
            string[]? znakiE = { "D" };
            NodeDictionary.AddNode("E", znakiE, 9);
            string[]? znakiF = { "C","E" };
            NodeDictionary.AddNode("F", znakiF, 6);
            string[]? znakiG = { "E" };
            NodeDictionary.AddNode("G", znakiG, 5);

            NodeDictionary.CalculateEF();
            NodeDictionary.CalculateLS();
            NodeDictionary.CalculateSlack();
            
            NodeDictionary.PrintNodeInformation(NodeDictionary.nodes);

            NodeDictionary.CriticalPath();
            NodeDictionary.PrintCriticalNodeInformation(NodeDictionary.criticalNodes);
            NodeDictionary.GantChartConsole();
        }
    }
}