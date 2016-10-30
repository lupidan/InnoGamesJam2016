using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MovementManager
{
	public MovementManager ()
	{
	}

	public List<Position> getReachablePositions(int mapWidth, int mapHeight, List<TileController> allTiles, Unit unit) {
		int[,] costsMatrix = transformToCostsMatrix (mapWidth, mapHeight, allTiles);
		int[,] realCostsMatrix = calculateRealCostsMatrix (costsMatrix, unit.position, unit.Definition.maxMovements);
		return toListOfPositions (realCostsMatrix, unit.Definition.maxMovements);
	}
	/// <summary>
	/// Transforms the real costs matrix to a list positions. 
	/// Returns positions whose real costs are smaller than the max costs, if max costs is greater than 0.
	/// Returns all reachable positions regardless of costs, if maxCosts is 0 or less.
	/// </summary>
	/// <returns>The list of positions.</returns>
	/// <param name="costMatrix">Cost matrix.</param>
	/// <param name="maxCosts">Max costs. Will be ignored if 0 or less</param>
	public List<Position> toListOfPositions(int[,] costMatrix, int maxCosts) {
		List<Position> positions = new List<Position> ();
		for (int x = 0; x < costMatrix.GetLength(0); x++) {
			for (int y = 0; y < costMatrix.GetLength(1); y++) {
				int costs = costMatrix[x,y];
				if (costs > 0 && (maxCosts <= 0 || costs <= maxCosts)) {
					positions.Add (new Position (x,y));
				}
			}
		}
		return positions;
	}

	/// <summary>
	/// Calculates the real cost matrix relative to the startPosition.
	/// Will only calculate real costs that are equal or smaller than maxCosts, if maxCosts is greater than 0.
	/// Will calculate real costs of all reachable positions of maxCosts is 0 or less.
	/// </summary>
	/// <returns>The real cost matrix.</returns>
	/// <param name="costMatrix">Cost matrix.</param>
	/// <param name="startPosition">Start position.</param>
	/// <param name="maxCosts">Max costs. Will be ignored if 0 or less</param>
	public int[,] calculateRealCostsMatrix(int[,] costsMatrix, Position startPosition, int maxCosts) {
		int width = costsMatrix.GetLength(0);
		int height = costsMatrix.GetLength(1);
		List<Position> activeTilesList = new List<Position>();
		int[,] realCostsMatrix = new int[width,height];

		//init
		activeTilesList.Add(startPosition);
		initializeIntMatrix (realCostsMatrix, -1);
		realCostsMatrix [startPosition.x, startPosition.y] = 0;

		while (activeTilesList.Count > 0) {
			HashSet<Position> nextIterationActiveTiles = new HashSet<Position>();
			//sort active tiles by cost ascending
			List<Position> sortedActiveTilesList = activeTilesList.OrderBy(pos => realCostsMatrix[pos.x,pos.y]).ToList();
			//loop over sorted tiles
			foreach (Position activeTile in sortedActiveTilesList) {
				int x = activeTile.x;
				int y = activeTile.y;
				int startCosts = realCostsMatrix [x,y];
				//calculate adjacent real costs
				List<Position> adjacentPositions = new List<Position>();
				//adjacentPositions.Add(new Position(x - 1, y + 1)); // top left
				adjacentPositions.Add(new Position(x, y + 1)); // top center
				//adjacentPositions.Add(new Position(x + 1, y + 1)); // top right
				adjacentPositions.Add(new Position(x - 1, y)); // left
				adjacentPositions.Add(new Position(x + 1, y)); // right
				//adjacentPositions.Add(new Position(x - 1, y - 1)); // bottom left
				adjacentPositions.Add(new Position(x, y - 1)); // bottom center
				//adjacentPositions.Add(new Position(x + 1, y - 1)); // bottom right
				foreach (Position position in adjacentPositions) {
					if (isPositionValid(position, width, height)) {
						//process tile only if not already processed
						if (realCostsMatrix[position.x,position.y] < 0) {
							//some unreachable tiles will be processed multiple times but that should be ok, no need to overoptimize here
							int costs = costsMatrix [position.x,position.y];
							// check if tile is walkable
							if (costs > 0) {
								int realCosts = startCosts + costs;
								//add tile to next active tiles only if movements left
								if (maxCosts <= 0 || realCosts <= maxCosts) {
									realCostsMatrix [position.x,position.y] = realCosts;
									if (maxCosts <= 0 || realCosts < maxCosts) {
										nextIterationActiveTiles.Add (position);
									}
								}								
							}	
						}
					}
				}				
			}
			//create list of all directly reachable tiles from active tiles
			//mark reachable tiles active
			//mark active tiles inactive
			activeTilesList = nextIterationActiveTiles.ToList();
		}
		return realCostsMatrix;
	}

	public void initializeIntMatrix(int[,] intMatrix, int defaultValue) {
		for (int x = 0; x < intMatrix.GetLength (0); x++) {
			for (int y = 0; y < intMatrix.GetLength (1); y++) {
				intMatrix [x, y] = defaultValue;
			}
		}
	}

	public bool isPositionValid(Position pos, int width, int height) {
		return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
	}

	/// <summary>
	/// Transforms a list of TileController objects to a costs matrix of width and height.
	/// Not walkable tiles will be assigned the cost '-1'.
	/// Missing tiles will be assigned the cost '-1' (not walkable).
	/// </summary>
	/// <returns>The to costs matrix.</returns>
	/// <param name="mapWidth">Map width.</param>
	/// <param name="mapHeight">Map height.</param>
	/// <param name="allTiles">All tiles.</param>
	public int[,] transformToCostsMatrix(int mapWidth, int mapHeight, List<TileController> allTiles) {
		int[,] costsMatrix = new int[mapWidth,mapHeight];
		// ensure undefined tiles are not walkable
		initializeIntMatrix (costsMatrix, -1);
		foreach (TileController tile in allTiles) {
			int value = tile.tileData.Definition.cost;
			if (!tile.tileData.Definition.walkable) {
				value = -1;
			}
			costsMatrix [tile.tileData.position.x,tile.tileData.position.y] = value;
		}
		return costsMatrix;
	}


}

