﻿using UnityEngine;
using System.Collections;

public class MapGennerator : MonoBehaviour {
	
	public enum DrawMode
	{
		NoiseMap, ColourMap, Mesh
	}

	public DrawMode drawMode;

	public const int mapChunkSize = 241;
	[Range(0,6)]
	public int levelOfDetail;
    public float noiseScale;

    public int octaves;
	[Range(0,1)]
    public float persistance;
    public float lacunarity;

	public int seed;
	public Vector2 offset;

	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

	public TerrainType[] regions;

    public void GenerateMap()
    {
		//generate noise map
		float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);

		//assign colors to specific parts of map
		Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
		for (int y = 0; y < mapChunkSize; y++) {
			for (int x = 0; x < mapChunkSize; x++) {
				float currenntHeight = noiseMap [x, y];
				for (int i = 0; i < regions.Length; i++) {
					if (currenntHeight <= regions [i].height) {
						colourMap [y * mapChunkSize + x] = regions [i].colour;
						break;
					}
				}
			}
		}

        MapDisplay display = FindObjectOfType<MapDisplay>();
		//draw texture to display dependong on drawmode
		if (drawMode == DrawMode.NoiseMap) {
			display.DrawTexture (TextureGenerator.TextureFromHeightMap (noiseMap));
		} else if (drawMode == DrawMode.ColourMap) {
			display.DrawTexture (TextureGenerator.TextureFromColourMap (colourMap, mapChunkSize, mapChunkSize));
		} else if (drawMode == DrawMode.Mesh) {
			display.DrawMesh (MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail),TextureGenerator.TextureFromColourMap (colourMap, mapChunkSize, mapChunkSize));
		}
    }

	void onValidate() {
		if (lacunarity < 1) {
			lacunarity = 1;
		}
		if (octaves < 0) {
			octaves = 0;
		}

	}

}
//struct for terrain types
[System.Serializable]
public struct TerrainType{

	public string name; 
	public float height;
	public Color colour;

}
