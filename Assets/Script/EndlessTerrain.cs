using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EndlessTerrain : MonoBehaviour {

	public const float maxViewDist = 300;
	public Transform viewer;

	public static Vector2 viewerPostion;
	int chunkSize;
	int chunkVisibleInView;

	Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

	void Start(){
		chunkSize = MapGennerator.mapChunkSize - 1; //actual chunk size is 240
		chunkVisibleInView = Mathf.RoundToInt(maxViewDist / chunkSize);

	}

	void Update(){
		viewerPostion = new Vector2 (viewer.position.x, viewer.position.z);
		UpdateVisibleChunks ();
	}

	void UpdateVisibleChunks(){

		for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++) {
			terrainChunksVisibleLastUpdate [i].SetVisible (false);
		}
		terrainChunksVisibleLastUpdate.Clear ();

		//find coord of chunk viewer is standing on
		int currentChunkCoordX = Mathf.RoundToInt(viewerPostion.x /chunkSize);
		int currentChunkCoordY = Mathf.RoundToInt(viewerPostion.y /chunkSize);

		for (int yOffset = -chunkVisibleInView; yOffset <= chunkVisibleInView; yOffset++) {
			for (int xOffset = -chunkVisibleInView; xOffset <= chunkVisibleInView; xOffset++) {
				Vector2 viewedChunkCoord = new Vector2 (currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

				//instantiate chunk only if one doesnt exist there already
				if (terrainChunkDictionary.ContainsKey (viewedChunkCoord)) {
					terrainChunkDictionary [viewedChunkCoord].UpdateTerrainChunk ();
					if (terrainChunkDictionary [viewedChunkCoord].isVisible ()) {
						terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
					}
				} else {
					//instantiate new chunk
					terrainChunkDictionary.Add (viewedChunkCoord, new TerrainChunk (viewedChunkCoord, chunkSize, transform));
				}
			}
		}
	}
	public class TerrainChunk{
		//mesh of terrain
		GameObject meshObject;
		//cunks position
		Vector2 position;
		Bounds bounds;


		public TerrainChunk(Vector2 coord, int size, Transform parent){
			position = coord * size;
			bounds = new Bounds(position, Vector2.one * size);
			Vector3 positionV3 = new Vector3(position.x, 0, position.y);
			meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
			meshObject.transform.position = positionV3;
			meshObject.transform.localScale = Vector3.one * size / 10;
			meshObject.transform.parent = parent;
			SetVisible(false);
		}

		// update to find distance of viewer from edge of chunk, if viewrer is topo far away, disable mesh
		public void UpdateTerrainChunk(){
			float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance (viewerPostion));
			bool visible = viewerDstFromNearestEdge <= maxViewDist;
			SetVisible (visible);
		}

		public void SetVisible(bool visible){
			meshObject.SetActive (visible);
		}
		public bool isVisible(){
			return meshObject.activeSelf;
		}
	}

}