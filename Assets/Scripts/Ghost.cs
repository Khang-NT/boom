using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ghost : MonoBehaviour, MapManagerListener {
    public MapManager mapManager;
    private Rigidbody2D rigidBody;

    // ghost attribute
    public float heart;
    public float speed;
    public float smartness;

    // moving and pathFinding variables
    private List<GameObject> booms;
    private List<GameObject> flames;

    private bool ready;
    private bool changed;

    private List<MapLocation> path = new List<MapLocation>();
    private Vector3 destination;

    private const float PIVOT = 0.05f;

    // Check if ghost is ready to move to next position int path
    private bool check_ready()
    {
        Debug.Log("Check if ghost ready : " + Vector3.Distance(transform.position, destination));
        if (Vector3.Distance(transform.position, destination) <= PIVOT)
        {
            ready = true;
            return true;
        }
        else
        {
            ready = false;
            return false;
        }
    }

    // Find min path from ghost to player
    private void pathFinding()
    {
        path.Clear();
        List<GameObject> obstructions;
        if (smartness == 2)
        {
            obstructions = booms;
        }

        // implement floyd warshall algorithm
        MapLocation temp = new MapLocation(7, 11);
        path.Add(temp);
        temp = new MapLocation(9, 11);
        path.Add(temp);
    }

    private void move()
    {
        Debug.Log("Ghost is moving");

        Vector3 temp = destination - transform.position;
        Vector2 move = new Vector2(
            temp.x * speed * Time.deltaTime * mapManager.defaultSpeed,
            temp.y * speed * Time.deltaTime * mapManager.defaultSpeed);
        rigidBody.velocity = move;
    }

    // Use this for initialization
    void Start () {
        rigidBody = GetComponent<Rigidbody2D>();
        mapManager.addListener (this);
    }

	public void onMapReady() {
        Debug.Log("Init ghost value");

        booms = mapManager.getBooms();
        flames = mapManager.getFlame();
        pathFinding();

        ready = true;
        changed = false;
        destination = transform.position;
    }

    public void onMapChanged()
    {
        booms = mapManager.getBooms();
        flames = mapManager.getFlame();
        changed = true;
    }

	// Update is called once per frame
	void Update () {
        // Find new path when map changed
        if (changed == true)
        {
            changed = false;
            if (check_ready() == true)
            {
                pathFinding();
            }
        }

        // Check if ghost go to old destination successfully
        if (check_ready() == true)
        {
            Debug.Log("Ghost ready to move to new location");
            if (path.Count != 0)
            {
                MapLocation temp = path[0];

                destination = MapManager.mapLocationToVector3(temp);

                ready = false;

                path.RemoveAt(0);
            }
            else
            {
                // di random 1 o xung quanh
            }
        }
        else
        {
            Debug.Log("Ghost is moving to new location");
            move();
        }
    }
}
