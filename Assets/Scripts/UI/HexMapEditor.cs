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
    public Material terrainMaterial;
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
    bool editMode;



    void Awake()
    {
        terrainMaterial.DisableKeyword("GRID_ON");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButton(0))
            {
                HandleInput();
                return;
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    DestroyUnit();
                } else
                {
                    CreateUnit();
                }
                return;
            }
        }
        //previousCell = null;
    }

    void HandleInput()
    {
        HexCell currentCell = GetCellUnderCursor();
        if (editMode)
        {
            EditCells(currentCell);
        } else
        {
            hexGrid.FindDistancesTo(currentCell);
        }
    }

    HexCell GetCellUnderCursor()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            return hexGrid.GetCell(hit.point);
        }
        return null;
    }

    void CreateUnit()
    {
        HexCell cell = GetCellUnderCursor();
        if (cell && !cell.Unit)
        {
            hexGrid.AddUnit(Instantiate(HexUnit.unitPrefab), cell, Random.Range(0f, 360f));
        }
    }

    void DestroyUnit()
    {
        HexCell cell = GetCellUnderCursor();
        if (cell && cell.Unit)
        {
            hexGrid.RemoveUnit(cell.Unit);
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

    public void SetEditMode(bool toggle)
    {
        editMode = toggle;
        hexGrid.ShowUI(!toggle);
    }

    public void ShowUI(bool visible)
    {
        hexGrid.ShowUI(visible);
    }

    public void ShowGrid (bool visible)
    {
        if (visible)
        {
            terrainMaterial.EnableKeyword("GRID_ON");
        } else
        {
            terrainMaterial.DisableKeyword("GRID_ON");
        }
    }
}
