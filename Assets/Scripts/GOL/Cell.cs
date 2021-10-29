using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool isAlive = false;
    private void Update()
    {
        this.gameObject.GetComponent<Renderer>().enabled = isAlive;
    }
}
