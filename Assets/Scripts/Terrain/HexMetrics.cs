using UnityEngine;

/// <summary>
/// Constants
/// </summary>
public static class HexMetrics
{

	public const float outerRadius = 10f;
	public const float innerRadius = outerRadius * 0.866025404f; //sqrt(3)/2

	public const float solidFactor = 0.75f;
	public const float blendFactor = 1f - solidFactor;

	public const float elevationStep = 5f;

	public const int chunkSizeX = 5, chunkSizeZ = 5;

	public const float waterElevationOffset = -0.5f;
	public const float waterFactor = 0.6f;
	public const float waterBlendFactor = 1f - waterFactor;

	public const int hashGridSize = 256;
	public const float hashGridScale = 0.25f;
	static HexHash[] hashGrid;

	static float[][] featureThresholds =
	{
		new float[] {0.0f, 0.0f, 0.4f},
		new float[] {0.0f, 0.4f, 0.6f},
		new float[] {0.4f, 0.6f, 0.8f}
	};

	public static float [] GetFeatureThresholds(int level)
    {
		return featureThresholds[level];
    }

	public static void InitializeHashGrid(int seed)
    {
		hashGrid = new HexHash[hashGridSize * hashGridSize];
		Random.State currentState = Random.state;
		Random.InitState(seed);
		for (int i = 0; i < hashGrid.Length; i++)
        {
			hashGrid[i] = HexHash.Create();
        }
		Random.state = currentState;
    }

	public static HexHash SampleHashGrid(Vector3 position)
    {
		int x = (int)(position.x * hashGridScale) % hashGridSize;
		if (x < 0)
        {
			x += hashGridSize;
        }
		int z = (int)(position.z * hashGridScale) % hashGridSize;
		if (z < 0)
        {
			z += hashGridSize;
        }
		return hashGrid[x + z * hashGridSize];
    }

	static Vector3[] corners = {
		// This hexagon is orientated with point at the top
		new Vector3(0f, 0f, outerRadius),
		new Vector3(innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(0f, 0f, -outerRadius),
		new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(0f, 0f, outerRadius) // duplicate for iteration purposes in HexMesh
	};

	public static Vector3 GetFirstCorner(HexDirection direction)
    {
		return corners[(int)direction];
    }

	public static Vector3 GetSecondCorner(HexDirection direction)
    {
		return corners[(int)direction + 1];
    }

	public static Vector3 GetFirstSolidCorner(HexDirection direction)
	{
		return corners[(int)direction]*solidFactor;
	}
	public static Vector3 GetSecondSolidCorner(HexDirection direction)
	{
		return corners[(int)direction+1]*solidFactor;
	}

	public static Vector3 GetBridge (HexDirection direction)
    {
		return (corners[(int)direction] + corners[(int)direction + 1]) * blendFactor;
    }

	public static Vector3 GetFirstWaterCorner(HexDirection direction)
    {
		return corners[(int)direction] * waterFactor;
    }
	
	public static Vector3 GetSecondWaterCorner(HexDirection direction)
    {
		return corners[(int)direction + 1] * waterFactor;
    }
	public static Vector3 GetWaterBridge(HexDirection direction)
    {
		return (corners[(int)direction] + corners[(int)direction + 1]) * waterBlendFactor;
	}

}