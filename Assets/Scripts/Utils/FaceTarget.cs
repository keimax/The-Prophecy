using System.Collections.Generic;
using UnityEngine;

public class FaceTarget : MonoBehaviour
{
    public bool findPlayer;
    public GameObject targetObj; // should be private
    public float rotationSpeed = 0.25f;
    public float rotationOffset = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        if (findPlayer)
        {

            FindPlayer();
        }
        else
        {

        }
    }




    private void FindPlayer()
    {
        // Array to list - neat!
        List<GameObject> players = new();
        GameObject[] g = GameObject.FindGameObjectsWithTag("Player");
        players.AddRange(g);

        if (players.Count > 0)
        {
            try
            {
                targetObj = GameObject.FindGameObjectsWithTag("Player")[0];
                if (targetObj)
                { 
                    Debug.Log(targetObj);
          //          this.transform.GetComponent<EnemyShip>().SetTarget(targetObj);
                    this.transform.parent = targetObj.transform.parent; // player container
                }
            }
            catch (System.Exception)
            {
                //  throw;
            }
        }
    }
    public void SetTarget(GameObject g)
    {
        {
            targetObj = g;
        }
    }

    public GameObject GetTarget()
    {
        return targetObj;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetObj != null)
        {
        //    float angle = Mathf.Atan2(targetObj.transform.position.y - transform.position.y, targetObj.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        //    Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + (270 + rotationOffset)));
        //    transform.rotation = targetRotation;
        }
        else
        {
            if (targetObj == null)
            {
         //       FindPlayer();
            }

        }
    }

}

