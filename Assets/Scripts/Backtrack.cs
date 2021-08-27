using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backtrack : Maze
{

    public int countRooms, minRoomSize, maxRoomSize;

    public override void Generate()
    {
        if (clvl == 0)
            Generate(Random.Range(1, width - 1), clvl, Random.Range(1, depth - 1));
        else
        {
            Generate(levelStart.x, levelStart.y, levelStart.z);
        }
    }

    public override void AddRooms()
    {
        AddRooms(countRooms, minRoomSize, maxRoomSize);
    }

    void AddRooms(int count, int minSize, int maxSize) 
    {
        for (int c = 0; c < count; c++)
        {
            int startX = Random.Range(3, width - 3);
            int startZ = Random.Range(3, depth - 3);
            int roomWidth = Random.Range(minSize, maxSize);
            int roomDepth = Random.Range(minSize, maxSize);
            for (int x = startX; x < width - 3 && x < startX + roomWidth; x++)
            {
                for (int z = startZ; z < depth - 3 && z < startZ + roomDepth; z++)
                {
                    map[x, clvl, z] = 0;
                }
            }
        }
    }

    void Generate(int x, int y, int z)
    {
        if (CountNeighbours(x, y, z) >= 2 || map[x, y, z] == 6) return;
        map[x, y, z] = 0;

        directions.Shuffle();

        Generate(x + directions[0].x, y, z + directions[0].z);
        Generate(x + directions[1].x, y, z + directions[1].z);
        Generate(x + directions[2].x, y, z + directions[2].z);
        Generate(x + directions[3].x, y, z + directions[3].z);

        if (CountNeighbours(x, y, z) == 1) potentialStart.Add(new MapLocation(x, y, z));
    }
}