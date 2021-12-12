using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;

/// <summary>
/// Makes it possible to edit the hex grid
/// </summary>
public class HexMapEditor : MonoBehaviour
{
    public HexGrid hexGrid;
    private int activeElevation;
    private int activeWaterLevel;
    private int activeTreeLevel, activeStoneLevel;
    private int activeTerrainTypeIndex;

    public void SetTerrainTypeIndex(int index)
    {
        activeTerrainTypeIndex = index;
    }

    int brushSize;
    public void SetBrushSize(float size)
    {
        brushSize = (int)size;
    }

    bool applyElevation = true;
    bool applyWaterLevel = true;
    bool applyTreeLevel, applyStoneLevel;

    void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() && !EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            EditCells(hexGrid.GetCell(hit.point));
        }
    }

    void EditCells(HexCell center)
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;
        for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + brushSize; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }

        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
        {
            for (int x = centerX - brushSize; x <= centerX + r; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
    }

    void EditCell(HexCell cell)
    {
        if (cell)
        {
            if (activeTerrainTypeIndex >= 0)
            {
                cell.TerrainTypeIndex = activeTerrainTypeIndex;
            }
            if (applyElevation)
            {
                cell.Elevation = activeElevation;
            }
            if (applyWaterLevel)
            {
                cell.WaterLevel = activeWaterLevel;
            }
            if (applyTreeLevel)
            {
                cell.TreeLevel = activeTreeLevel;
            }
            if (applyStoneLevel)
            {
                cell.StoneLevel = activeStoneLevel;
            }
        }
    }

    public void SetApplyElevation (bool toggle)
    {
        applyElevation = toggle;
    }
    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }

    public void SetApplyWaterLevel(bool toggle)
    {
        applyWaterLevel = toggle;
    }

    public void SetWaterLevel(float level)
    {
        activeWaterLevel = (int)level;
    }

    public void SetApplyTreeLevel(bool toggle)
    {
        applyTreeLevel = toggle;
    }

    public void SetTreeLevel(float level)
    {
        activeTreeLevel = (int)level;
    }

    public void SetApplyStoneLevel(bool toggle)
    {
        applyStoneLevel = toggle;
    }

    public void SetStoneLevel(float level)
    {
        activeStoneLevel = (int)level;
    }
    public void ShowUI(bool visible)
    {
        hexGrid.ShowUI(visible);
    }

    public void Save()
    {
        string path = Path.Combine(Application.persistentDataPath, "test.map");
        Debug.Log("File saved in " + path);
        using (
            BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create))
        ) {
            writer.Write(0);
            hexGrid.Save(writer);
        }
        
    }

    public void Load()
    {
        string path = Path.Combine(Application.persistentDataPath, "test.map");
        using (
            BinaryReader reader =
                new BinaryReader(File.OpenRead(path))
        ) {
            int header = reader.ReadInt32();
            if (header == 0) {
                hexGrid.Load(reader);
            } else
            {
                Debug.LogWarning("Unknown map format: " + header);
            }

        }
    }
}
