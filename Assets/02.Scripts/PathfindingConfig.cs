using UnityEngine;
using Pathfinding;

public class PathfindingConfig : MonoBehaviour
{
    [Header("Pathfinding Settings")]
    [Tooltip("Set to Four for orthogonal movement (no diagonal), Eight for diagonal movement")]
    public NumNeighbours neighbourConnections = NumNeighbours.Four;

    void Start()
    {
        ConfigureGridGraph();
    }

    public void ConfigureGridGraph()
    {
        // Tìm GridGraph
        GridGraph gridGraph = AstarPath.active.data.gridGraph;

        if (gridGraph != null)
        {
            // Thay đổi số connections
            gridGraph.neighbours = neighbourConnections;

            // Nếu set thành Four, tắt cutCorners để tránh di chuyển chéo
            if (neighbourConnections == NumNeighbours.Four)
            {
                gridGraph.cutCorners = false;
            }

            Debug.Log($"[PathfindingConfig] Set grid connections to: {neighbourConnections}");

            // Scan lại graph để áp dụng thay đổi
            AstarPath.active.Scan(gridGraph);
        }
        else
        {
            Debug.LogError("[PathfindingConfig] GridGraph not found!");
        }
    }

    // Gọi từ inspector để test
    [ContextMenu("Apply Four Directions")]
    public void SetFourDirections()
    {
        neighbourConnections = NumNeighbours.Four;
        ConfigureGridGraph();
    }

    [ContextMenu("Apply Eight Directions")]
    public void SetEightDirections()
    {
        neighbourConnections = NumNeighbours.Eight;
        ConfigureGridGraph();
    }
}
