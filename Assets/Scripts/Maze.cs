using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLocation
{
    public int x, y, z;

    public MapLocation(int _x, int _y, int _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    //public LevelLocation(int _x, int _y, int _z)
}

public class Maze : MonoBehaviour
{
    public int clvl = 0, level, width, depth, scale = 6;
    public byte[,,] map;

    public (int x, int y, int z) levelStart;

    public GameObject straight, deadEnd, stairwell, cross, t, corner, player;

    public List<MapLocation> directions = new List<MapLocation>()
    {
        new MapLocation(1,0,0),
        new MapLocation(0,0,1),
        new MapLocation(-1,0,0),
        new MapLocation(0,0,-1)
    };

    public List<MapLocation> potentialStart = new List<MapLocation>()
    {

    };
    // Start is called before the first frame update
    void Start()
    {
        map = new byte[width, level, depth];
        InitializeMap();
        for(clvl = 0; clvl < level; clvl++)
        {
            Generate();
            AddRooms();
            if (clvl == 0)
                PickStart();
            if (level > 1 && clvl < level - 1)
                PlaceStairwell();
            DrawMap(clvl);
        }
    }

    public virtual void AddRooms()
    {
        
    }

    void InitializeMap()
    {
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
                for(int y = 0; y < level; y++)
                {
                    map[x, y, z] = 1;
                }
    }

    public virtual void Generate()
    {
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                if (Random.Range(0, 100) < 50)
                    map[x, 0, z] = 0;
            }
    }

    void DrawMap(int y)
    {
        for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                Vector3 pos = new Vector3(x * scale, y * 6, z * scale);
                if (map[x, y, z] == 6)
                {

                }
                else if (map[x, y, z] == 0)
                {
                    int count = CountNeighbours(x, y, z);
                    switch (count)
                    {
                        case 1:                            //dead ends and stairwells
                            if (map[x - 1, y, z] == 0)
                            {
                                Instantiate(deadEnd, pos, Quaternion.Euler(0, 180, 0));
                            }
                            else if (map[x + 1, y, z] == 0)
                            {
                                Instantiate(deadEnd, pos, Quaternion.Euler(0, 0, 0));
                            }
                            else if (map[x, y, z + 1] == 0)
                            {
                                Instantiate(deadEnd, pos, Quaternion.Euler(0, -90, 0));
                            }
                            else if (map[x, y, z - 1] == 0)
                            {
                                Instantiate(deadEnd, pos, Quaternion.Euler(0, 90, 0));
                            }
                            //potentialStart.Add(new MapLocation(x * scale, y * 6, z * scale));
                            break;
                        case 2:                             //straights and corners
                            if (map[x + 1, y, z] != 1 && map[x - 1, y, z] != 1)   //straight
                            {
                                Instantiate(straight, pos, Quaternion.Euler(0, 0, 0));
                            }
                            else if (map[x, y, z + 1] != 1 && map[x, y, z - 1] != 1)
                            {
                                Instantiate(straight, pos, Quaternion.Euler(0, 90, 0));
                            }
                            else if (map[x, y, z + 1] != 1 && map[x - 1, y, z] != 1)  //corner
                            {
                                Instantiate(corner, pos, Quaternion.Euler(0, -90, 0));
                            }
                            else if (map[x, y, z + 1] != 1 && map[x + 1, y, z] != 1)
                            {
                                Instantiate(corner, pos, Quaternion.Euler(0, 0, 0));
                            }
                            else if (map[x, y, z - 1] != 1 && map[x + 1, y, z] != 1)
                            {
                                Instantiate(corner, pos, Quaternion.Euler(0, 90, 0));
                            }
                            else if (map[x, y, z - 1] != 1 && map[x - 1, y, z] != 1)
                            {
                                Instantiate(corner, pos, Quaternion.Euler(0, 180, 0));
                            }
                            break;
                        case 3:                            //t junctions
                            if (map[x - 1, y, z] == 1)
                            {
                                Instantiate(t, pos, Quaternion.Euler(0, 0, 0));
                            }
                            else if (map[x + 1, y, z] == 1)
                            {
                                Instantiate(t, pos, Quaternion.Euler(0, 180, 0));
                            }
                            else if (map[x, y, z + 1] == 1)
                            {
                                Instantiate(t, pos, Quaternion.Euler(0, 90, 0));
                            }
                            else if (map[x, y, z - 1] == 1)
                            {
                                Instantiate(t, pos, Quaternion.Euler(0, -90, 0));
                            }
                            break;
                        default:                               //cross junctions
                            Instantiate(cross, pos, Quaternion.Euler(0, 0, 0));
                            break;
                    }
                }
                else if (map[x, y, z] == 28)
                {
                    if (map[x - 1, y, z] == 0)
                    {
                        Instantiate(stairwell, pos, Quaternion.Euler(0, 180, 0));
                    }
                    else if (map[x + 1, y, z] == 0)
                    {
                        Instantiate(stairwell, pos, Quaternion.Euler(0, 0, 0));
                    }
                    else if (map[x, y, z + 1] == 0)
                    {
                        Instantiate(stairwell, pos, Quaternion.Euler(0, -90, 0));
                    }
                    else if (map[x, y, z - 1] == 0)
                    {
                        Instantiate(stairwell, pos, Quaternion.Euler(0, 90, 0));
                    }
                }
            }
    }

    void PickStart()
    {
        int rs = Random.Range(0, potentialStart.Count);
        Vector3 pos = new Vector3(potentialStart[rs].x * scale, 0, potentialStart[rs].z * scale);
        player.transform.position = pos;
        Debug.Log(rs + " / " + potentialStart.Count);

        if (map[potentialStart[rs].x - 1, 0, potentialStart[rs].z] == 0)
        {
            player.transform.rotation = Quaternion.Euler(0, -90, 0);
        }
        else if (map[potentialStart[rs].x + 1, 0, potentialStart[rs].z] == 0)
        {
            player.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (map[potentialStart[rs].x, 0, potentialStart[rs].z + 1] == 0)
        {
            player.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (map[potentialStart[rs].x, 0, potentialStart[rs].z - 1] == 0)
        {
            player.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        potentialStart.Remove(potentialStart[rs]);
    }

    public void PlaceStairwell()
    {
        int rs = Random.Range(0, potentialStart.Count);
        while (potentialStart[rs].x <= 1 || potentialStart[rs].x >= width - 2 || potentialStart[rs].z <= 1 || potentialStart[rs].z >= depth - 2)
        {
            rs = Random.Range(0, potentialStart.Count);
        }

        if (map[potentialStart[rs].x - 1, potentialStart[rs].y, potentialStart[rs].z] == 0)
        {
            levelStart = (potentialStart[rs].x + 1, potentialStart[rs].y + 1, potentialStart[rs].z);
        }
        else if (map[potentialStart[rs].x + 1, potentialStart[rs].y, potentialStart[rs].z] == 0)
        {
            levelStart = (potentialStart[rs].x - 1, potentialStart[rs].y + 1, potentialStart[rs].z);
        }
        else if (map[potentialStart[rs].x, potentialStart[rs].y, potentialStart[rs].z + 1] == 0)
        {
            levelStart = (potentialStart[rs].x, potentialStart[rs].y + 1, potentialStart[rs].z - 1);
        }
        else if (map[potentialStart[rs].x, potentialStart[rs].y, potentialStart[rs].z - 1] == 0)
        {
            levelStart = (potentialStart[rs].x, potentialStart[rs].y + 1, potentialStart[rs].z + 1);
        }
        else levelStart = (-10, -10, -10);
        
        map[potentialStart[rs].x, potentialStart[rs].y, potentialStart[rs].z] = 28;
        map[potentialStart[rs].x, potentialStart[rs].y + 1, potentialStart[rs].z] = 6;
        potentialStart.Clear();
    }

public int CountNeighbours(int x, int y, int z)
    {
        int count = 0;
        if (x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1) return 5;
        if (map[x - 1, y, z] != 1) count++;
        if (map[x + 1, y, z] != 1) count++;
        if (map[x, y, z - 1] != 1) count++;
        if (map[x, y, z + 1] != 1) count++;

        return count;
    }
    public int CountDiagonalNeighbours(int x, int y, int z)
    {
        int count = 0;
        if (x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1) return 5;
        if (map[x - 1, y, z + 1] == 0) count++;
        if (map[x + 1, y, z + 1] == 0) count++;
        if (map[x - 1, y, z - 1] == 0) count++;
        if (map[x + 1, y, z - 1] == 0) count++;

        return count;
    }

    public int CountAllNeighbours(int x, int y, int z)
    {
        return CountNeighbours(x, y, z) + CountDiagonalNeighbours(x, y, z);
    }
}
