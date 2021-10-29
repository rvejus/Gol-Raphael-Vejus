using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Run : MonoBehaviour
{
    /*
    These rules, which compare the behavior of the automaton to real life, can be condensed into the following:
    Any live cell with two or three live neighbours survives.
    Any dead cell with three live neighbours becomes a live cell.
    All other live cells die in the next generation. Similarly, all other dead cells stay dead.
    */

    public Material aliveMaterial;
    public Material deadMaterial;
    public GameObject cell;

    public int numberOfColumns = 100;
    public int numberOfRows = 100;
    public int numberOfDepth = 100;
    public float spaceBetweenCells = 0.8f;
    public float timeToEvolveInSeconds = 0.10f;

    private List<GameObject> _cubes = new List<GameObject>();
    private GameObject[,,] _world;

    // Start is called before the first frame update
    // It initializes the world by generating cells from the "cell" prefab
    // We also generate an initial population of alive cells to get this thing started 
    void Start()
    {
        // World Array 
        _world = new GameObject[numberOfColumns, numberOfRows,numberOfDepth];

        // Put cells into work
        for (int co = 0; co < numberOfColumns; co++)
        {
            for (int li = 0; li < numberOfRows; li++)
            {
                for (int ta = 0; ta < numberOfDepth; ta++)
                {
                    _world[co, li, ta] = Instantiate(cell);
                    _world[co, li,ta].transform.position = new Vector3((float)(co + (co * spaceBetweenCells)), (float)(ta+(ta*spaceBetweenCells)), (float)(li + (li * spaceBetweenCells)));

                    // Random chance of life...
                    if (Random.value > 0.6f)
                    {
                        _world[co, li,ta].GetComponent<Renderer>().material = aliveMaterial;
                        _world[co, li,ta].GetComponent<Cell>().isAlive = true;
                    }
                }
               
            }
        }

        // Let the cells do their thing
        InvokeRepeating(nameof(Evolve), 0, timeToEvolveInSeconds);
    }

    private void Evolve()
    {
        bool[,,] newState = new bool[numberOfColumns, numberOfRows, numberOfDepth];

        // Calculate the cell moves into a new world state
        for (int co = 0; co < numberOfColumns; co++)
        {
            for (int li = 0; li < numberOfRows; li++)
            {
                for (int ta = 0; ta < numberOfDepth; ta++)
                {
                    if (_world[co, li, ta].GetComponent<Cell>().isAlive)
                        newState[co, li, ta] =
                            NumberOfAliveNeighbours(co, li,ta) == 2 || NumberOfAliveNeighbours(co, li, ta) == 3;
                    else
                        newState[co, li, ta] = NumberOfAliveNeighbours(co, li , ta) == 3;
                }
            }
        }

        // Apply the new world state to the world
        for (int co = 0; co < numberOfColumns; co++)
        {
            for (int li = 0; li < numberOfRows; li++)
            {
                for (int ta = 0; ta < numberOfDepth; ta++)
                {
                    if (_world[co, li,ta].GetComponent<Cell>().isAlive != newState[co, li,ta])
                    {
                        // If the state of the cell changed, then we perform an update
                        _world[co, li,ta].GetComponent<Renderer>().material = newState[co, li,ta] ? aliveMaterial : deadMaterial;
                        _world[co, li,ta].GetComponent<Cell>().isAlive = newState[co, li,ta];
                    }
                }
                
            }
        }
    }

    // Calculate the number of alive neighbours to determine what happens to the cell next
    // According to the rules, 2 or 3 neighbours means staying alive. And having exactly 3 neighbours brings a dead cell back to life
    int NumberOfAliveNeighbours(int co, int li , int ta)
    {
        int count = 0;

        for (int coScanner = -1; coScanner <= 1; coScanner++)
        {
            for (int liScanner = -1; liScanner <= 1; liScanner++)
            {
                for (int taScanner = -1; taScanner <= 1; taScanner++)
                {
                    int calculatedCo = co - coScanner;
                    int calculatedLi = li - liScanner;
                    int calculatedTa = ta - taScanner;
                    if (calculatedCo >= 0 && calculatedCo < numberOfColumns && calculatedLi >= 0 &&
                        calculatedLi < numberOfRows && calculatedTa >= 0 && calculatedTa < numberOfDepth)
                    {
                        if (co != calculatedCo || li != calculatedLi || ta != calculatedTa)
                        {
                            if (_world[calculatedCo, calculatedLi , calculatedTa ].GetComponent<Cell>().isAlive)
                            {
                                count++;
                            }
                        }
                    } 
                }
                
            }
        }

        return count;
    } 
}
