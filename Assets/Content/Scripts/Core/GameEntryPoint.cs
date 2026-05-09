using UnityEngine;

public class GameEntryPoint : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject canvasPrefab;

    private void Start()
    {
        Instantiate(playerPrefab);
        Instantiate(canvasPrefab);
    }
}
