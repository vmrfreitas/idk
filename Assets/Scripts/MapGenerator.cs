using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using System;

public class MapGenerator : MonoBehaviour {
	private int debugcounter=0;
	private const int centerX = 27;
	private const int centerY = 8;
	private int areaCounter = 10;
	public int width;
	public int height;
	private	int x=0, y=0, z=0;
	public int startX1;
	public int startY1;
	public int startX2;
	public int startY2;

	public int iteractions;
	public float seed;
	public bool useRandomSeed;
	public Tile ground;
	public Tile wall;
	public Tilemap tileMap;

	public List<Vector3> worldCoordTileA, worldCoordTileB; 

	[Range(0,100)]
	public int randomFillPercent;

	int[,] map;
	int[,] map2;
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
			worldCoordTileA.Clear();
			worldCoordTileB.Clear();
			GenerateMap();
		}
	}

	void GenerateMap() {
		List<Room> roomList = new List<Room> ();
		bool isPossible;
		map = new int[width,height];
		map2 = new int[width,height];
		int count = 0;
		do {
			roomList.Clear();
			isPossible = true;
			RandomFillMap(count);
			count++;
			for (int i = 0; i < iteractions; i ++) {
				SmoothMap();
				map = (int[,])map2.Clone();
			}
			roomList = findWallsAndAreas(); // pegar coordenada de todos os pontos em uma area, pegar quais são as paredes
			List<Coord> startRoomTiles = new List<Coord> ();
			map[startX1,startY1] = 0;
			map[startX2,startY2] = 0;
			startRoomTiles.Add(new Coord(startX1, startY1));
			startRoomTiles.Add(new Coord(startX2, startY2));
			roomList.Add(new Room(startRoomTiles, map, 0));
			foreach(Room room in roomList){
				if(!room.isPossible(map, height, width)){
					isPossible = false;
					break;
				}
				
			}
		} while(!isPossible);
		ConnectClosestRooms(roomList); // fazer distancia de todas as paredes entre todas as paredes até achar o mais próximo pra conectar
	}

	void ConnectClosestRooms(List<Room> allRooms) {

		int bestDistance = 0;
		Coord bestTileA = new Coord ();
		Coord bestTileB = new Coord ();
		Room bestRoomA = new Room ();
		Room bestRoomB = new Room ();
		bool possibleConnectionFound = false;

		foreach (Room roomA in allRooms) {
			possibleConnectionFound = false;

			foreach (Room roomB in allRooms) {
				if (roomA == roomB) {
					continue;
				}
				if (roomA.IsConnected(roomB)) {
					possibleConnectionFound = false;
					break;
				}

				for (int tileIndexA = 0; tileIndexA < roomA.getEdgeTiles().Count; tileIndexA ++) {
					for (int tileIndexB = 0; tileIndexB < roomB.getEdgeTiles().Count; tileIndexB ++) {
						Coord tileA = roomA.getEdgeTiles()[tileIndexA];
						Coord tileB = roomB.getEdgeTiles()[tileIndexB];
						int distanceBetweenRooms = (int)(Mathf.Pow (tileA.tileX-tileB.tileX,2) + Mathf.Pow (tileA.tileY-tileB.tileY,2));

						if (distanceBetweenRooms < bestDistance || !possibleConnectionFound) {
							bestDistance = distanceBetweenRooms;
							possibleConnectionFound = true;
							bestTileA = tileA;
							bestTileB = tileB;
							bestRoomA = roomA;
							bestRoomB = roomB;
						}
					}
				}
			}

			if (possibleConnectionFound) {
				CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
			}
		}
	}

	void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB) {
		Room.ConnectRooms (roomA, roomB);
		DrawLine(tileA.tileX, tileA.tileY, tileB.tileX, tileB.tileY);
		worldCoordTileA.Add(CoordToWorldPoint (tileA));
		worldCoordTileB.Add(CoordToWorldPoint (tileB));
	}
	
	Vector3 CoordToWorldPoint(Coord tile) {
		return new Vector3 (-width / 2 + .5f + tile.tileX + centerX, -height / 2 + .5f + tile.tileY +  centerY, 0);
	}

	List<Room> findWallsAndAreas() {
		List<Room> roomList = new List<Room> ();
		areaCounter = 10;
		//this for the areas
		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				if (map[x,y] == 0){
					//Debug.Log(debugcounter);
					//Debug.Log(map);
					//debugcounter++;
					List<Coord> coordList = new List<Coord> ();
					paintBucket(x,y, coordList);
					roomList.Add(new  Room(coordList, map, areaCounter));
					areaCounter++;
				}
			}
		}

		return roomList;
	}

	void paintBucket(int x, int y, List<Coord> coordList){
		map[x,y] = areaCounter;
		coordList.Add(new Coord(x,y));
		if (map[x+1,y] != 1 && map[x+1,y] != areaCounter && x+1 < width){
			paintBucket(x+1, y, coordList);
		}
		if (map[x,y+1] != 1 && map[x,y+1] != areaCounter && y+1 < height){
			paintBucket(x, y+1, coordList);
		} 
		if (map[x-1,y] != 1 && map[x-1,y] != areaCounter && x-1 > 0){
			paintBucket(x-1, y, coordList);
		}
		if (map[x,y-1] != 1 && map[x,y-1] != areaCounter && y-1 > 0){
			paintBucket(x,y-1, coordList);
		}
		return;
	}

	void RandomFillMap(int count) {
		if (useRandomSeed) {
			seed = Time.time;
			seed += count;
		}

		System.Random pseudoRandom = new System.Random(seed.GetHashCode());

		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				if (x == 0 || x == width-1 || y == 0 || y == height -1) {
					map[x,y] = 1;
				}
				else {
					map[x,y] = (pseudoRandom.Next(0,100) < randomFillPercent)? 1: 0;
				}
				map2[x,y] = map[x,y];
			}
		}
	}

	void SmoothMap() {
		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				int neighbourWallTiles = GetSurroundingWallCount(x,y);

				if (neighbourWallTiles > 4)
					map2[x,y] = 1;
				else if (neighbourWallTiles < 4)
					map2[x,y] = 0;

			}
		}
	}

	int GetSurroundingWallCount(int gridX, int gridY) {
		int wallCount = 0;
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX ++) {
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY ++) {
				if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
					if (neighbourX != gridX || neighbourY != gridY) {
						wallCount += map[neighbourX,neighbourY];
					}
				}
				else {
					wallCount ++;
				}
			}
		}

		return wallCount;
	}

	void DrawLine(int x0, int y0, int x1, int y1)
    {
        int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = (dx > dy ? dx : -dy) / 2, e2;
        for(;;) {
            map[x0,y0]=0;
            if (x0 == x1 && y0 == y1) break;
            e2 = err;
            if (e2 > -dx) { err -= dy; x0 += sx; }
            if (e2 < dy) { err += dx; y0 += sy; }
        }
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
		int i = 0;
		Gizmos.color = Color.green;
		foreach (Vector3 coordA in worldCoordTileA){
			Gizmos.DrawLine (coordA, worldCoordTileB[i]);
			i++;
		}
	}

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

	struct Coord {
		public int tileX;
		public int tileY;

		public Coord(int x, int y) {
			tileX = x;
			tileY = y;
		}
	}
	
	class Room {
		private List<Coord> tiles;
		private List<Coord> edgeTiles;
		private List<Room> connectedRooms;
		private int roomSize;
		private int roomNumber;

		public Room() {
		}

		public Room(List<Coord> roomTiles, int[,] map, int roomNumber) {
			this.roomNumber = roomNumber;
			tiles = roomTiles;
			roomSize = tiles.Count;
			connectedRooms = new List<Room>();

			edgeTiles = new List<Coord>();
			foreach (Coord tile in tiles) {
				for (int x = tile.tileX-1; x <= tile.tileX+1; x++) {
					for (int y = tile.tileY-1; y <= tile.tileY+1; y++) {
						if (x>-1 && y>-1){
							if ((x == tile.tileX || y == tile.tileY) && map[x,y] == 1) {
								edgeTiles.Add(new Coord (x, y));
							}
						}
					}
				}
			}
		}

		public bool isPossible(int[,] map, int height, int width) {
			foreach (Coord tile in edgeTiles){
				for (int x = tile.tileX-1; x <= tile.tileX+1; x++) {
					for (int y = tile.tileY-1; y <= tile.tileY+1; y++) {
						if(x < width && x >= 0 && y < height && y >= 0){
							if(map[x,y] != roomNumber && map[x,y] != 1){
								return false;
							}
						}
					}
				}
			}
			return true;
		}

		public static void ConnectRooms(Room roomA, Room roomB) {
			roomA.connectedRooms.Add (roomB);
			roomB.connectedRooms.Add (roomA);
		}

		public bool IsConnected(Room otherRoom) {
			return connectedRooms.Contains(otherRoom);
		}

		public List<Coord> getTiles(){ 
			return tiles;
		}

		public List<Coord> getEdgeTiles(){
			return edgeTiles;
		} 

		public List<Room> getConnectedRooms(){ 
			return connectedRooms;
		}

		public void setConnectedRooms(List<Room> connectedRooms){
			this. connectedRooms = connectedRooms; 
		}

		public int getRoomSize(){ 
			return roomSize;
		}

		public int getRoomNumber(){ 
			return roomNumber;
		}
	}
}