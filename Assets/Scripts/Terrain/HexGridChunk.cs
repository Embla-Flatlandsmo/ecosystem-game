using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages cells inside a chunk
/// </summary>
public class HexGridChunk : MonoBehaviour
{
    HexCell[] cells;

    public HexMesh terrain, water, waterShore;
    public HexFeatureManager features;
    Canvas gridCanvas;

    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();

        cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
    }

    public void Refresh()
    {
        enabled = true;
    }

    private void LateUpdate()
    {
        Triangulate();
        enabled = false;
    }
    public void AddCell(int index, HexCell cell)
    {
        cells[index] = cell;
        cell.chunk = this;
        cell.transform.SetParent(transform, false);
        cell.uiRect.SetParent(gridCanvas.transform, false);
    }

    public void ShowUI (bool visible)
    {
        gridCanvas.gameObject.SetActive(visible);
    }

    public void Triangulate()
    {
        terrain.Clear();
        water.Clear();
        waterShore.Clear();
        features.Clear();
        for (int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }
        terrain.Apply();
        water.Apply();
        waterShore.Apply();
        features.Apply();
    }

    void Triangulate(HexCell cell)
    {
        for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++)
        {
            Triangulate(dir, cell);
        }
        if (!cell.IsUnderwater)
        {
            features.AddFeature(cell, cell.Position);
        }
    }

    void Triangulate(HexDirection direction, HexCell cell)
    {
        Vector3 center = cell.transform.localPosition;
        EdgeVertices e = new EdgeVertices(
            center + HexMetrics.GetFirstSolidCorner(direction),
            center + HexMetrics.GetSecondSolidCorner(direction));
        Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
        Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);
        terrain.AddTriangle(center, v1, v2);
        terrain.AddTriangleColor(cell.Color);

        HexCell neighbor = cell.GetNeighbor(direction) ?? cell;
        HexCell prevNeighbor = cell.GetNeighbor(direction.Previous()) ?? cell;
        HexCell nextNeighbor = cell.GetNeighbor(direction.Next()) ?? cell;

        if (cell.IsUnderwater)
        {
            TriangulateWater(direction, cell, center);
        } else
        {
            features.AddFeature(cell, (center + e.v1 + e.v5) * (1f / 3f));
        }
        if (direction == HexDirection.NE)
        {
            TriangulateConnection(direction, cell, v1, v2);
        }
        if (direction <= HexDirection.SE)
        {
            TriangulateConnection(direction, cell, v1, v2);
        }
    }

    void TriangulateWater(HexDirection direction, HexCell cell, Vector3 center)
    {
        center.y = cell.WaterSurfaceY;
        HexCell neighbor = cell.GetNeighbor(direction);

        if (neighbor != null && !neighbor.IsUnderwater)
        {
            TriangulateWaterShore(direction, cell, neighbor, center);
        } else
        {
            TriangulateWaterOpen(direction, cell, neighbor, center);
        }
    }

    void TriangulateWaterOpen(HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center)
    {
        Vector3 c1 = center + HexMetrics.GetFirstWaterCorner(direction);
        Vector3 c2 = center + HexMetrics.GetSecondWaterCorner(direction);
        water.AddTriangle(center, c1, c2);

        if (direction <= HexDirection.SE && neighbor != null)
        {
            Vector3 bridge = HexMetrics.GetWaterBridge(direction);
            Vector3 e1 = c1 + bridge;
            Vector3 e2 = c2 + bridge;
            water.AddQuad(c1, c2, e1, e2);
            if (direction <= HexDirection.E)
            {
                HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
                if (nextNeighbor == null || !nextNeighbor.IsUnderwater)
                {
                    return;
                }
                water.AddTriangle(
                    c2, e2, c2 + HexMetrics.GetWaterBridge(direction.Next()));
            }
        }
    }

    void TriangulateWaterShore(HexDirection direction, HexCell cell, HexCell neighbor, Vector3 center)
    {
        EdgeVertices e1 = new EdgeVertices(
            center + HexMetrics.GetFirstWaterCorner(direction),
            center + HexMetrics.GetSecondWaterCorner(direction));
        water.AddTriangle(center, e1.v1, e1.v2);
        water.AddTriangle(center, e1.v2, e1.v3);
        water.AddTriangle(center, e1.v3, e1.v4);
        water.AddTriangle(center, e1.v4, e1.v5);

        Vector3 bridge = HexMetrics.GetWaterBridge(direction);
        Vector3 center2 = neighbor.Position;
        center2.y = center.y;
        EdgeVertices e2 = new EdgeVertices(
            center2 + HexMetrics.GetSecondSolidCorner(direction.Opposite()),
            center2 + HexMetrics.GetFirstSolidCorner(direction.Opposite()));
        waterShore.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
        waterShore.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
        waterShore.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
        waterShore.AddQuad(e1.v4, e1.v5, e2.v4, e2.v5);
        waterShore.AddQuadUV(0f, 0f, 0f, 1f);
        waterShore.AddQuadUV(0f, 0f, 0f, 1f);
        waterShore.AddQuadUV(0f, 0f, 0f, 1f);
        waterShore.AddQuadUV(0f, 0f, 0f, 1f);

        HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
        if (nextNeighbor != null)
        {
            Vector3 v3 = nextNeighbor.Position + (nextNeighbor.IsUnderwater ?
                HexMetrics.GetFirstWaterCorner(direction.Previous()) :
                HexMetrics.GetFirstSolidCorner(direction.Previous()));
            v3.y = center.y;
            waterShore.AddTriangle(
                e1.v5, e2.v5, v3);
            waterShore.AddTriangleUV(
                new Vector2(0f, 0f),
                new Vector2(0f, 1f),
                new Vector2(0f, nextNeighbor.IsUnderwater ? 0f : 1f));
        }
    }

    void TriangulateConnection(
        HexDirection direction, HexCell cell, Vector3 v1, Vector3 v2)
    {
        HexCell neighbor = cell.GetNeighbor(direction);
        if (neighbor == null)
        {
            return;
        }

        Vector3 bridge = HexMetrics.GetBridge(direction);

        Vector3 v3 = v1 + bridge;
        Vector3 v4 = v2 + bridge;
        v3.y = v4.y = neighbor.Elevation * HexMetrics.elevationStep;
        HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
        if (direction <= HexDirection.E && nextNeighbor != null)
        {
            Vector3 v5 = v2 + HexMetrics.GetBridge(direction.Next());
            v5.y = nextNeighbor.Elevation * HexMetrics.elevationStep;
            terrain.AddTriangle(v2, v4, v5);
            terrain.AddTriangleColor(cell.Color, neighbor.Color, nextNeighbor.Color);
        }

        terrain.AddQuad(v1, v2, v3, v4);
        terrain.AddQuadColor(cell.Color, neighbor.Color);
    }


}
