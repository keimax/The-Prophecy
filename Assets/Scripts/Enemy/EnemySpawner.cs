
using UnityEngine;
using UnityEngine.Animations;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] shipPrefabs;
    public float spawnDistance = 12f;
    public float spawnRate = 1f;
    public int amountPerSpawn = 1;
    [Range(0f, 45f)]
    public float trajectoryVariance = 15f;

    private void Start()
    {
        GameObject.Destroy(this, 60);
        InvokeRepeating(nameof(Spawn), spawnRate, spawnRate);
    }

    public void Spawn()
    {

        int r = Random.Range(0, shipPrefabs.Length);

        GameObject shipPrefab = shipPrefabs[r];

        for (int i = 0; i < amountPerSpawn; i++)
        {
            // Choose a random direction from the center of the spawner and
            // spawn the asteroid a distance away
            Vector2 spawnDirection = Random.insideUnitCircle.normalized;
            Vector3 spawnPoint = spawnDirection * spawnDistance;

            // Offset the spawn point by the position of the spawner so its
            // relative to the spawner location
            spawnPoint += transform.position;

            // Calculate a random variance in the asteroid's rotation which will
            // cause its trajectory to change
            float variance = Random.Range(-trajectoryVariance, trajectoryVariance);
            //    Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);
            Quaternion rotation = shipPrefab.transform.rotation;

            // Create the new asteroid by cloning the prefab and set a random
            // size within the range
            GameObject newShip = Instantiate(shipPrefab, spawnPoint, rotation);


            // Set the trajectory to move in the direction of the spawner
            Vector2 trajectory = rotation * -spawnDirection;
            // newShip.SetTrajectory(trajectory);
            newShip.GetComponent<Rigidbody>().AddForce(spawnDirection);
        }
    }

}
