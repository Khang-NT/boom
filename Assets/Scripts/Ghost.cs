using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ghost : MonoBehaviour, MapManagerListener {
    public MapManager mapManager;
    private Rigidbody2D rigidBody;
    public GameObject player;

    // ghost attribute
    public float heart;
    public float speed;
    public float smartness;

    // moving and pathFinding variables
    private List<GameObject> booms;
    private List<GameObject> flames;

    private bool ready;
    private bool changed;
    private MapLocation player_last_position;

    private List<MapLocation> path = new List<MapLocation>();
    private Vector3 destination;

    private const float PIVOT = 0.05f;

    // Check if ghost is ready to move to next position int path
    private bool check_ready()
    {
        // Debug.Log("Check if ghost ready : " + Vector3.Distance(transform.position, destination));
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
        //MapLocation temp = new MapLocation(7, 11);
        //path.Add(temp);
        //temp = new MapLocation(9, 11);
        //path.Add(temp);
    }

    // on flame collision handler
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "flame")
        {
            Debug.Log("Ghost mat mau do dinh flame");

            heart -= 1;
            if (heart == 0)
            {
                Debug.Log("Ghost die");

                Destroy(this.gameObject);
            }   
        }
    }

    private void move()
    {
        // Debug.Log("Ghost is moving");

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
        // Debug.Log("Init ghost value");
        player = mapManager.getPlayer();
        player_last_position = MapManager.vector3ToMapLocation(player.transform.position);

        booms = mapManager.getBooms();
        flames = mapManager.getFlame();
        pathFinding();

        ready = true;
        changed = false;
        destination = transform.position;
    }

    public void onMapChanged()
    {
        // Check if obstructions on map change
        Debug.Log("Map changed");
        booms = mapManager.getBooms();
        flames = mapManager.getFlame();
        changed = true;
    }

    // Update is called once per frame
    void Update () {
        // Check if player move to new position
        MapLocation player_current_position = MapManager.vector3ToMapLocation(player.transform.position);

        if (player_current_position.X != player_last_position.X
            || player_current_position.Y != player_last_position.Y)
        {
            Debug.Log("Player move to new position");

            player_last_position = player_current_position;
            changed = true;
        }

        // Find new path when map changed
        if (changed == true)
        {
            Debug.Log("Map thay doi");
            if (check_ready() == true)
            {
                Debug.Log("Ma da di chuyen xong, tim duong moi");

                changed = false;
                pathFinding();
            }
        }

        // Check if ghost go to old destination successfully
        if (check_ready() == true)
        {
            // Debug.Log("Ghost ready to move to new location");
            if (path.Count != 0)
            {
                MapLocation temp = path[0];

                destination = MapManager.mapLocationToVector3(temp);

                ready = false;

                path.RemoveAt(0);
            }
            else
            {
                // Dung im khong di chuyen
            }
        }
        else
        {
            // Debug.Log("Ghost is moving to new location");
            move();
        }
    }
}
