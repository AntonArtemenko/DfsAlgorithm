using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Lab7.Models;
using System.Threading;
using System.Diagnostics;

public class Graph
{
    private int _numberOfWays = 0;
    private int _edgeCount;  
    public int startVertex;
    public int endVertex;
    private List<int>[] adjList;
    public List<List<int>> listToWriteIntoJson = new List<List<int>>();

    public Graph()
    {
        InitVariables();
    }


    private void InitVariables()
    {
        var inputJsonSchema = ReadFromInputJson("./input.json");
        var input = JsonConvert.DeserializeObject<Input>(inputJsonSchema);
        startVertex = int.Parse(input.Start);
        endVertex = int.Parse(input.End);
        adjList = ParseFromJsonToArrayOfLists(input.Map);
    }


    private List<int>[] ParseFromJsonToArrayOfLists(IReadOnlyList<string[]> input)
    {
        this._edgeCount = input.Count;
        List<int>[] result = new List<int>[_edgeCount];
        for (var i = 0; i < input.Count; i++)
        {
            result[i] = new List<int>();
            for (int j = 0; j < input[i].Length; j++)
            {
                result[i].Add(int.Parse(input[i][j]));
            }
        }
        return result;
    }


    private static string ReadFromInputJson(string path)
    {
        using (var streamReader = new StreamReader(path, true))
        {
            return streamReader.ReadToEnd();
        }
    }

    public void FindAllWays(int s, int d)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        // the code that you want to measure comes here
        bool[] isVisited = new bool[_edgeCount];
        List<int> pathList = new List<int>();

        pathList.Add(s);

        DepthFirstTraversal(s, d, isVisited, pathList);
        watch.Stop();
        long microseconds = watch.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));
        Console.WriteLine($"My alg work time: {microseconds} microseconds");
    }

    public void DFSUtil(int v, bool[] visited)
    {
        // Mark the current node as visited and 
        // print it 
        visited[v] = true;
        Console.Write($"{v} ");

        // Recur for all the vertices adjacent 
        // to this vertex 
        
        foreach(var i in adjList[v])
            if (!visited[i])
                DFSUtil(i, visited);

    }

    // DFS traversal of the vertices reachable from v. 
    // It uses recursive DFSUtil() 
    public void DFS(int v)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        // Mark all the vertices as not visited 
        bool[] visited = new bool[_edgeCount];
        for (int i = 0; i < _edgeCount; i++)
            visited[i] = false;

        // Call the recursive helper function 
        // to print DFS traversal 
        DFSUtil(v, visited);
        long microseconds = watch.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));
        Console.WriteLine($"\nDFS classic alg work time: {microseconds} microseconds");
    }

    private void DepthFirstTraversal(int u, int d, bool[] isVisited, List<int> localPathList)
    {

        isVisited[u] = true;

        if (u.Equals(d))
        {
            listToWriteIntoJson.Add(new List<int>());
            Console.WriteLine(string.Join(" ", localPathList));
            for (int i = 0; i < localPathList.Count(); i++)
            {
                listToWriteIntoJson[_numberOfWays].Add(localPathList[i]);
            }

            _numberOfWays++;
            isVisited[u] = false;
            return;
        }
  
        foreach (int i in adjList[u])
        {
            if (!isVisited[i])
            {
                localPathList.Add(i);
                DepthFirstTraversal(i, d, isVisited,
                                    localPathList);

                localPathList.Remove(i);
            }
        } 
        isVisited[u] = false;
    }

    private static void WriteAllSuccessfulWaysToJson(string path, List<List<int>> localPathList)
    {
        var outputJsonSchema = JsonConvert.SerializeObject(localPathList, Formatting.Indented);
        using (var streamWriter = new StreamWriter(path))
        {
            streamWriter.WriteAsync(outputJsonSchema);
        }
    }

    public static void Main(String[] args)
    {  
        Graph g = new Graph();
        g.FindAllWays(g.startVertex, g.endVertex);
        g.DFS(0);
        WriteAllSuccessfulWaysToJson("./output.json", g.listToWriteIntoJson);
    }
}