using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiMazemanager : MonoBehaviour
{
    GameObject wallObject;
    GameObject mazeHolder;
    GameObject digger;
    Vector3 startPos;
    public float timer = 0.01f;
    float spacing = 1f;
    public int size = 21;
    public int chanceOfVisible = 50;
    GameObject[,] maze;

    Vector2Int[] diggerPos = new Vector2Int[4];
    Stack<Vector2Int>[] visitedTiles;

    public enum Direction
    {
        EAST, NORTH, SOUTH, WEST
    }

    void Start()
    {
        visitedTiles = new Stack<Vector2Int>[4];
        visitedTiles[0] = new Stack<Vector2Int>();
        visitedTiles[1] = new Stack<Vector2Int>();
        visitedTiles[2] = new Stack<Vector2Int>();
        visitedTiles[3] = new Stack<Vector2Int>();

        wallObject = GameObject.Find("BaseBlock");
        startPos = wallObject.transform.position;
        mazeHolder = GameObject.Find("MazeHolder");
        digger = GameObject.Find("Digger");
        maze = new GameObject[size, size];

        diggerPos[0] = new Vector2Int(1, 1);
        diggerPos[1] = new Vector2Int(size-2, 1);
        diggerPos[2] = new Vector2Int(1, size-2);
        diggerPos[3] = new Vector2Int(size-2, size-2);

        SpawnWalls();
        StartCoroutine(MakeMazeDepthFirst(0));
        StartCoroutine(MakeMazeDepthFirst(1));
        StartCoroutine(MakeMazeDepthFirst(2));
        StartCoroutine(MakeMazeDepthFirst(3));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopAllCoroutines();
            visitedTiles = new Stack<Vector2Int>[4];
            visitedTiles[0] = new Stack<Vector2Int>();
            visitedTiles[1] = new Stack<Vector2Int>();
            visitedTiles[2] = new Stack<Vector2Int>();
            visitedTiles[3] = new Stack<Vector2Int>();

            MazeReset();

            diggerPos[0] = new Vector2Int(1, 1);
            diggerPos[1] = new Vector2Int(size - 2, 1);
            diggerPos[2] = new Vector2Int(1, size - 2);
            diggerPos[3] = new Vector2Int(size - 2, size - 2);

            StartCoroutine(MakeMazeDepthFirst(0));
            StartCoroutine(MakeMazeDepthFirst(1));
            StartCoroutine(MakeMazeDepthFirst(2));
            StartCoroutine(MakeMazeDepthFirst(3));
        }

    }

    IEnumerator MakeMazeDepthFirst(int number)
    {
        visitedTiles[number].Push(diggerPos[number]);
        SetTile(diggerPos[number], false);

        Direction walkDirection = Direction.NORTH;
        while (true)
        {
            List<Direction> availableDirectons = PossibleDirections(number);
            if (availableDirectons.Count != 0)
            {
                walkDirection = RandomDirection(availableDirectons);
                TakeRandomStep(walkDirection, number);
                SetTile(diggerPos[number], false);
                TakeRandomStep(walkDirection, number);
                SetTile(diggerPos[number], false);
                MoveDigger(number);
                visitedTiles[number].Push(diggerPos[number]);
            }
            else
            {
                if (visitedTiles[number].Count != 0)
                {
                    diggerPos[number] = visitedTiles[number].Pop();
                    MoveDigger(number);
                }
                else
                {
                    break;
                }
            }

            yield return new WaitForSeconds(timer);
        }
    }

    List<Direction> PossibleDirections(int number)
    {
        List<Direction> directions = new List<Direction>();

        if (diggerPos[number].y < size - 2 && maze[diggerPos[number].x, diggerPos[number].y + 2].activeSelf)
            directions.Add(Direction.NORTH);
        if (diggerPos[number].x < size - 2 && maze[diggerPos[number].x + 2, diggerPos[number].y].activeSelf)
            directions.Add(Direction.EAST);
        if (diggerPos[number].y > 1 && maze[diggerPos[number].x, diggerPos[number].y - 2].activeSelf)
            directions.Add(Direction.SOUTH);
        if (diggerPos[number].x > 1 && maze[diggerPos[number].x - 2, diggerPos[number].y].activeSelf)
            directions.Add(Direction.WEST);

        return directions;
    }

    void TakeRandomStep(Direction direction, int number)
    {
        switch (direction)
        {
            case Direction.NORTH:
                if (diggerPos[number].y < size - 2 && maze[diggerPos[number].x, diggerPos[number].y + 2].activeSelf)
                    diggerPos[number] += new Vector2Int(0, 1);
                break;
            case Direction.EAST:
                if (diggerPos[number].x < size - 2 && maze[diggerPos[number].x + 2, diggerPos[number].y].activeSelf)
                    diggerPos[number] += new Vector2Int(1, 0);
                break;
            case Direction.SOUTH:
                if (diggerPos[number].y > 1 && maze[diggerPos[number].x, diggerPos[number].y - 2].activeSelf)
                    diggerPos[number] += new Vector2Int(0, -1);
                break;
            case Direction.WEST:
                if (diggerPos[number].x > 1 && maze[diggerPos[number].x - 2, diggerPos[number].y].activeSelf)
                    diggerPos[number] += new Vector2Int(-1, 0);
                break;
            default:
                //error
                break;
        }
    }

    // ============================================================= base maze utility ============================================
    void SpawnWalls()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                maze[x, y] = Instantiate(wallObject, startPos + new Vector3(x * spacing, y * spacing), Quaternion.identity, mazeHolder.transform);

            }
        }
    }

    void SetTile(int x, int y, bool on)
    {
        maze[x, y].SetActive(on);
    }

    void SetTile(Vector2Int pos, bool on)
    {
        maze[pos.x, pos.y].SetActive(on);
    }

    void MazeReset()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                SetTile(x, y, true);
            }
        }
    }

    Direction RandomDirection()
    {
        switch (Random.Range(0, 4))
        {
            case 0:
                return Direction.EAST;
            case 1:
                return Direction.NORTH;
            case 2:
                return Direction.SOUTH;
            default:
                return Direction.WEST;
        }
    }

    Direction RandomDirection(List<Direction> directions)
    {
        return directions[Random.Range(0, directions.Count)];
    }

    void MoveDigger(int number)
    {
        digger.transform.position = (Vector2)diggerPos[number] * spacing;
    }
}
