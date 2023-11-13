using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExistenceHandler : MonoBehaviour
{
    private Vector3 checkPoint;
    private float deathBarrier = -10f;
    // Start is called before the first frame update
    void Start()
    {
        checkPoint = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.y < this.deathBarrier) {
            this.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            this.transform.position = this.checkPoint;
        }
    }

    public void SetCheckpoint(Vector3 position) {
        this.checkPoint = position;
    }
}
