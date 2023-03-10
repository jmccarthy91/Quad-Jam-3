using UnityEngine;

public class Plateau : MonoBehaviour
{
    [Header("Plateau Data")]
    [SerializeField] private Transform[] _enemySpawnPoints;
    [SerializeField] private Transform[] _miningNodeSpawnPoints;
    [SerializeField] private GameObject _miningNodeObject = null;
    [SerializeField] private GameObject _enemyObject = null;

    private void Awake()
    {
        SpawnObjects();
    }

    private void SpawnObjects()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        var minerals = GameObject.FindGameObjectsWithTag("Mineral");

        foreach (GameObject o in enemies)
            Destroy(o);

        foreach (GameObject o in minerals)
            Destroy(o);

        foreach (Transform t in _miningNodeSpawnPoints)
        {
            Instantiate(_miningNodeObject, t.position, t.rotation);
        }

        foreach (Transform t in _enemySpawnPoints)
        {
            Instantiate(_enemyObject, t.position, t.rotation);
        }
    }
}
