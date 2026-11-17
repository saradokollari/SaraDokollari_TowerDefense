using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }
    [SerializeField] private EnemyMovement basicEnemy;
    [SerializeField] private EnemyMovement tougherEnemy;
    [SerializeField] private Transform waypointRoot;
    [SerializeField] private Transform enemyParent;
    [SerializeField] private int[] enemiesPerWave = { 6, 10, 14 };
    [SerializeField] private float[] spawnIntervals = { 2.5f, 2f, 1.5f };
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private int waveBonus = 3;

    private bool running;
    private int currentWave;
    private string status = "Build your defenses, then press Start.";
    public bool GameEnded { get; private set; }
    public bool Won { get; private set; }
    public int CurrentWave => currentWave;
    public int TotalWaves => enemiesPerWave == null ? 0 : enemiesPerWave.Length;
    public string Status => status;
    private int AliveEnemies => enemyParent == null ? 0
        : enemyParent.GetComponentsInChildren<EnemyMovement>().Length;

    private void Awake() { Instance = this; }

    public bool BeginWaves()
    {
        if (running || GameEnded) return false;
        if (basicEnemy == null || !basicEnemy.gameObject.activeSelf || !basicEnemy.enabled ||
            waypointRoot == null || waypointRoot.childCount < 2 || enemyParent == null ||
            enemiesPerWave == null || enemiesPerWave.Length == 0)
        {
            Debug.LogError("Check WaveManager: assign an enabled enemy prefab, Waypoints, " +
                "Enemies parent, and wave counts.", this);
            return false;
        }
        running = true;
        Time.timeScale = 1f;
        StartCoroutine(RunWaves());
        return true;
    }

    private IEnumerator RunWaves()
    {
        for (int wave = 0; wave < enemiesPerWave.Length; wave++)
        {
            currentWave = wave + 1;
            status = "Zombies incoming!";
            float interval = 1.5f;
            if (spawnIntervals != null && wave < spawnIntervals.Length)
                interval = Mathf.Max(0.1f, spawnIntervals[wave]);

            for (int i = 0; i < enemiesPerWave[wave]; i++)
            {
                EnemyMovement prefab = basicEnemy;
                if (wave >= 1 && tougherEnemy != null && tougherEnemy.gameObject.activeSelf &&
                    tougherEnemy.enabled && i % 3 == 2) prefab = tougherEnemy;

                EnemyMovement enemy = Instantiate(prefab, waypointRoot.GetChild(0).position,
                    Quaternion.identity, enemyParent);
                enemy.SetPath(waypointRoot);
                if (i < enemiesPerWave[wave] - 1) yield return new WaitForSeconds(interval);
            }

            status = "Defeat the remaining zombies.";
            yield return null;
            while (AliveEnemies > 0) yield return new WaitForSeconds(0.2f);

            if (wave < enemiesPerWave.Length - 1)
            {
                if (TowerShop.Instance != null) TowerShop.Instance.AddCoins(waveBonus);
                float remaining = Mathf.Max(0f, timeBetweenWaves);
                while (remaining > 0f)
                {
                    status = $"Cleared! +{waveBonus} coins. Next wave: {Mathf.CeilToInt(remaining)}s";
                    float wait = Mathf.Min(1f, remaining);
                    yield return new WaitForSeconds(wait);
                    remaining -= wait;
                }
            }
        }
        FinishGame(true);
    }

    public void EnemyEscaped()
    {
        if (!running || GameEnded) return;
        FinishGame(false);
    }

    private void FinishGame(bool victory)
    {
        GameEnded = true;
        Won = victory;
        running = false;
        status = victory ? "All waves defeated." : "A zombie reached the village.";
        StopAllCoroutines();
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().path);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
