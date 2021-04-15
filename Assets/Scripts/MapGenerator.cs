using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using System;

public class MapGenerator : MonoBehaviour {
	private int debugcounter=0;
	private const int centerX = 0;
	private const int centerY = 0;
	private int areaCounter = 14;
	private int roomIndex;
	private bool reached;
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
	void DrawWall() {
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
		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				tileMapGround.SetTile(new Vector3Int(x, y, 0), ground);
			}
		}
	}
	
	Vector3Int MapToWorldCoord(int x, int y){
		return new Vector3Int(x, y, 0);
	}

	void GenerateMap() {
		List<Room> roomList = new List<Room> ();
		List<List<Room>> listOfRoomLists = new List<List<Room>>();

		bool isPossible;
		bool allConnected = false;
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

		listOfRoomLists.Add(roomList); //first roomlist in the list is the one containing all rooms
		bool firstTime = true;
		while(allConnected == false){
			ConnectClosestRooms(listOfRoomLists, firstTime);
			firstTime = false;
			allConnected = CheckIfAllConnected(listOfRoomLists);
		}
		ClearInconsistencies(roomList); // CLEAR THE INCONSISTENCIES
		ClearInconsistencies(passageList); 
		ClearRoomNumbers(roomList); // ERASE THE COLORS
		map2 = new int[width,height];
		//map2 = (int[,])map.Clone();
		PlaceTiles();
		//SetWallNumbers(roomList);
		//SetWallNumbers(passageList);
		//SetCornerNumbers(roomList);
	}

	void PlaceTiles() {
		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				if (Rule0(x,y)){
					continue;
				}
				if (Rule1(x,y)){
					continue;
				}
				if (Rule2(x,y)){
					continue;
				}
				if (Rule3(x,y)){
					continue;
				}
				if (Rule4(x,y)){
					continue;
				}
				if (Rule5(x,y)){
					continue;
				}
				if (Rule6(x,y)){
					continue;
				}
				if (Rule7(x,y)){
					continue;
				}
				if (Rule8(x,y)){
					continue;
				}
				if (Rule9(x,y)){
					continue;
				}
				if (Rule10(x,y)){
					continue;
				}
				if (Rule11(x,y)){
					continue;
				}
				if (Rule12(x,y)){
					continue;
				}
				if (Rule13(x,y)){
					continue;
				}
			}
		}
	}

	bool isWall(int x, int y){
		if(x<0 || x>= width){
			return true;
		}
		if(y<0 || y>= height){
			return true;
		}
		if(map[x,y]==1){
			return true;
		}
		return false;
	}

	bool Rule0(int x, int y){
		if(map[x,y]==0){
			map2[x,y]=0;
			return true;
		}
		return false;
	}
	
	bool Rule1(int x, int y){
		if(isWall(x-1,y+1)){
			if(isWall(x,y+1)){
				if(isWall(x+1,y+1)){
					if(isWall(x-1,y)){
						if(isWall(x,y)){
							if(isWall(x+1,y)){
								if(isWall(x-1,y-1)){
									if(isWall(x,y-1)){
										if(isWall(x+1,y-1)){
											map2[x,y]=1;
											return true;
										}
									}
								}
							}
						}
					}
				}
			}
		}
		return false;
	}

	bool Rule2(int x, int y){
		if(!isWall(x,y+1)){
			if(!isWall(x-1,y)){
				if(isWall(x,y)){
					if(isWall(x+1,y)){
						if(isWall(x,y-1)){
							if(isWall(x+1,y-1)){
								map2[x,y]=2;
								return true;
							}
						}
					}
				}
			}
		}
		return false;
	}

	bool Rule3(int x, int y){
		if(!isWall(x,y+1)){
			if(isWall(x-1,y)){
				if(isWall(x,y)){
					if(isWall(x+1,y)){
						if(isWall(x,y-1)){
							map2[x,y]=3;
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	bool Rule4(int x, int y){
		if(!isWall(x,y+1)){
			if(isWall(x-1,y)){
				if(isWall(x,y)){
					if(!isWall(x+1,y)){
						if(isWall(x-1,y-1)){
							if(isWall(x,y-1)){
								map2[x,y]=4;
								return true;
							}
						}
					}
				}
			}
		}
		return false;
	}

	bool Rule5(int x, int y){
		if(isWall(x,y+1)){
			if(!isWall(x-1,y)){
				if(isWall(x,y)){
					if(isWall(x+1,y)){
						if(isWall(x,y-1)){
							map2[x,y]=5;
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	bool Rule6(int x, int y){
		if(isWall(x,y+1)){
			if(isWall(x-1,y)){
				if(isWall(x,y)){
					if(!isWall(x+1,y)){
						if(isWall(x,y-1)){
							map2[x,y]=6;
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	bool Rule7(int x, int y){
		if(isWall(x,y+1)){
			if(isWall(x+1,y+1)){
				if(!isWall(x-1,y)){
					if(isWall(x,y)){
						if(isWall(x+1,y)){
							if(!isWall(x,y-1)){
								map2[x,y]=7;
								return true;
							}
						}
					}
				}
			}
		}
		return false;
	}

	bool Rule8(int x, int y){
		if(isWall(x,y+1)){
			if(isWall(x-1,y)){
				if(isWall(x,y)){
					if(isWall(x+1,y)){
						if(!isWall(x,y-1)){
							map2[x,y]=8;
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	bool Rule9(int x, int y){
		if(isWall(x-1,y+1)){
			if(isWall(x,y+1)){
				if(isWall(x-1,y)){
					if(isWall(x,y)){
						if(!isWall(x+1,y)){
							if(!isWall(x,y-1)){
								map2[x,y]=9;
								return true;
							}
						}
					}
				}
			}
		}
		return false;
	}

	bool Rule10(int x, int y){
		if(isWall(x,y)){
			if(isWall(x+1,y)){
				if(isWall(x,y-1)){
					if(!isWall(x+1,y-1)){
						map2[x,y]=10;
						return true;
					}
				}
			}
		}
		return false;
	}

	bool Rule11(int x, int y){
		if(isWall(x-1,y)){
			if(isWall(x,y)){
				if(!isWall(x-1,y-1)){
					if(isWall(x,y-1)){
						map2[x,y]=11;
						return true;
					}
				}
			}
		}
		return false;
	}

	bool Rule12(int x, int y){
		if(isWall(x,y+1)){
			if(!isWall(x+1,y+1)){
				if(isWall(x,y)){
					if(isWall(x+1,y)){
						map2[x,y]=12;
						return true;
					}
				}
			}
		}
		return false;
	}

	bool Rule13(int x, int y){
		if(!isWall(x-1,y+1)){
			if(isWall(x,y+1)){
				if(isWall(x-1,y)){
					if(isWall(x,y)){
						map2[x,y]=13;
						return true;
					}
				}
			}
		}
		return false;
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

	bool CheckIfAllConnected(List<List<Room>> listOfRoomLists){
		roomIndex = -1;
		foreach(Room room in listOfRoomLists[0]){
			DeepSearch(room, -1);
		}
		for(int i=0;i<=roomIndex;i++){
			List<Room> tempList = new List<Room>();
			foreach(Room room in listOfRoomLists[0]){
				if(room.getRoomIndex()==i){
					tempList.Add(room);
					room.setRoomIndex(-1);
					room.setVisited(false);
				}
			}
			if(tempList.Count>0){
				listOfRoomLists.Add(tempList);
			}
		}
		if(listOfRoomLists.Count == 1 || (listOfRoomLists[0].Count == listOfRoomLists[1].Count)){
			return true;
		}
		return false;
	}

	void DeepSearch(Room room, int firstTime){
		if (room.getVisited()==false){
			if(firstTime == -1){
				roomIndex++;
				firstTime++;
			}
			room.setRoomIndex(roomIndex);
			room.setVisited(true);
			foreach(Room connectedRoom in room.getConnectedRooms()){
				DeepSearch(connectedRoom, firstTime);
			}
		}
		return;
	}

	void isReachable(Room roomA, Room roomB) { 
		if(roomA.getVisited() == false){
			if(roomA == roomB){
				Debug.Log("locurada");
				reached = true;
			}
			roomA.setVisited(true);
			foreach(Room connectedRoom in roomA.getConnectedRooms()){
				isReachable(connectedRoom, roomB);
			}
		}
	}
	void ConnectClosestRooms(List<List<Room>> listOfRoomLists, bool firstTime) {
		
				Debug.Log("tamo ae");

		int bestDistance = 0;
		Coord bestTileA = new Coord ();
		Coord bestTileB = new Coord ();
		Room bestRoomA = new Room ();
		Room bestRoomB = new Room ();
		bool possibleConnectionFound = false;
		int i = firstTime?0:1;
		for(; i<listOfRoomLists.Count-1; i++) {
			List<Room> roomListA = listOfRoomLists[i];
			List<Room> roomListB = new List<Room>();
			if(i==0){
				roomListB = roomListA;
			}else{
				roomListB = listOfRoomLists[i+1];
			}
			foreach (Room roomA in roomListA) {
				possibleConnectionFound = false;

				foreach (Room roomB in roomListB) {
					if (roomA == roomB) {
						continue;
					}
					reached = false;
					isReachable(roomA, roomB);
					foreach(Room room in listOfRoomLists[0]){
						room.setVisited(false);
					}
					if (reached) {
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

		listOfRoomLists.RemoveRange(1,listOfRoomLists.Count-1);
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
		if(map[x,y]==1) {
			return Color.black;
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
	
	class Room : IComparable<Room> {
		private List<Coord> tiles;
		private List<Coord> edgeTiles;
		private List<Coord> cornerTiles;
		private List<Room> connectedRooms;
		private int roomSize;
		private bool visited;
		private int roomIndex;
		private int roomNumber;
		public Room() {
		}

		public Room(List<Coord> roomTiles, int[,] map, int roomNumber) {
			this.roomNumber = roomNumber;
			this.roomIndex = -1;
			tiles = roomTiles;
			roomSize = tiles.Count;
			connectedRooms = new List<Room>();
			this.visited = false;
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

		public int CompareTo(Room otherRoom) {
			return otherRoom.roomSize.CompareTo (roomSize);
		}
		public void setVisited(bool visited){ 
			this.visited = visited;
		}
		public bool getVisited(){ 
			return visited;
		}
		public void setRoomIndex(int roomIndex){ 
			this.roomIndex = roomIndex;
		}
		public int getRoomIndex(){ 
			return roomIndex;
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