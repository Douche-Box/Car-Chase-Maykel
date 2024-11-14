using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reinforcements : MonoBehaviour
{
    [SerializeField] GameObject _reinforcementPrefab;
    [SerializeField] List<Transform> _spawnTransforms = new();
    [SerializeField] float _spawnDelay;

    Transform _carTransform;

    float amountToSpawn;

    private void Start()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("ReinforcementSpawn");
        for (int i = 0; i < gameObjects.Length; i++)
        {
            _spawnTransforms.Add(gameObjects[i].transform);
        }
        _carTransform = GameObject.FindGameObjectWithTag("Car").transform;
    }

    /// <summary>
    /// Spawn in new reinforcements
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="duration"></param>
    public void SpawnReinforcements(float amount, float duration)
    {
        amountToSpawn = amount;
        StartCoroutine(SpawnCar(_spawnDelay, duration));
    }

    /// <summary>
    /// Spawns the reinforment and gets the closest criminal to persue
    /// </summary>
    /// <param name="spawnDelay"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    IEnumerator SpawnCar(float spawnDelay, float duration)
    {
        yield return new WaitForSeconds(spawnDelay);
        int closestIndex = 0;
        float closestDistance = Mathf.Infinity;
        for (int i = 0; i < _spawnTransforms.Count; i++)
        {
            float distance = Vector3.Distance(_carTransform.position, _spawnTransforms[i].position);
            if (distance < closestDistance)
            {
                closestIndex = i;
                closestDistance = distance;
            }
        }
        GameObject car = Instantiate(_reinforcementPrefab, _spawnTransforms[closestIndex].position, _spawnTransforms[closestIndex].rotation);
        car.GetComponent<PoliceAgentFollow>().maxLifeSpan = duration;
        amountToSpawn--;
        if (amountToSpawn > 0)
        {
            StartCoroutine(SpawnCar(_spawnDelay, duration));
        }
    }
}
