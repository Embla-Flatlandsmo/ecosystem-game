using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for features (trees, plants etc) for a single chunk
/// </summary>
public class HexFeatureManager : MonoBehaviour
{
    //public Transform treePrefab;
    public HexFeatureCollection[] treeCollections, stoneCollections; //Because 2D arrays are visible in UI

    Transform container;
    public void Clear()
    {
        if (container)
        {
            Destroy(container.gameObject);
        }
        container = new GameObject("Features Container").transform;
        container.SetParent(transform, false);
    }
    public void Apply()
    {

    }

    public void AddFeature(HexCell cell, Vector3 position)
    {
        HexHash hash = HexMetrics.SampleHashGrid(position);
        Transform prefab = PickPrefab(treeCollections, cell.TreeLevel, hash.a, hash.d);
        Transform stonePrefab = PickPrefab(stoneCollections, cell.StoneLevel, hash.b, hash.d);
        if (prefab)
        {
            if (stonePrefab && hash.b < hash.a)
            {
                prefab = stonePrefab;
            }
        } else if (stonePrefab)
        {
            prefab = stonePrefab;
        } else
        {
            return;
        }

        Transform instance = Instantiate(prefab);
        //position.y += instance.localScale.y * 0.5f;
        instance.localPosition = position;
        instance.localRotation = Quaternion.Euler(0f, 360f * hash.e, 0f);
        instance.SetParent(container, false);
    }

    Transform PickPrefab(HexFeatureCollection[] collection, int level, float hash, float choice)
    {
        if (level > 0)
        {
            float[] thresholds = HexMetrics.GetFeatureThresholds(level - 1);
            for (int i = 0; i < thresholds.Length; i++)
            {
                if (hash < thresholds[i])
                {
                    return collection[i].Pick(choice)  ;
                }
            }
        }
        return null;
    }
}
