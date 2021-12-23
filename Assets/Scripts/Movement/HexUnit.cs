using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HexUnit : MonoBehaviour
{

    public static HexUnit unitPrefab;
    HexCell location;
    public HexCell Location
    {
        get
        {
            return location;
        }
        set
        {
            if (location)
            {
                location.Unit = null;
            }
            location = value;
            value.Unit = this;
            transform.localPosition = value.Position;
        }
    }

    List<HexCell> pathToTravel;
    const float travelSpeed = 4f;

    float orientation;
    public float Orientation
    {
        get
        {
            return orientation;
        }
        set
        {
            orientation = value;
            transform.localRotation = Quaternion.Euler(0f, value, 0f);
        }
    }
    private void Start()
    {
        
    }
    private void OnDrawGizmos()
    {
        Debug.Log("OnDrawGizmos called");
        if (pathToTravel == null || pathToTravel.Count == 0)
        {
            return;
        }

        Vector3 a, b = pathToTravel[0].Position;

        for (int i = 1; i < pathToTravel.Count; i++)
        {
            a = b;
            b = (pathToTravel[i - 1].Position + pathToTravel[i].Position) * 0.5f;
            for (float t = 0f; t < 1f; t += 0.1f)
            {
                Gizmos.DrawSphere(Vector3.Lerp(a, b, t), 2f);
            }
        }

        a = b;
        b = pathToTravel[pathToTravel.Count - 1].Position;
        for (float t = 0f; t < 1f; t += 0.1f)
        {
            Gizmos.DrawSphere(Vector3.Lerp(a, b, t), 2f);
        }
    }

    IEnumerator TravelPath()
    {
        Vector3 a, b = pathToTravel[0].Position;
        for (int i = 1; i < pathToTravel.Count; i++)
        {
            a = b;
            b = (pathToTravel[i - 1].Position + pathToTravel[i].Position) * 0.5f;
            for (float t = 0f; t < 1f; t += Time.deltaTime*travelSpeed)
            {
                transform.localPosition = Vector3.Lerp(a, b, t);
                yield return null;
            }
        }
        a = b;
        b = pathToTravel[pathToTravel.Count - 1].Position;
        for (float t = 0f; t < 1f; t += Time.deltaTime*travelSpeed)
        {
            transform.localPosition = Vector3.Lerp(a, b, t);
            yield return null;
        }
    }

    public void ValidateLocation()
    {
        transform.localPosition = location.Position;
    }

    public void Die()
    {
        location.Unit = null;
        Destroy(gameObject);
    }

    public void Save(BinaryWriter writer)
    {
        location.coordinates.Save(writer);
        writer.Write(orientation);
    }

    public static void Load(BinaryReader reader, HexGrid grid)
    {
        HexCoordinates coordinates = HexCoordinates.Load(reader);
        float orientation = reader.ReadSingle();
        grid.AddUnit(Instantiate(unitPrefab), grid.GetCell(coordinates), orientation);
    }

    public bool IsValidDestination(HexCell cell)
    {
        return !cell.IsUnderwater && !cell.Unit;
    }

    public void Travel (List<HexCell> path)
    {
        Location = path[path.Count - 1];
        pathToTravel = path;
        StopAllCoroutines();
        StartCoroutine(TravelPath());
    }

    private void OnEnable()
    {
        if (location) // Prevent units from being stuck along path upon play mode recompilation
        {
            transform.localPosition = location.Position;
        }
    }

}
