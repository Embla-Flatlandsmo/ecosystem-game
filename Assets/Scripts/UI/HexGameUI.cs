//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.EventSystems;

//public class HexGameUI : MonoBehaviour
//{
//    public HexGrid grid;
//    HexCell currentCell;
//    HexUnit selectedUnit;

//    void Update()
//    {
//        if (!EventSystem.current.IsPointerOverGameObject())
//        {
//            if (Input.GetMouseButtonDown(0))
//            {
//                DoSelection();
//            }
//        }
//    }

//    public void SetEditMode(bool toggle)
//    {
//        enabled = !toggle;
//        grid.ShowUI(!toggle);
//    }

//    bool UpdateCurrentCell()
//    {
//        HexCell cell = grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
//        if (currentCell != currentCell)
//        {
//            currentCell = currentCell;
//            return true;
//        }
//        return false;
//    }

//    void DoSelection()
//    {
//        UpdateCurrentCell();
//        if (currentCell)
//        {
//            selectedUnit = currentCell.Unit;
//        }
//    }
//}
