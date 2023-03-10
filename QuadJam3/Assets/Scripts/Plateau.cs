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
        PlayerDeathTrigger.OnDeathTrigger += SpawnObjects;
    }

    private void SpawnObjects()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        var minerals = GameObject.FindGameObjectsWithTag("Mineral");

        foreach (GameObject o in enemies)
            Destroy(o);

        foreach (GameObject o in minerals)
            Destroy(o);

        for (int i = 0; i < _miningNodeSpawnPoints.Length; i++)
        {
            Instantiate(_miningNodeObject, _miningNodeSpawnPoints[i].position, _miningNodeSpawnPoints[i].rotation);
        }

        for (int i = 0; i < _enemySpawnPoints.Length; i++)
        {
            Instantiate(_enemyObject, _enemySpawnPoints[i].position, _enemySpawnPoints[i].rotation);
        }
    }
}
