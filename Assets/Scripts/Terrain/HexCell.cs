using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;


/// <summary>
/// Stores information about each cell (color, elevation, etc)
/// </summary>
public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;

    public HexCell PathFrom { get; set; }
    public HexUnit Unit { get; set; }

    public Vector3 Position
    {
        get
        {
            return transform.localPosition;
        }
    }

    int terrainTypeIndex;
    public Color Color
    {
        get
        {
            return HexMetrics.colors[terrainTypeIndex];
        }
    }

    public int TerrainTypeIndex
    {
        get
        {
            return terrainTypeIndex;
        }
        set
        {
            if (terrainTypeIndex != value)
            {
                terrainTypeIndex = value;
                Refresh();
            }
        }
    }

    public int Elevation
    {
        get
        {
            return elevation;
        }
        set
        {
            if (elevation == value)
            {

                return;
            }
            elevation = value;
            RefreshPosition();
            Refresh();
        }
    }
    int elevation = int.MinValue;

    public int WaterLevel
    {
        get
        {
            return waterLevel;
        }
        set
        {
            if (waterLevel == value)
            {
                return;
            }
            waterLevel = value;
            Refresh();
        }
    }
    int waterLevel = 1;

    public float WaterSurfaceY
    {
        get
        {
            return (waterLevel + HexMetrics.waterElevationOffset) * HexMetrics.elevationStep;
        }
    }
    public bool IsUnderwater
    {
        get
        {
            return waterLevel > elevation;
        }
    }


    int treeLevel;

    public int TreeLevel
    {
        get
        {
            return treeLevel;
        }
        set
        {
            if (treeLevel != value)
            {
                treeLevel = value;
                RefreshSelfOnly();
            }
        }
    }

    int stoneLevel;

    public int StoneLevel
    {
        get
        {
            return stoneLevel;
        }
        set
        {
            if (stoneLevel != value)
            {
                stoneLevel = value;
                RefreshSelfOnly();
            }
        }
    }


    int distance;
    public int Distance
    {
        get
        {
            return distance;
        }
        set
        {
            distance = value;
            UpdateDistanceLabel();
        }
    }

    public HexGridChunk chunk;

    public RectTransform uiRect;

    [SerializeField]
    HexCell[] neighbors;

    public HexCell GetNeighbor (HexDirection direction)
    {
        return neighbors[(int)direction];
    }
    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(elevation, neighbors[(int)direction].elevation);
    }

    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(elevation, otherCell.elevation);
    }


    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    void RefreshSelfOnly()
    {
        chunk.Refresh();
        if (Unit)
        {
            Unit.ValidateLocation();
        }
    }

    void Refresh ()
    {
        if (chunk)
        {
            chunk.Refresh();
            for (int i = 0; i < neighbors.Length; i++)
            {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk)
                {
                    neighbor.chunk.Refresh();
                }
            }
            if (Unit)
            {
                Unit.ValidateLocation();
            }
        }
    }

    void RefreshPosition()
    {
        Vector3 position = transform.localPosition;
        position.y = elevation * HexMetrics.elevationStep;
        transform.localPosition = position;

        Vector3 uiPosition = uiRect.localPosition;
        uiPosition.z = elevation * -HexMetrics.elevationStep;
        uiRect.localPosition = uiPosition;
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write((byte)terrainTypeIndex);
        writer.Write((byte)elevation);
        writer.Write((byte)waterLevel);
        writer.Write((byte)treeLevel);
        writer.Write((byte)stoneLevel);
    }

    public void Load(BinaryReader reader)
    {
        terrainTypeIndex = reader.ReadByte();
        elevation = reader.ReadByte();
        RefreshPosition();
        waterLevel = reader.ReadByte();
        treeLevel = reader.ReadByte();
        stoneLevel = reader.ReadByte();
    } 

    public void DisableHighlight()
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();

        highlight.enabled = false;
    }

    public void EnableHighlight(Color color)
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        highlight.color = color;
        highlight.enabled = true;
    }
    void UpdateDistanceLabel()
    {
        Text label = uiRect.GetComponent<Text>();
        label.text = distance == int.MaxValue ? "" : distance.ToString();
    }
}
public enum HexEdgeType
{
    Flat, Slope, Cliff
}
