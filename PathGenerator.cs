using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System.Linq;

public class PathGenerator : MonoBehaviour
{
    //i didnt comment my code and now i regret everything, oh c# god please help me to understand this absolute bolognese of a script
    [Header("configurable variables")]
    public Vector2 border;
    public GameObject pathPrefab;
    public int steps;
    public int splits;
    public float pathOffset;
    public bool PathSplitting;
    public SpawnWave WaveSpanwer;
    
    [Space]

    public int SpacesMovedUp;
    public int SpacesMovedForward;
    public int SpacesMovedDown;
    
    [Space]

    public int BiasUp;
    public int BiasForward;
    public int BiasDown;
    public int BiasSplit;
    [Tooltip("set to 0 to have no points be deleted and set it higher to have more points deleted")]
    public int BiasPoints;

    [Header("bezier stuff")]
    public PathCreator pathCreator;
    public List<Vector2> points = new List<Vector2>();

    [Header("debug")]
    public int i;
    public int rand;
    public Vector2 spawnPosition;
    public bool up, forwards, down, split;
    public bool pathFinished;
    public GameObject PathHolder;
    public List<GameObject> splitPath = new List<GameObject>();
    public static int NumberOfSplits;

    private void Start()
    {
        if (!split)
            spawnPosition.x = border.x * -1;
        PathHolder = new GameObject("path Holder");
        PathHolder.transform.parent = transform.parent;
    }

    private void FixedUpdate()
    {
        if (i <= steps)
        {
            rand = Random.Range(1, BiasUp + BiasForward + BiasDown + BiasSplit + 1);
            if (Enumerable.Range(1 ,BiasUp).Contains(rand))
            {
                //Debug.Log("up");
                if (!down)
                {
                    if (spawnPosition.y <= border.y)
                    {
                        //up
                        if (forwards)
                            points.Add(spawnPosition);
                        for (int i = 0; i < SpacesMovedUp; i++)
                        {
                            spawnPosition.y++;
                            GameObject path = Instantiate(pathPrefab, spawnPosition, transform.rotation, PathHolder.transform);
                            SpawnWave.points.Add(path);
                            //Debug.Log("moved forwards by 1 tile");
                        }
                        up = true;
                        down = false;
                        forwards = false;
                        split = false;
                    }
                    //else
                        //Debug.Log("touching top border");
                }
            }

            if (Enumerable.Range(BiasUp + 1, BiasForward).Contains(rand))
            {
                //Debug.Log("forwards");
                if (spawnPosition.x <= border.x)
                {
                    //forwards
                    if (!forwards)
                        points.Add(spawnPosition);
                    for (int i = 0; i < SpacesMovedForward; i++)
                    {
                        spawnPosition.x++;
                        GameObject path = Instantiate(pathPrefab, spawnPosition, transform.rotation, PathHolder.transform);
                        SpawnWave.points.Add(path);
                        //Debug.Log("moved forwards by 1 tile");
                    }
                    up = false;
                    forwards = true;
                    down = false;
                    split = false;
                }
                else
                {
                    points.Add(spawnPosition);
                    End();
                    //Debug.Log("touching right border");
                }
            }

            if (Enumerable.Range(BiasUp + BiasForward + 1, BiasUp).Contains(rand))
            {
                //Debug.Log("down");
                if (!up)
                {
                    if (spawnPosition.y >= border.y * -1)
                    {
                        //down
                        if (forwards)
                            points.Add(spawnPosition);
                        for (int i = 0; i < SpacesMovedDown; i++)
                        {
                            spawnPosition.y--;
                            GameObject path = Instantiate(pathPrefab, spawnPosition, transform.rotation, PathHolder.transform);
                            SpawnWave.points.Add(path);
                        }
                        //SplitPathStart.Add(spawnPosition);
                        up = false;
                        forwards = false;
                        down = true;
                        split = false;
                    }
                    //else
                        //Debug.Log("touching bottom border");
                }
            }

            if (PathSplitting)
            {
                if (Enumerable.Range(BiasUp + BiasForward + BiasDown + 1, BiasSplit).Contains(rand))
                {
                    if (NumberOfSplits != splits)
                    {
                        if (forwards)
                        {
                            if (spawnPosition.y >= border.y * -1 || spawnPosition.y <= border.y)
                            {
                                split = true;
                                splitPath.Add(Instantiate(gameObject, transform.position, transform.rotation));
                                splitPath.Last().transform.parent = transform.parent;
                                splitPath.Last().GetComponent<PathGenerator>().spawnPosition = spawnPosition;
                                splitPath.Last().GetComponent<PathGenerator>().BiasDown *= 2;
                                splitPath.Last().GetComponent<PathGenerator>().points.Clear();
                                splitPath.Last().GetComponent<PathGenerator>().ForceDown(8);
                                splitPath.Last().GetComponent<PathGenerator>().split = true;
                                ForceUp(8);
                                NumberOfSplits++;
                                BiasUp *= 2;
                            }
                        }
                    }
                }
            }

            i++;
        }
        if (PathSplitting)
        {
            if (pathFinished)
            {
                for (int i = 0; i < splitPath.Count; i++)
                {
                    splitPath[i].GetComponent<PathGenerator>().pathCreator.bezierPath.MovePoint(0, RandomUtils.GetNearestFromList(splitPath[i].GetComponent<PathGenerator>().points[0], points.ToArray()));
                }
            }
        }
    }

    //run this to force the path to move down by X
    public void ForceDown(int MoveDownByX)
    {
        if (spawnPosition.y >= border.y * -1)
        {
            //down
            if (forwards)
                points.Add(spawnPosition);
            for (int i = 0; i < MoveDownByX; i++)
            {
                spawnPosition.y--;
                GameObject path = Instantiate(pathPrefab, spawnPosition, transform.rotation, PathHolder.transform);
                SpawnWave.points.Add(path);
            }
            up = false;
            forwards = false;
            down = true;
            split = false;
        }
        //else
            //Debug.Log("touching bottom border");
    }

    //run this to force the path to move up by X
    public void ForceUp(int MoveUpByX)
    {
        if (spawnPosition.y <= border.y)
        {
            //up
            if (forwards)
                points.Add(spawnPosition);
            for (int i = 0; i < MoveUpByX; i++)
            {
                spawnPosition.y++;
                GameObject path = Instantiate(pathPrefab, spawnPosition, transform.rotation, PathHolder.transform);
                SpawnWave.points.Add(path);
                //Debug.Log("moved forwards by 1 tile");
            }
            up = true;
            down = false;
            forwards = false;
            split = false;
        }
        //else
            //Debug.Log("touching top border");
    }

    //run this to force the path to move forwards by X
    public void ForceForwards(int MoveForwardsByX)
    {
        if (spawnPosition.x <= border.x)
        {
            //forwards
            if (!forwards)
                points.Add(spawnPosition);
            for (int i = 0; i < MoveForwardsByX; i++)
            {
                spawnPosition.x++;
                GameObject path = Instantiate(pathPrefab, spawnPosition, transform.rotation, PathHolder.transform);
                SpawnWave.points.Add(path);
                //Debug.Log("moved forwards by 1 tile");
            }
            up = false;
            forwards = true;
            down = false;
            split = false;
        }
        else
        {
            points.Add(spawnPosition);
            End();
            //Debug.Log("touching right border");
        } 
    }

    void End()
    {
        for (int i = 1; i < points.Count - 1; i++)
        {
            int rand = Random.Range(0, BiasPoints);
            if (rand != 1)
                points.RemoveAt(i);
        }

        //create spline
        for (int i = 0; i < points.Count; i++)
        {
            //offset
            /*
            if (SpacesMovedDown != SpacesMovedUp)
                points[i] = new Vector2(points[i].x + Random.Range(0f, SpacesMovedForward) / 2, points[i].y + Random.Range(0, Mathf.Max(SpacesMovedUp, SpacesMovedDown) - Mathf.Min(SpacesMovedUp, SpacesMovedDown)));
            else
                points[i] = new Vector2(points[i].x + Random.Range(0f, SpacesMovedForward) / 2, points[i].y + Random.Range(0, SpacesMovedDown)); //you can use spaces moved up or down here
            */

            points[i] = new Vector2(points[i].x + Random.Range(pathOffset * -1, pathOffset) / 2, points[i].y + Random.Range(pathOffset * -1, pathOffset));
        }

        pathCreator.bezierPath = new BezierPath(points.ToArray(), false, PathSpace.xy);

        if (PathSplitting)
        {
            for (int i = 0; i < splitPath.Count; i++)
            {
                splitPath[i].GetComponent<PathGenerator>().pathCreator.bezierPath.MovePoint(0, RandomUtils.GetNearestFromList(splitPath[i].GetComponent<PathGenerator>().points[0], points.ToArray()));
            }
        }

        i = steps;
        PathHolder.SetActive(false);
        pathFinished = true;
        WaveSpanwer.StartRound();
    }

    private void OnDrawGizmosSelected()
    {
        //draw border
        Gizmos.color = new Color(1, 0, 0);
        Gizmos.DrawWireCube(transform.position, new Vector3(border.x, border.y) * 2);
    }
}