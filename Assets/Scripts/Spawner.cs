using UnityEngine;



/// <summary>
/// The Spawner class that spawns fish, flocks and sharks.
/// </summary>
public class Spawner : MonoBehaviour
{
    // Old code written by Allen She (I think) for spawning, I replaced it to allow for a minimum distance
    // [SerializeField] protected Vector2 spawnXRange;
    // [SerializeField] protected Vector2 spawnYRange;
    // [SerializeField] protected Vector2 spawnZRange;

    // protected Vector3 randomPos => new(
    //     Random.Range(spawnXRange.x, spawnXRange.y),
    //     Random.Range(spawnYRange.x, spawnYRange.y),
    //     Random.Range(spawnZRange.x, spawnZRange.y)
    // ); 


    [SerializeField] protected GameObject[] prefabs;
    [SerializeField] [TagSelector] protected string prefabTag;
    [SerializeField] protected int minCount;
    [SerializeField] protected int maxCount;
    [SerializeField] protected int initCount;

    private bool init = false;
    private GameObject[] gameObjects;

    [SerializeField, Range(0f, 25f)] protected float spawnMinRadius;
    [SerializeField, Range(0f, 25f)] protected float spawnMaxRadius;



    protected Vector3 randomPos{
        get {
            float angle = Random.Range(0f, 6.283f);
            float radius = Mathf.Sqrt(Random.Range(spawnMinRadius * spawnMinRadius, spawnMaxRadius * spawnMaxRadius));
            
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            float y = Random.Range(0f, 10f);
            
            return new Vector3(x,y,z);
        }
    }


    private void FixedUpdate()
    {
        gameObjects = GameObject.FindGameObjectsWithTag(prefabTag);
        int count = gameObjects.Length;
        if (count < minCount)
        {
            if (init) {
                Spawn(minCount - count);
            } else {
                Spawn(initCount);
                init = true;  //I would do this in a start or awake function but it doesnt work so i have to do it this way
            }
        } else if (count > maxCount) {
            Debug.Log("Max");
            for (int i=0; i<(count - maxCount); i++) {
                Debug.Log("Destroying", gameObjects[i]);
                Destroy(gameObjects[i]);
            }
        }
    }

    /// <summary>
    /// Spawn method spawns the prefabs.
    /// </summary>
    /// <param name="count"></param>
    protected virtual void Spawn(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
            Instantiate(prefab, randomPos, Quaternion.identity);
        }
    }

    /// <summary>
    /// SetMinCount method sets the minimum count of the prefabs.
    /// </summary>
    /// <param name="count"></param>
    public void SetMinCount(int count)
    {
        minCount = count;
    }

    public void SetMinCount(float count)
    {
        minCount = (int)count;
    }
}
