using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgorithmController : MonoBehaviour
{
    public static AlgorithmController _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError($"Multiple instances of type {this}");
            return;
        }

        _instance = this;
    }

    private static AlgorithmBase[] algorithms;

    [SerializeField] private LineRenderer lr;

    public static bool ShowGeneration => _instance._showGeneration;
    [SerializeField] private bool _showGeneration;

    public static bool Benchmark => _instance._benchmark;
    [SerializeField] private bool _benchmark;

    public static bool AlgorithmIsRunning;

    public static int benchmarkIterations = 100_000;

    private AlgorithmBase currentRunningAlgorithm;

    public void Start()
    {
        algorithms = GetComponents<AlgorithmBase>(); //please dear god be in the right order please
    }

    public static void ResetLineRenderer()
    {
        _instance.lr.enabled = false;
    }

    public static void RunAlgorithm(int id)
    {
        if (AlgorithmIsRunning)
        {
            return;
        }

        _instance.currentRunningAlgorithm = algorithms[id];

        Vector2Int startPos = WaypointController.GetStartPos();
        Vector2Int endPos = WaypointController.GetEndPos();

        if (startPos == new Vector2Int(-1, -1))
        {
            Debug.LogError("No startpoint");
            return;
        }

        if (endPos == new Vector2Int(-1, -1))
        {           
            Debug.LogError("No endpoint");
            return;
        }

        GridController.ResetGrid();

        AlgorithmIsRunning = true;
        _instance.currentRunningAlgorithm.GeneratePath(startPos, endPos);                
    }

    public static void StopAlgorithm()
    {
        if (ShowGeneration)
        {
            _instance.currentRunningAlgorithm.StopAllCoroutines();
            
        }

        GridController.ResetGrid();
        AlgorithmIsRunning = false;
        _instance.currentRunningAlgorithm = null;
    }

    public static void ShowPath(Path path)
    {
        //GridController.ResetGrid();      

        Vector3[] vertexPositions = new Vector3[path.path.Count + 2];
        _instance.lr.positionCount = vertexPositions.Length;

        Vector2Int endPos = WaypointController.GetEndPos();
        vertexPositions[0] = new Vector3(endPos.x, endPos.y);

        int index = 1;
        foreach (Vector2Int nodePos in path.path)
        {
            GridNode node = GridController.GetNodeAtNodePos(nodePos);
            node.ShowAsPath();
            node.MarkAsChanged();

            vertexPositions[index] = new Vector3(nodePos.x, nodePos.y);
            index++;
        }
        
        Vector2Int startPos = WaypointController.GetStartPos();
        vertexPositions[index] = new Vector3(startPos.x, startPos.y);

        _instance.lr.SetPositions(vertexPositions);

        _instance.lr.enabled = true;

        AlgorithmIsRunning = false;
    }


    public void SetShowMode(bool value)
    {
        _showGeneration = value;
    }

    public void SetBechmarkMode(bool value)
    {
        _benchmark = value;

        UIController._instance.OpenBenchmarkUI(value);
    }

    public void SetBenchmarkIterations(string value)
    {
        int.TryParse(value, out benchmarkIterations);
    }

    public static void DisplayBenchmark(long milliseconds, int nodes, int pathLength)
    {
        UIController._instance.OpenBenchmarkUI(true);

        //int microSeconds = Mathf.RoundToInt((float)milliseconds * 1000 / BENCHMARK_ITERATIONS); //microseconds per algorithm pass

        float microSeconds = ((float)milliseconds * 1000 / benchmarkIterations);

        UIController._instance.Benchmark_Time.text = microSeconds.ToString();
        UIController._instance.Benchmark_Nodes.text = nodes.ToString();
        UIController._instance.Benchmark_PathLength.text = (pathLength + 1).ToString();
    }
}
