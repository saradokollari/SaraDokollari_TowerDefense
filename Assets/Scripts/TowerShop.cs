using UnityEngine;
using UnityEngine.InputSystem;

public class TowerShop : MonoBehaviour
{
    public static TowerShop Instance { get; private set; }

    [SerializeField] private Camera gameCamera;
    [SerializeField] private Transform towerParent;
    [SerializeField] private GameObject[] towerPrefabs = new GameObject[3];
    [SerializeField] private int startingCoins = 10;
    [Header("Winter UI")]
    [SerializeField] private Font uiFont;
    [SerializeField] private Texture2D coinIcon;

    private readonly string[] towerNames = { "Basic", "Rapid", "Heavy" };
    private readonly string[] descriptions = { "Steady fire", "Fast arrows", "Heavy cannon" };
    private readonly int[] prices = { 4, 6, 10 };
    private int coins;
    private TowerSlot selectedSlot;
    private bool started;
    private string message = "Choose your defenses, then start Wave 1.";
    private Rect originalCameraRect;
    private bool cameraRectSaved;

    private bool Ended => WaveManager.Instance != null && WaveManager.Instance.GameEnded;
    private Rect MenuRect => new Rect(SnowUI.Width - 760f, 12f, 728f, 176f);

    private void Awake()
    {
        Instance = this;
        coins = startingCoins;
        Time.timeScale = 0f;
        ReserveHeaderSpace();
    }

    private void ReserveHeaderSpace()
    {
        if (gameCamera == null) return;
        if (!cameraRectSaved)
        {
            originalCameraRect = gameCamera.rect;
            cameraRectSaved = true;
        }
        float fraction = SnowUI.HeaderHeight * SnowUI.Scale / Mathf.Max(1, Screen.height);
        gameCamera.rect = new Rect(0f, 0f, 1f, 1f - fraction);
    }

    private void Update()
    {
        ReserveHeaderSpace();
        if (Ended || Mouse.current == null || gameCamera == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        if (SnowUI.MousePosition().y < SnowUI.HeaderHeight) return;

        Vector2 screenPoint = Mouse.current.position.ReadValue();
        TowerSlot clickedSlot = null;
        Ray ray = gameCamera.ScreenPointToRay(screenPoint);
        if (Physics.Raycast(ray, out RaycastHit hit, 200f,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
        {
            clickedSlot = hit.collider.GetComponentInParent<TowerSlot>();
            if (clickedSlot == null)
            {
                foreach (TowerSlot slot in FindObjectsByType<TowerSlot>(FindObjectsSortMode.None))
                {
                    if (!slot.Owns(hit.collider.transform)) continue;
                    clickedSlot = slot;
                    break;
                }
            }
        }
        selectedSlot = clickedSlot;
        message = selectedSlot == null ? "Click a glowing spot to build, or a tower to sell."
            : selectedSlot.Occupied ? "Sell for a full refund, or click away to cancel."
            : "Choose a tower. Its price will be deducted from your balance.";
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        message = $"+{amount} coins added to your balance.";
    }

    private void Buy(int index)
    {
        if (Ended || selectedSlot == null || selectedSlot.Occupied) return;
        if (towerPrefabs == null || index < 0 || index >= towerPrefabs.Length ||
            index >= prices.Length || towerPrefabs[index] == null || towerParent == null)
        {
            message = "Assign the tower prefabs and Tower Parent in the Inspector.";
            return;
        }
        if (coins < prices[index]) { message = "Not enough coins."; return; }
        coins -= prices[index];
        selectedSlot.Build(towerPrefabs[index], towerParent, prices[index]);
        selectedSlot = null;
        message = $"{towerNames[index]} tower built. Click it to open its sell menu.";
    }

    private void SellSelected()
    {
        if (Ended || selectedSlot == null || !selectedSlot.Occupied) return;
        int refund = selectedSlot.Sell();
        coins += refund;
        selectedSlot = null;
        message = $"Tower sold. Full refund: +{refund} coins.";
    }

    private void StartWaves()
    {
        if (WaveManager.Instance != null && WaveManager.Instance.BeginWaves())
        {
            started = true;
            selectedSlot = null;
            message = "Defend the village! Build and sell while enemies advance.";
        }
        else message = "Check the WaveManager setup and Console.";
    }

    private void OnGUI()
    {
        Matrix4x4 previousMatrix = GUI.matrix;
        Color previousColor = GUI.color;
        GUI.color = Color.white;
        GUI.matrix = Matrix4x4.Scale(new Vector3(SnowUI.Scale, SnowUI.Scale, 1f));
        DrawHeader();
        if (Ended) DrawResult();
        else if (selectedSlot != null) DrawMenu();
        else DrawInstructions();
        GUI.matrix = previousMatrix;
        GUI.color = previousColor;
    }

    private void DrawHeader()
    {
        SnowUI.Fill(new Rect(0f, 0f, SnowUI.Width, SnowUI.HeaderHeight), SnowUI.Navy);
        SnowUI.Fill(new Rect(0f, SnowUI.HeaderHeight - 3f, SnowUI.Width, 3f), SnowUI.Ice);
        SnowUI.Text(new Rect(32f, 14f, 680f, 38f), "SNOW VILLAGE DEFENSE", uiFont, 30, Color.white);
        SnowUI.Text(new Rect(32f, 52f, 680f, 28f), "Protect the village. Stop every zombie.", uiFont, 18, SnowUI.Muted);

        SnowUI.Panel(new Rect(32f, 96f, 170f, 78f));
        if (coinIcon != null)
            GUI.DrawTexture(new Rect(44f, 107f, 56f, 56f), coinIcon, ScaleMode.ScaleToFit, true);
        else SnowUI.Text(new Rect(44f, 110f, 60f, 45f), "GOLD", uiFont, 18, SnowUI.Ice);
        SnowUI.Text(new Rect(108f, 104f, 84f, 60f), coins.ToString(), uiFont, 36, Color.white);

        SnowUI.Panel(new Rect(220f, 96f, 510f, 78f));
        WaveManager waves = WaveManager.Instance;
        string title = waves == null ? "PREPARATION"
            : waves.GameEnded ? (waves.Won ? "VILLAGE SAVED" : "DEFENSE LOST")
            : waves.CurrentWave == 0 ? $"PREPARE FOR {waves.TotalWaves} WAVES"
            : $"WAVE {waves.CurrentWave} / {waves.TotalWaves}";
        SnowUI.Text(new Rect(236f, 103f, 475f, 30f), title, uiFont, 24, SnowUI.Ice);
        string status = waves == null ? "Build before starting." : waves.Status;
        SnowUI.Text(new Rect(236f, 135f, 475f, 30f), status, uiFont, 16, SnowUI.Muted);
    }

    private void DrawInstructions()
    {
        Rect panel = MenuRect;
        SnowUI.Panel(panel);
        SnowUI.Text(new Rect(panel.x + 20f, panel.y + 12f, 680f, 30f),
            started ? "DEFEND THE VILLAGE" : "BUILD YOUR DEFENSES", uiFont, 24, SnowUI.Ice);
        SnowUI.Text(new Rect(panel.x + 20f, panel.y + 48f, 680f, 47f),
            "Click glowing brackets to build. Click a placed tower to sell it.", uiFont, 20, Color.white);
        float noticeWidth = started ? 680f : 465f;
        SnowUI.Text(new Rect(panel.x + 20f, panel.y + 104f, noticeWidth, 53f),
            message, uiFont, 17, SnowUI.Muted);
        if (!started && SnowUI.Button(new Rect(panel.x + 505f, panel.y + 109f, 200f, 48f),
            "START WAVE 1", uiFont)) StartWaves();
    }

    private void DrawMenu()
    {
        Rect panel = MenuRect;
        SnowUI.Panel(panel);
        bool occupied = selectedSlot.Occupied;
        SnowUI.Text(new Rect(panel.x + 20f, panel.y + 10f, 610f, 34f),
            occupied ? "SELL THIS TOWER?" : "CHOOSE YOUR TOWER", uiFont, 24, SnowUI.Ice);
        if (SnowUI.Button(new Rect(panel.x + 656f, panel.y + 12f, 52f, 34f), "X", uiFont))
        { selectedSlot = null; return; }

        if (occupied)
        {
            SnowUI.Text(new Rect(panel.x + 20f, panel.y + 51f, 680f, 35f),
                $"Full refund: {selectedSlot.Refund} coins. The build spot will reopen.", uiFont, 20, Color.white);
            if (SnowUI.Button(new Rect(panel.x + 20f, panel.y + 106f, 330f, 49f),
                $"SELL  +{selectedSlot.Refund}", uiFont, true, true)) SellSelected();
            if (SnowUI.Button(new Rect(panel.x + 370f, panel.y + 106f, 338f, 49f),
                "KEEP TOWER", uiFont)) selectedSlot = null;
        }
        else
        {
            for (int i = 0; i < towerNames.Length; i++)
            {
                float x = panel.x + 20f + i * 232f;
                if (SnowUI.Button(new Rect(x, panel.y + 59f, 222f, 56f),
                    $"{towerNames[i].ToUpper()}  {prices[i]} coins", uiFont, coins >= prices[i]))
                { Buy(i); break; }
                SnowUI.Text(new Rect(x, panel.y + 121f, 222f, 29f), descriptions[i],
                    uiFont, 17, SnowUI.Muted, TextAnchor.MiddleCenter);
            }
        }
    }

    private void DrawResult()
    {
        WaveManager waves = WaveManager.Instance;
        SnowUI.Fill(new Rect(0f, SnowUI.HeaderHeight, SnowUI.Width,
            SnowUI.Height - SnowUI.HeaderHeight), new Color(0.02f, 0.06f, 0.11f, 0.72f));
        float y = SnowUI.HeaderHeight + (SnowUI.Height - SnowUI.HeaderHeight) / 2f - 150f;
        Rect panel = new Rect(SnowUI.Width / 2f - 310f, y, 620f, 300f);
        SnowUI.Panel(panel);
        SnowUI.Text(new Rect(panel.x + 25f, panel.y + 25f, 570f, 65f),
            waves.Won ? "VILLAGE SAVED!" : "VILLAGE OVERRUN", uiFont, 40,
            waves.Won ? SnowUI.Ice : new Color(1f, 0.65f, 0.5f), TextAnchor.MiddleCenter);
        SnowUI.Text(new Rect(panel.x + 35f, panel.y + 100f, 550f, 70f),
            waves.Won ? $"You survived all {waves.TotalWaves} waves. Your village is safe!"
                      : "A zombie reached the exit. Reposition your defenses and try again.",
            uiFont, 23, Color.white, TextAnchor.MiddleCenter);
        if (SnowUI.Button(new Rect(panel.x + 120f, panel.y + 210f, 380f, 58f),
            "PLAY AGAIN", uiFont)) waves.RestartGame();
    }

    private void OnDestroy()
    {
        if (cameraRectSaved && gameCamera != null) gameCamera.rect = originalCameraRect;
        if (Instance == this) { Instance = null; Time.timeScale = 1f; }
    }
}
