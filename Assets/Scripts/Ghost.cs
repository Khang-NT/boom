using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ghost : MonoBehaviour, MapManagerListener {
    public MapManager mapManager;

    // ghost attributes
    public int smartness;
    public float heart;
    public float speed;
    private bool ready;
    private float pivot = 0.5f;

    // moving & pathFinding variables
    private List<GameObject> booms;
    private List<GameObject> flames;
    private List<GameObject> obstructions;

    private List<Vector2> path = new List<Vector2>();
    private Vector2 direction;
    private Vector2 destination;

    // Get obstructions list 
    private void get_obstructions ()
    {
        switch (smartness)
        {
            case 1:
                obstructions = new List<GameObject>(booms);
                break;
            case 2:
                obstructions = new List<GameObject>(booms);
                foreach (GameObject flame in flames)
                {
                    obstructions.Add(flame);
                }
                break;
            default:
                obstructions = new List<GameObject>(booms);
                break;
        }
    }

    private void pathFinding (List<GameObject> obstructions)
    {
        path.Clear();

        Vector2 temp = new Vector2(4.0881f, 5.834106f);

        path.Add(temp);

        Debug.Log("pathFinding");
    }

    private void check_ready()
    {
        Vector2 currentPosition = new Vector2(transform.position.x - pivot, transform.position.y - pivot);
        if (currentPosition == destination)
        {
            ready = true;
        }
        else
        {
            ready = false;
        }
    }

    // Use this for initialization
    void Start () {
        mapManager.addListener (this);
    }

	public void onMapReady() {
        ready = true;
        
        booms = mapManager.getBooms();
        flames = mapManager.getFlame();
        get_obstructions();
        pathFinding(obstructions);
        Debug.Log("Init path " + path.Count);

        direction = new Vector2(0, 0);
        destination = new Vector2(transform.position.x - pivot, transform.position.y - pivot);
    }

    private void move()
    {
        Vector3 move = new Vector3(
            direction.x * speed * Time.deltaTime * mapManager.defaultSpeed,
            direction.y * speed * Time.deltaTime * mapManager.defaultSpeed,
            0);
        transform.Translate(move);
        check_ready();
    }


    public void onMapChanged()
    {
        booms = mapManager.getBooms();
        flames = mapManager.getFlame();
        get_obstructions();
        pathFinding(obstructions);
    }

	// Update is called once per frame
	void Update () {
        if (ready == false)
        {
            Debug.Log("Not ready");
            move();
        }
        else
        { 
            if (path.Count == 0)
            {
                Debug.Log("ready but empty path");
                // Lay mot vi tri bat ki xung quanh va di chuyen
                direction = new Vector2(0, 0);
                destination = new Vector2(transform.position.x - pivot, transform.position.y - pivot);
            }
            else
            {
                Vector2 currentPosition = new Vector2(transform.position.x - pivot, transform.position.y - pivot);

                destination = path[0];
                direction = destination - currentPosition;

                path.RemoveAt(0);

                Debug.Log(destination + " " + direction);

                move();
            }
        }
    }
}
