using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the entire map/grid (cells inside chunks)
/// </summary>
public class HexGrid : MonoBehaviour
{
    public int cellCountX = 20, cellCountZ = 15;
    public int chunkCountX, chunkCountZ;

    public Color[] colors;

    public HexCell cellPrefab;
    public Text cellLabelPrefab;

    public HexGridChunk chunkPrefab;
    public int seed = 0;

    HexGridChunk[] chunks;
    HexCell[] cells;
    [System.NonSerialized] List<HexCell> frontier;
    List<HexUnit> units = new List<HexUnit>();
    public HexUnit unitPrefab;

    void Awake()
    {

        HexMetrics.InitializeHashGrid(seed);
        HexMetrics.colors = colors;
        HexUnit.unitPrefab = unitPrefab;
        CreateMap(cellCountX, cellCountZ);
    }

    public bool CreateMap(int x, int z)
    {
        if ( x <= 0 || x % HexMetrics.chunkSizeX != 0 ||
            z <= 0 || z % HexMetrics.chunkSizeZ != 0)
        {
            Debug.LogError("Unsupported map size. X,Z must be a multiple of " + HexMetrics.chunkSizeX + ", " + HexMetrics.chunkSizeZ);
            return false;
        }

        ClearUnits();

        if (chunks != null)
        {
            for (int i = 0; i < chunks.Length; i++)
            {
                Destroy(chunks[i].gameObject);
            }
        }
        cellCountX = x;
        cellCountZ = z;
        chunkCountX = cellCountX / HexMetrics.chunkSizeX;
        chunkCountZ = cellCountZ / HexMetrics.chunkSizeZ;

        CreateChunks();
        CreateCells();
        return true;
    }

    void CreateCells()
    {
        cells = new HexCell[cellCountZ * cellCountX];

        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];
        for (int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        return cells[index];
    }

    public HexCell GetCell(HexCoordinates coordinates)
    {
        int z = coordinates.Z;
        if (z < 0 || z >= cellCountZ)
        {
            return null;
        }
        int x = coordinates.X + z / 2;
        if (x < 0 || x >= cellCountX)
        {
            return null;
        }
        return cells[x + z * cellCountX];
    }

    public void ShowUI(bool visible)
    {
        for (int i = 0; i < chunks.Length; i++)
        {
            chunks[i].ShowUI(visible);
        }
    }

    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

        // Cell connections
        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }
        if (z > 0)
        {
            // Rows zig zag so we have to deal with odd/even rows differently
            if ((z & 1)==0) // bitwise AND. even numbers have LSB=0
            {
                // Even rows
                cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            } else
            {
                // Odd rows
                cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                if (x < cellCountX - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z-HexMetrics.innerRadius*0.5f);
        cell.uiRect = label.rectTransform;
        cell.Elevation = 0;

        AddCellToChunk(x, z, cell);
    }

    void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(cellCountX);
        writer.Write(cellCountZ);
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Save(writer);
        }

        writer.Write(units.Count);
        for (int i = 0; i < units.Count; i++)
        {
            units[i].Save(writer);
        }
    }

    public void Load(BinaryReader reader)
    {
        ClearUnits();
        StopAllCoroutines();
        int x = reader.ReadInt32();
        int z = reader.ReadInt32();

        if (x != cellCountX || z != cellCountZ)
        {
            if (!CreateMap(x, z))
            {
                return;
            }
        }

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Load(reader);
        }
        for (int i = 0; i < chunks.Length; i++)
        {
            chunks[i].Refresh();
        }

        int unitCount = reader.ReadInt32();
        for (int i = 0; i < unitCount; i++)
        {
            HexUnit.Load(reader, this);
        }
    }
    public void FindDistancesTo(HexCell cell)
    {
        StopAllCoroutines();
        StartCoroutine(Search(cell));
    }

    IEnumerator Search(HexCell cell)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Distance = int.MaxValue;
        }
        WaitForSeconds delay = new WaitForSeconds(1 / 60f);
        //Queue<HexCell> frontier = new Queue<HexCell>();
        frontier = ListPool<HexCell>.Get();
        cell.Distance = 0;
        frontier.Add(cell);
        //frontier.Enqueue(cell);
        while (frontier.Count>0)
        {
            yield return delay;
            HexCell current = frontier[0];
            frontier.RemoveAt(0);
            //HexCell current = frontier.Dequeue();
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (neighbor == null || neighbor.Distance != int.MaxValue)
                {
                    continue;
                }
                if (neighbor.IsUnderwater)
                {
                    continue;
                }
                if (current.GetEdgeType(neighbor) == HexEdgeType.Cliff)
                {
                    continue;
                }
                neighbor.Distance = current.Distance + 1;
                frontier.Add(neighbor);
                //frontier.Enqueue(neighbor);
            }
        }
        for (int i = 0; i < cells.Length; i++)
        {
            yield return delay;
            cells[i].Distance =
                cell.coordinates.DistanceTo(cells[i].coordinates);
        }
        ListPool<HexCell>.Add(frontier);
    }

    void ClearUnits()
    {
        for (int i = 0; i < units.Count; i++)
        {
            units[i].Die();
        }
        units.Clear();
    }

    public void AddUnit(HexUnit unit, HexCell location, float orientation)
    {
        units.Add(unit);
        unit.transform.SetParent(transform, false);
        unit.Location = location;
        unit.Orientation = orientation;
    }

    public void RemoveUnit(HexUnit unit)
    {
        units.Remove(unit);
        unit.Die();
    }
}
