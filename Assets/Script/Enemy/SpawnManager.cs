using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GamePlayManager gamePlayManager; // Tham chiếu đến GamePlayManager
    [SerializeField] private TowerHealth towerHealth;
    public GameObject enemyPrefab; // Prefab của quái
    public Transform[] spawnPoints; // Các điểm spawn quái
    public int[] enemiesPerLevel; // Số lượng quái theo mỗi level
    public float spawnInterval = 1f; // Khoảng thời gian giữa các lần spawn
    public GameObject button; // Nút spawn quái
    public int[] coinsPerLevel; // Số tiền nhận được cho mỗi level

    private int currentLevel = 0; // Level hiện tại
    private int enemiesSpawned; // Số quái đã spawn trong level hiện tại
    private int enemiesDefeated; // Số quái đã bị tiêu diệt

    void Start()
    {
        button.SetActive(true); // Đảm bảo nút hiện khi bắt đầu
    }

    // Phương thức để bắt đầu spawn quái cho level hiện tại khi nhấn nút
    public void SpawnCurrentLevel()
    {
        towerHealth.RestoreHealth();
        gamePlayManager.DisableUpgradeLevel(); // Vô hiệu hóa nâng cấp
        if (currentLevel < enemiesPerLevel.Length)
        {
            button.SetActive(false); // Ẩn nút sau khi nhấn
            enemiesSpawned = 0;
            enemiesDefeated = 0;
            StartCoroutine(SpawnEnemies(enemiesPerLevel[currentLevel]));
        }
        else
        {
            Debug.Log("All levels completed!");
        }
    }

    IEnumerator SpawnEnemies(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, Quaternion.identity);
        enemiesSpawned++;
    }

    // Phương thức này được gọi khi một quái bị tiêu diệt
    public void OnEnemyDefeated()
    {
        enemiesDefeated++;
        towerHealth.OnEnemyDefeated();

        // Cộng tiền khi tiêu diệt hết quái trong level
        if (enemiesDefeated >= enemiesPerLevel[currentLevel])
        {
            // Thêm số tiền tương ứng với cấp độ
            gamePlayManager.AddCoins(coinsPerLevel[currentLevel]);

            // Tăng cấp độ và hiển thị lại nút để spawn cho level tiếp theo
            currentLevel++;
            button.SetActive(true); // Hiển thị lại nút để spawn cho level tiếp theo

            // Mở lại khả năng nâng cấp
            gamePlayManager.EnableUpgradeLevel();
        }
    }
}
