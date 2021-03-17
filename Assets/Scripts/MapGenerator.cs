using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using System;

public class MapGenerator : MonoBehaviour {
	private int debugcounter=0;
	private const int centerX = 27;
	private const int centerY = 8;
	private int areaCounter = 14;
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
	public Tile wall1;
	public Tile wall2;
	public Tile wall3;
	public Tile wall4;
	public Tile wall5;
	public Tile wall6;
	public Tile wall7;
	public Tile wall8;
	public Tile wall9;
	public Tile wall10;
	public Tile wall11;
	public Tile wall12;
	public Tile wall13;
	public Tilemap tileMapGround;
	public Tilemap tileMapWall;
	private List<Room> passageList;
	public List<Vector3> worldCoordTileA, worldCoordTileB; 

	[Range(0,100)]
	public int randomFillPercent;

	int[,] map;
	int[,] map2;
	void Start() {
		passageList = new List<Room> ();
		GenerateMap();
		DrawGround();
		DrawWall();
	}
	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			worldCoordTileA.Clear();
			worldCoordTileB.Clear();
			passageList.Clear();
			GenerateMap();
			tileMapWall.ClearAllTiles();
			DrawWall();
		}
	}
	void DrawWall() {//todo
		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				switch (map2[x,y]){
					case 1:
						tileMapWall.SetTile(MapToWorldCoord(x,y), wall1);
						break;
					case 2:
						tileMapWall.SetTile(MapToWorldCoord(x,y), wall2);
						break;
					case 3:
						tileMapWall.SetTile(MapToWorldCoord(x,y), wall3);
						break;
					case 4:
						tileMapWall.SetTile(MapToWorldCoord(x,y), wall4);
						break;
					case 5:
						tileMapWall.SetTile(MapToWorldCoord(x,y), wall5);
						break;
					case 6:
						tileMapWall.SetTile(MapToWorldCoord(x,y), wall6);
						break;
					case 7:
						tileMapWall.SetTile(MapToWorldCoord(x,y), wall7);
						break;
					case 8:
						tileMapWall.SetTile(MapToWorldCoord(x,y), wall8);
						break;
					case 9:
						tileMapWall.SetTile(MapToWorldCoord(x,y), wall9);
						break;
					case 10:
						tileMapWall.SetTile(MapToWorldCoord(x,y), wall10);
						break;
					case 11:
						tileMapWall.SetTile(MapToWorldCoord(x,y), wall11);
						break;
					case 12:
						tileMapWall.SetTile(MapToWorldCoord(x,y), wall12);
						break;
					case 13:
						tileMapWall.SetTile(MapToWorldCoord(x,y), wall13);
						break;
				}
			}
		}
	}
	void DrawGround() {
		for (int x = -25; x < 10; x ++) {
			for (int y = -7; y < 23; y ++) {
				tileMapGround.SetTile(new Vector3Int(x, y, 0), ground);
			}
		}
	}
	
	Vector3Int MapToWorldCoord(int x, int y){
		return new Vector3Int(x-25, y-7, 0);
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
			RandomFillMap(count); // STEP TO RANDOM FILL THE MAP
			count++;
			for (int i = 0; i < iteractions; i ++) { // CELLULAR AUTOMATA
				SmoothMap(); 
				map = (int[,])map2.Clone();
			}
			roomList = findWallsAndAreas(); // GET EVERY AREA
			SetStartingRoom(roomList); // SET THE STARTING ROOM
			foreach(Room room in roomList){ // CHECK IF THE MAP IS POSSIBLE
				if(!room.isPossible(map, height, width)){
					isPossible = false;
					break;
				}
			}
		} while(!isPossible);
		
		roomList.Sort ();
		roomList [0].isMainRoom = true;
		roomList [0].isAccessibleFromMainRoom = true;
		ConnectClosestRooms(roomList); // CONNECT EVERY ROOM
		ClearInconsistencies(roomList); // CLEAR THE INCONSISTENCIES
		ClearInconsistencies(passageList); 
		ClearRoomNumbers(roomList); // ERASE THE COLORS
		map2 = (int[,])map.Clone();
		SetWallNumbers(roomList);
		SetWallNumbers(passageList);
		SetCornerNumbers(roomList);
	}
	void SetCornerNumbers(List<Room> roomList){
		foreach (Room room in roomList){
			foreach (Coord tile in room.getCornerTiles()){
				if(tile.tileX-1 < 0){
					if(map2[tile.tileX+1,tile.tileY]==8){
						map2[tile.tileX,tile.tileY]=10;
						continue;
					} else {
						map2[tile.tileX,tile.tileY]=12;
						continue;
					}
				}
				if(tile.tileX+1 == width){ // direita é "parede"
					if(map2[tile.tileX-1,tile.tileY]==8){ //8 na esquerda
						map2[tile.tileX,tile.tileY]=11;
						continue;
					} else { //n tem 8 na esquerda
						map2[tile.tileX,tile.tileY]=13;
						continue;
					}
				}
				if (map2[tile.tileX+1,tile.tileY]==8){ //8 na direita
					map2[tile.tileX,tile.tileY]=10;
					continue;
				} else {
					if(map2[tile.tileX-1,tile.tileY]==8){ //8 na esquerda
						map2[tile.tileX,tile.tileY]=11;
						continue;
					}
				}
				if (map2[tile.tileX+1,tile.tileY]==3){ //3 na direita
					map2[tile.tileX,tile.tileY]=12;
					continue;
				} else {
					if(map2[tile.tileX-1,tile.tileY]==3){ //3 na esquerda
						map2[tile.tileX,tile.tileY]=13;
						continue;
					}
				}
			}
		}
	}

	void SetWallNumbers(List<Room> roomList){
		foreach (Room room in roomList){
			foreach (Coord tile in room.getEdgeTiles()){
				if(tile.tileX-1<0){ //caso seja do quarto inicial
					if(tile.tileY+1 == height){//embaixo é "parede"
						if(map[tile.tileX,tile.tileY-1]==1){ //emcima é parede
							map2[tile.tileX,tile.tileY]=6;
							continue;
						} else { //em cima não é parede
							if(tile.tileX+1 == width){//direita é "parede"
								map2[tile.tileX,tile.tileY]=3;
								continue;
							}
							if(map[tile.tileX+1,tile.tileY]==1){//direita é parede
								map2[tile.tileX,tile.tileY]=3;
								continue;
							} else { 
								map2[tile.tileX,tile.tileY]=4;
								continue;
							}
						}
					}
					if(map[tile.tileX,tile.tileY+1] == 1){ //embaixo é parede
						if(map[tile.tileX,tile.tileY-1]==1){ //emcima é parede
							map2[tile.tileX,tile.tileY]=6;
							continue;
						} else { //em cima não é parede
							if(tile.tileX+1 == width){//direita é "parede"
								map2[tile.tileX,tile.tileY]=3;
								continue;
							}
							if(map[tile.tileX+1,tile.tileY]==1){//direita é parede
								map2[tile.tileX,tile.tileY]=3;
								continue;
							} else { 
								map2[tile.tileX,tile.tileY]=4;
								continue;
							}
						}
					} else{//embaixo não é parede 
						if(tile.tileX+1 == width){ //direita é "parede"
							map2[tile.tileX,tile.tileY]=8;
							continue;
						}
						if(map[tile.tileX+1,tile.tileY]==1){ //direita é parede
							map2[tile.tileX,tile.tileY]=8;
							continue;
						} else { //direita não é parede
							map2[tile.tileX,tile.tileY]=9;
							continue;
						}
					}
				}
				if (map[tile.tileX-1,tile.tileY]==1){ //esquerda é parede
					if(tile.tileX+1 == width){ //direita é "parede"
						if(tile.tileY+1 == height){ //prabaixo é "parede"
							map2[tile.tileX,tile.tileY]=3;
							continue;
						}
						if(map[tile.tileX,tile.tileY+1]==1){ //prabaixo é parede
							map2[tile.tileX,tile.tileY]=3;
							continue;
						} else { //prabaixo não é parede
							map2[tile.tileX,tile.tileY]=8;
							continue;
						}
					}
					if (map[tile.tileX+1,tile.tileY]==1){ //direita é parede
						if(tile.tileY+1 == height){ //prabaixo é "parede"
							map2[tile.tileX,tile.tileY]=3;
							continue;
						}
						if(map[tile.tileX,tile.tileY+1]==1){ //prabaixo é parede
							map2[tile.tileX,tile.tileY]=3;
							continue;
						} else { //prabaixo não é parede
							map2[tile.tileX,tile.tileY]=8;
							continue;
						}
					} else { //direita não é parede
						if(tile.tileY+1 == height){ //prabaixo é "parede"
							if (map[tile.tileX,tile.tileY-1]==1){ //pracima é parede
								map2[tile.tileX,tile.tileY]=6;
								continue;
							} else { //pracima não é parede
								map2[tile.tileX,tile.tileY]=4;
								continue;
							}
						}
						if(map[tile.tileX,tile.tileY+1]==1){ //prabaixo é parede
							if (map[tile.tileX,tile.tileY-1]==1){ //pracima é parede
								map2[tile.tileX,tile.tileY]=6;
								continue;
							} else { //pracima não é parede
								map2[tile.tileX,tile.tileY]=4;
								continue;
							}
						} else { //prabaixo não é parede
							if (map[tile.tileX,tile.tileY-1]==1){ //pracima é parede
								map2[tile.tileX,tile.tileY]=9;
								continue;
							}
						}
					}
				} else { //direita é parede
					if(tile.tileY+1 == height){ //prabaixo é "parede"
						if(map[tile.tileX,tile.tileY-1]==1){ //pracima é parede
							map2[tile.tileX,tile.tileY]=5;
							continue;
						} else { //pracima não é parede
							map2[tile.tileX,tile.tileY]=2;
							continue;
						}
					}
					if (map[tile.tileX,tile.tileY+1]==1){ //prabaixo é parede
						if(map[tile.tileX,tile.tileY-1]==1){ //pracima é parede
							map2[tile.tileX,tile.tileY]=5;
							continue;
						} else { //pracima não é parede
							map2[tile.tileX,tile.tileY]=2;
							continue;
						}
					} else { //prabaixo não é parede
						map2[tile.tileX,tile.tileY]=7;
						continue;
					}
				}
			}
		}
	}

	void SetStartingRoom(List<Room> roomList){
		List<Coord> startRoomTiles = new List<Coord> ();
		map[startX1,startY1] = 0;
		map[startX2,startY2] = 0;
		startRoomTiles.Add(new Coord(startX1, startY1));
		startRoomTiles.Add(new Coord(startX2, startY2));
		roomList.Add(new Room(startRoomTiles, map, 0));
	}
	void ClearRoomNumbers(List<Room> roomList){
		foreach(Room room in roomList){
			foreach (Coord tile in room.getTiles()){
				map[tile.tileX,tile.tileY] = 0;
			}
		}
	}
	void ClearInconsistencies(List<Room> roomList){
		foreach (Room room in roomList){
			foreach (Coord tile in room.getEdgeTiles()){
				for (int x = tile.tileX-1; x <= tile.tileX+1; x++) {
					for (int y = tile.tileY-1; y <= tile.tileY+1; y++) {
						if (x>0 && y>0 && x<width-1 && y<height-1){
							if (x == tile.tileX || y == tile.tileY) {
								//010
								if (map[x-1,y]!=1 && map[x,y]==1 && map[x+1,y]!=1){
									map[x,y] = 0;
								}
								if (map[x,y-1]!=1 && map[x,y]==1 && map[x,y+1]!=1){
									map[x,y] = 0;
								}
							}
						}
					}
				}
			}
		}
	}

	void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false) {
		
		List<Room> roomListA = new List<Room> ();
		List<Room> roomListB = new List<Room> ();
		
		if (forceAccessibilityFromMainRoom) {
			foreach (Room room in allRooms) {
				if (room.isAccessibleFromMainRoom) {
					roomListB.Add (room);
				} else {
					roomListA.Add (room);
				}
			}
		} else {
			roomListA = allRooms;
			roomListB = allRooms;
		}
		
		int bestDistance = 0;
		Coord bestTileA = new Coord ();
		Coord bestTileB = new Coord ();
		Room bestRoomA = new Room ();
		Room bestRoomB = new Room ();
		bool possibleConnectionFound = false;
		
		foreach (Room roomA in roomListA) {
			if (!forceAccessibilityFromMainRoom) {
				possibleConnectionFound = false;
				if (roomA.getConnectedRooms().Count > 0) {
					continue;
				}
			}
			
			foreach (Room roomB in roomListB) {
				if (roomA == roomB || roomA.IsConnected(roomB)) {
					continue;
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
			if (possibleConnectionFound && !forceAccessibilityFromMainRoom) {
				CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
			}
		}
		
		if (possibleConnectionFound && forceAccessibilityFromMainRoom) {
			CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
			ConnectClosestRooms(allRooms, true);
		}
		
		if (!forceAccessibilityFromMainRoom) {
			ConnectClosestRooms(allRooms, true);
		}
	}
	
	void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB) {
		Room.ConnectRooms (roomA, roomB);
		List<Coord> passageCoords = DrawLine(tileA.tileX, tileA.tileY, tileB.tileX, tileB.tileY);
		passageList.Add(new Room(passageCoords, map, 0));
		worldCoordTileA.Add(CoordToWorldPoint (tileA));
		worldCoordTileB.Add(CoordToWorldPoint (tileB));
	}
	
	List<Coord> DrawLine(int x0, int y0, int x1, int y1)
    {
		List<Coord> coords = new List<Coord>();
        int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = (dx > dy ? dx : -dy) / 2, e2;
        for(;;) {
            map[x0,y0]=0;
			coords.Add(new Coord(x0, y0));
            if (x0 == x1 && y0 == y1) break;
            e2 = err;
            if (e2 > -dx) { err -= dy; x0 += sx; }
            if (e2 < dy) { err += dx; y0 += sy; }
        }
		return coords;
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
		if (map[x,y] == 0) {
			return Color.white;
		}
		if (map[x,y] == 14){
			return Color.red;
		}
		if (map[x,y] == 15){
			return Color.blue;
		}
		if (map[x,y] == 16){
			return Color.green;
		}
		if (map[x,y] == 17){
			return Color.cyan;
		}
		if (map[x,y] == 18){
			return Color.yellow;
		}
		if (map[x,y] > 19){
			return Color.gray;
		}
		return Color.black;
	}

	struct Coord {
		public int tileX;
		public int tileY;

		public Coord(int x, int y) {
			tileX = x;
			tileY = y;
		}
	}
	
	class Room : IComparable<Room> {
		private List<Coord> tiles;
		private List<Coord> edgeTiles;
		private List<Coord> cornerTiles;
		private List<Room> connectedRooms;
		private int roomSize;
		public bool isAccessibleFromMainRoom;
		public bool isMainRoom;
		private int roomNumber;

		public Room() {
		}

		public Room(List<Coord> roomTiles, int[,] map, int roomNumber) {
			this.roomNumber = roomNumber;
			tiles = roomTiles;
			roomSize = tiles.Count;
			connectedRooms = new List<Room>();

			edgeTiles = new List<Coord>();
			cornerTiles = new List<Coord>();
			foreach (Coord tile in tiles) {
				for (int x = tile.tileX-1; x <= tile.tileX+1; x++) {
					for (int y = tile.tileY-1; y <= tile.tileY+1; y++) {
						if (x>-1 && y>-1){
							if ((x == tile.tileX || y == tile.tileY) && map[x,y] == 1) {
								edgeTiles.Add(new Coord (x, y));
							}else if(map[x,y] == 1){
								cornerTiles.Add(new Coord (x,y));
							}
						}
					}
				}
			}
		}
		
		public void SetAccessibleFromMainRoom() {
			if (!isAccessibleFromMainRoom) {
				isAccessibleFromMainRoom = true;
				foreach (Room connectedRoom in connectedRooms) {
					connectedRoom.SetAccessibleFromMainRoom();
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
			if (roomA.isAccessibleFromMainRoom) {
				roomB.SetAccessibleFromMainRoom ();
			} else if (roomB.isAccessibleFromMainRoom) {
				roomA.SetAccessibleFromMainRoom();
			}
			roomA.connectedRooms.Add (roomB);
			roomB.connectedRooms.Add (roomA);
		}

		public bool IsConnected(Room otherRoom) {
			return connectedRooms.Contains(otherRoom);
		}

		public int CompareTo(Room otherRoom) {
			return otherRoom.roomSize.CompareTo (roomSize);
		}
		public List<Coord> getTiles(){ 
			return tiles;
		}

		public List<Coord> getEdgeTiles(){
			return edgeTiles;
		} 

		public List<Coord> getCornerTiles(){
			return cornerTiles;
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