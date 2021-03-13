using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System;

public class MapGenerator : MonoBehaviour {
	private int debugcounter=0;
	private const int centerX = 27;
	private const int centerY = 8;
	private int areaCounter = 10;
	public int width;
	public int height;
	private int halfWidth;
	private int halfHeight;
	private	int x=0, y=0, z=0;
	public int iteractions;
	public string seed;
	public bool useRandomSeed;
	public Tile ground;
	public Tile wall;
	public Tilemap tileMap;

	[Range(0,100)]
	public int randomFillPercent;

	int[,] map;
	int[,] halfMap;
	int[,] halfMap2;
	void Start() {
		GenerateMap();
	}

	void DrawGround() {
		for (int x = -25; x < 10; x ++) {
			for (int y = -7; y < 23; y ++) {
				tileMap.SetTile(new Vector3Int(x, y, 0), ground);
			}
		}
	}

	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			GenerateMap();
		}

		if(Input.GetKeyDown(KeyCode.Space)){
			DrawGround();
			Debug.Log("asdadasd");
			x+=1;
			y+=1;
			z+=1;
		}
	}

	void GenerateMap() {
		map = new int[width,height];
		halfHeight = (int)(height/2);
		halfWidth = (int)(width/2);
		Debug.Log(halfHeight);
		Debug.Log(halfWidth);
		halfMap = new int[halfWidth,halfHeight];
		halfMap2 = new int[halfWidth,halfHeight];
		RandomFillMap();
		for (int i = 0; i < iteractions; i ++) {
			SmoothMap();
			halfMap = (int[,])halfMap2.Clone();
		}
		doubleSize();
		findWallsAndAreas(); // pegar coordenada de todos os pontos em uma area, pegar quais são as paredes
		//connectAllAreas(); // fazer distancia de todas as paredes entre todas as paredes até achar o mais próximo pra conectar
	}

	void doubleSize() {
		int maxWidth = width;
		int maxHeight = height;
		if(width%2 != 0){
			maxWidth--;
			for(int y = 0; y < height; y++){
				map[maxWidth, y] = 1;
			}
		}
		if(height%2 !=0 ){
			maxHeight--;
			for(int x = 0; x < width; x++){
				map[x, maxHeight] = 1;
			}
		}
		for (int x = 0; x < maxWidth; x ++) {
			for (int y = 0; y < maxHeight; y ++) {
				map[x,y] = halfMap[(int)(x/2),(int)(y/2)];
			}
		}
	}

	void connectAllAreas() {
		
	}
	
	void findWallsAndAreas() {
		areaCounter = 10;
		//this for the areas
		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				if (map[x,y] == 0){
					Debug.Log(debugcounter);
					Debug.Log(map);
					debugcounter++;
					paintBucket(x,y);
					areaCounter++;
				}
			}
		}
	}

	void paintBucket(int x, int y){
		map[x,y] = areaCounter;
		if (map[x+1,y] != 1 && map[x+1,y] != areaCounter && x+1 < width){
			paintBucket(x+1, y);
		}
		if (map[x,y+1] != 1 && map[x,y+1] != areaCounter && y+1 < height){
			paintBucket(x, y+1);
		} 
		if (map[x-1,y] != 1 && map[x-1,y] != areaCounter && x-1 > 0){
			paintBucket(x-1, y);
		}
		if (map[x,y-1] != 1 && map[x,y-1] != areaCounter && y-1 > 0){
			paintBucket(x,y-1);
		}
		return;
	}

	void RandomFillMap() {
		if (useRandomSeed) {
			seed = Time.time.ToString();
		}

		System.Random pseudoRandom = new System.Random(seed.GetHashCode());

		for (int x = 0; x < halfWidth; x ++) {
			for (int y = 0; y < halfHeight; y ++) {
				if (x == 0 || x == halfWidth-1 || y == 0 || y == halfHeight -1) {
					halfMap[x,y] = 1;
				}
				else {
					halfMap[x,y] = (pseudoRandom.Next(0,100) < randomFillPercent)? 1: 0;
				}
				halfMap2[x,y] = halfMap[x,y];
			}
		}
	}

	void SmoothMap() {
		for (int x = 0; x < halfWidth; x ++) {
			for (int y = 0; y < halfHeight; y ++) {
				int neighbourWallTiles = GetSurroundingWallCount(x,y);

				if (neighbourWallTiles > 4)
					halfMap2[x,y] = 1;
				else if (neighbourWallTiles < 4)
					halfMap2[x,y] = 0;

			}
		}
	}

	int GetSurroundingWallCount(int gridX, int gridY) {
		int wallCount = 0;
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX ++) {
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY ++) {
				if (neighbourX >= 0 && neighbourX < halfWidth && neighbourY >= 0 && neighbourY < halfHeight) {
					if (neighbourX != gridX || neighbourY != gridY) {
						wallCount += halfMap[neighbourX,neighbourY];
					}
				}
				else {
					wallCount ++;
				}
			}
		}

		return wallCount;
	}

	void OnDrawGizmos() {
		if (map != null) {
			for (int x = 0; x < width; x ++) {
				for (int y = 0; y < height; y ++) {
					Gizmos.color = getGizmoColor(x,y);
					Vector3 pos = new Vector3(-width/2 + x + centerX + .5f,-height/2 + y + centerY +.5f, 0);
					Gizmos.DrawCube(pos,Vector3.one);
				}
			}
		}
	}

/*	void OnDrawGizmos() {
		if (halfMap != null) {
			for (int x = 0; x < halfWidth; x ++) {
				for (int y = 0; y < halfHeight; y ++) {
					Gizmos.color = getGizmoColor(x,y);
					Vector3 pos = new Vector3(-halfWidth/2 + x + centerX + .5f,-halfHeight/2 + y + centerY +.5f, 0);
					Gizmos.DrawCube(pos,Vector3.one);
				}
			}
		}
	}
*/
	Color getGizmoColor(int x, int y) {
		if (map[x,y] == 1) {
			return Color.black;
		}
		if (map[x,y] == 0) {
			return Color.white;
		}
		if (map[x,y] == 10){
			return Color.red;
		}
		if (map[x,y] == 11){
			return Color.blue;
		}
		if (map[x,y] == 12){
			return Color.green;
		}
		if (map[x,y] == 13){
			return Color.cyan;
		}
		if (map[x,y] == 14){
			return Color.yellow;
		}
		return Color.magenta;
	}
}