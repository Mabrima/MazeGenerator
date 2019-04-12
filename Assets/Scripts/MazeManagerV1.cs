using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeManagerV1 : MonoBehaviour
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

    bool complete = false;

    Vector2Int diggerPos;
    Stack<Vector2Int> visitedTiles;

    public enum Direction
    {
        EAST, NORTH, SOUTH, WEST
    }

    void Start()
    {
        visitedTiles = new Stack<Vector2Int>();
        wallObject = GameObject.Find("BaseBlock");
        startPos = wallObject.transform.position;
        mazeHolder = GameObject.Find("MazeHolder");
        digger = GameObject.Find("Digger");
        maze = new GameObject[size, size];
        SpawnWalls();
        StartCoroutine(MakeMazeDepthFirst()); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopAllCoroutines();
            visitedTiles = new Stack<Vector2Int>();
            MazeReset();
            complete = false;
            StartCoroutine(MakeMazeDepthFirst());
        }

        if (complete)
        {
            if (Input.GetAxisRaw("Horizontal") == 1)
            {
                //move right
            }
            if (Input.GetAxisRaw("Horizontal") == -1)
            {
                //move left
            }
            if (Input.GetAxisRaw("Vertical") == 1)
            {
                //move up
            }
            if (Input.GetAxisRaw("Vertical") == -1)
            {
                //move down
            }
            
        }
    }

    IEnumerator MakeMazeDepthFirst()
    {
        diggerPos = new Vector2Int(1, 1);

        visitedTiles.Push(diggerPos);
        SetTile(diggerPos, false);

        Direction walkDirection = Direction.NORTH;
        while (true)
        {
            List<Direction> availableDirectons = PossibleDirections();
            if (availableDirectons.Count != 0)
            {
                walkDirection = RandomDirection(availableDirectons);
                TakeRandomStep(walkDirection);
                SetTile(diggerPos, false);
                TakeRandomStep(walkDirection);
                SetTile(diggerPos, false);
                MoveDigger();
                visitedTiles.Push(diggerPos);
            }
            else
            {
                if (visitedTiles.Count != 0)
                {
                    diggerPos = visitedTiles.Pop();
                    MoveDigger();
                }
                else
                {
                    complete = true;
                    break;
                }
            }

            yield return new WaitForSeconds(timer);
        }
    }

    List<Direction> PossibleDirections()
    {
        List<Direction> directions = new List<Direction>();

        if (diggerPos.y < size - 2 && maze[diggerPos.x, diggerPos.y + 2].activeSelf)
            directions.Add(Direction.NORTH);
        if (diggerPos.x < size - 2 && maze[diggerPos.x + 2, diggerPos.y].activeSelf)
            directions.Add(Direction.EAST);
        if (diggerPos.y > 1 && maze[diggerPos.x, diggerPos.y - 2].activeSelf)
            directions.Add(Direction.SOUTH);
        if (diggerPos.x > 1 && maze[diggerPos.x - 2, diggerPos.y].activeSelf)
            directions.Add(Direction.WEST);

        return directions;
    }

    // ============================================================ random digger ===================================================

    IEnumerator MakeMazeRandomWalk()
    {
        diggerPos = new Vector2Int(1, 1);
        SetTile(1, 1, false);
        Direction tempDir;
        while (true)
        {
            tempDir = RandomDirection();
            TakeRandomStep(tempDir);
            SetTile(diggerPos, false);
            TakeRandomStep(tempDir);
            SetTile(diggerPos, false);
            MoveDigger();

            yield return new WaitForSeconds(timer);
        }
    }

    void TakeRandomStep(Direction direction)
    {
        switch (direction)
        {
            case Direction.NORTH:
                if (diggerPos.y < size - 2 && maze[diggerPos.x, diggerPos.y+2].activeSelf)
                    diggerPos += new Vector2Int(0, 1);
                break;
            case Direction.EAST:
                if (diggerPos.x < size - 2 && maze[diggerPos.x+2, diggerPos.y].activeSelf)
                    diggerPos += new Vector2Int(1, 0);
                break;
            case Direction.SOUTH:
                if (diggerPos.y > 1 && maze[diggerPos.x, diggerPos.y-2].activeSelf)
                    diggerPos += new Vector2Int(0, -1);
                break;
            case Direction.WEST:
                if (diggerPos.x > 1 && maze[diggerPos.x-2, diggerPos.y].activeSelf)
                    diggerPos += new Vector2Int(-1, 0);
                break;
            default:
                //error
                break;
        }
    }

    // ============================================== random maze ==============================================

    IEnumerator MakeMazeFullyRandom()
    {
        int random;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                random = Random.Range(0, 100);
                SetTile(x, y, random < chanceOfVisible);

                yield return new WaitForSeconds(timer);
            }
        }
        yield return null;
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

    void MoveDigger()
    {
        digger.transform.position = (Vector2)diggerPos * spacing;
    }
}
