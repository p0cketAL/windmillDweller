using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerControll : MonoBehaviour
{
    public bool walking = false;

    public Transform currentCube;
    public Transform clickedCube;
    public Transform indicator;
    

    public List<Transform> finalPath = new List<Transform>();



    // Start is called before the first frame update
    void Start()
    {
        RayCastDown();
    }

    // Update is called once per frame
    void Update()
    {
        // get current cube under the player with RayCast  
        RayCastDown();

        if (currentCube.GetComponent<Walkable>().movingGround)
        {
            transform.parent = currentCube.parent;
        }
        else
        {
            transform.parent = null;
        }

        // click on the cube to more the player
        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition); 
            RaycastHit mouseHit;

            if (Physics.Raycast(mouseRay, out mouseHit))
            {
                if (mouseHit.transform.GetComponent<Walkable>() != null)
                {
                    clickedCube = mouseHit.transform;
                    finalPath.Clear();
                    FindPath();
                    indicator.position = mouseHit.transform.GetComponent<Walkable>().GetWalkPoint();

                }
            }
        }
    }

    void FindPath()
    {
        List<Transform> nextCubes = new List<Transform>();
        List<Transform> pastCubes = new List<Transform>();

        foreach (WalkPath path in currentCube.GetComponent<Walkable>().possiblePaths)
        {
            if (path.active)
            {
                nextCubes.Add(path.target);
                path.target.GetComponent<Walkable>().previousBlock = currentCube;
            }
        }

        pastCubes.Add(currentCube);

        ExploreCube(nextCubes, pastCubes);
        BuildPath();
    }

    public void RayCastDown()
    {
        Ray playerRayCast = new Ray(transform.position, -transform.up);
        //GetChild(0) ?

        RaycastHit playerHit;
      

        if (Physics.Raycast(playerRayCast, out playerHit))
        {
            if (playerHit.transform.GetComponent<Walkable>() != null)
            {
                currentCube = playerHit.transform;
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Ray ray = new Ray(transform.GetChild(0).position, -transform.up);
        Gizmos.DrawRay(ray);
    }
    void Clear()
    {
        foreach (Transform t in finalPath)
        {
            t.GetComponent<Walkable>().previousBlock = null;
        }
        finalPath.Clear();
        walking = false;
    }

    void ExploreCube(List<Transform> nextCubes, List<Transform> visitedCubes)
    {
        Transform current = nextCubes.First();
        nextCubes.Remove(current);

        if (current == clickedCube)
        {
            return;
        }
        foreach (WalkPath path in current.GetComponent<Walkable>().possiblePaths)
        {
            if (!visitedCubes.Contains(path.target) && path.active)
            {
                nextCubes.Add(path.target);
                path.target.GetComponent<Walkable>().previousBlock = current;
            }
        }

        visitedCubes.Add(current);

        if (nextCubes.Any())
        {
            ExploreCube(nextCubes, visitedCubes);
        }
    }

    void BuildPath()
    {
        Transform cube = clickedCube;
        while (cube != currentCube)
        {
            finalPath.Add(cube);
            if (cube.GetComponent<Walkable>().previousBlock != null)
                cube = cube.GetComponent<Walkable>().previousBlock;
            else
                return;
        }

        finalPath.Insert(0, clickedCube);

        FollowPath();
    }

    void FollowPath()
    {
        walking = true;
    }
}
