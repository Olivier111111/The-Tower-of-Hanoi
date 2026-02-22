using UnityEngine;

public class HanoiSetup : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public Peg[] pegs;                 // All peg objects
    public GameObject diskPrefab;       // Disk prefab to spawn
    public Transform diskSpawnPoint;    // Bottom of the first peg
    public Camera mainCamera;

    [Header("Peg Spacing")]
    public float spacing3Pegs = 2.5f;
    public float spacing4Pegs = 2.0f;

    [Header("Camera Positions")]
    public Vector3 cameraPos3Pegs = new Vector3(0, 4, -8);
    public Vector3 cameraPos4Pegs = new Vector3(0, 5, -10);
    public Vector3 cameraPosCyclic = new Vector3(0, 6, -12);

    [Header("Peg Labels")]
    public GameObject pegLabelPrefab;
    public float labelHeightOffset = 1.5f;

    [Header("Manual Weights (Weighted Mode Only)")]
    public int[] diskWeights;

    void Start()
    {
        if (GameConfig.Instance == null)
        {
            Debug.LogError("GameConfig not found! Make sure it exists in the Menu scene.");
            return;
        }

        if (pegs == null || pegs.Length == 0)
        {
            Debug.LogError("No pegs assigned in HanoiSetup!");
            return;
        }

        if (diskPrefab == null)
        {
            Debug.LogError("No disk prefab assigned in HanoiSetup!");
            return;
        }

        int pegCount = Mathf.Clamp(GameConfig.Instance.pegCount, 3, pegs.Length);
        int diskCount = Mathf.Clamp(GameConfig.Instance.diskCount, 1, 5); // allow up to 5 disks
        GameConfig.GameMode mode = GameConfig.Instance.gameMode;

        Debug.Log($"Peg count: {pegCount}, Disk count: {diskCount}, Mode: {mode}");

        // Activate only selected pegs
        for (int i = 0; i < pegs.Length; i++)
            pegs[i].gameObject.SetActive(i < pegCount);

        AdjustPegSpacing(pegCount);
        AdjustCamera(pegCount, mode);
        CreatePegLabels(pegCount);
        SpawnDisks(diskCount, mode);
    }

    void AdjustPegSpacing(int pegCount)
    {
        float spacing = (pegCount == 4) ? spacing4Pegs : spacing3Pegs;
        float startX = -spacing * (pegCount - 1) / 2f;

        for (int i = 0; i < pegCount; i++)
        {
            Vector3 pos = pegs[i].transform.position;
            pos.x = startX + i * spacing;
            pegs[i].transform.position = pos;
        }
    }

    void CreatePegLabels(int pegCount)
    {
        string[] letters = { "A", "B", "C", "D" };

        for (int i = 0; i < pegCount; i++)
        {
            if (pegLabelPrefab == null) break;

            GameObject label = Instantiate(pegLabelPrefab);
            var tmp = label.GetComponent<TMPro.TMP_Text>();
            if (tmp != null)
                tmp.text = letters[i];

            // Position above peg
            label.transform.position = pegs[i].transform.position + Vector3.up * labelHeightOffset;

            // Face camera
            if (mainCamera != null)
            {
                label.transform.LookAt(mainCamera.transform);
                label.transform.Rotate(0, 180f, 0);
            }
        }
    }
    void AdjustCamera(int pegCount, GameConfig.GameMode mode)
    {
        if (mainCamera == null || pegs == null || pegCount == 0) return;

        Vector3 center = Vector3.zero;
        int activePegs = 0;

        for (int i = 0; i < pegCount; i++)
        {
            if (pegs[i] != null && pegs[i].gameObject.activeSelf)
            {
                center += pegs[i].transform.position;
                activePegs++;
            }
        }

        if (activePegs > 0)
            center /= activePegs;

        float height;
        float distance;

        if (pegCount == 4)
        {
            height = 3f;      // lower
            distance = 6f;    // closer
        }
        else
        {
            height = 3.5f;
            distance = 6f;
        }

        if (mode == GameConfig.GameMode.Cyclic)
        {
            height += 0.5f;
            distance += 1f;
        }

        Vector3 newCamPos = center + new Vector3(0, height, -distance);
        mainCamera.transform.position = newCamPos;

        Vector3 lookTarget = center + Vector3.up * 1.2f;
        mainCamera.transform.LookAt(lookTarget);
    }

    void SpawnDisks(int diskCount, GameConfig.GameMode mode)
    {
        Peg startingPeg = pegs[0];

        // Custom HEX colors
        Color[] colors = new Color[4];
        ColorUtility.TryParseHtmlString("#4A90E2", out colors[0]); // Blue
        ColorUtility.TryParseHtmlString("#50C878", out colors[1]); // Green
        ColorUtility.TryParseHtmlString("#D64545", out colors[2]); // Red
        ColorUtility.TryParseHtmlString("#F0BE46", out colors[3]); // Yellow

        float largest = 1.4f;
        float smallest = 0.5f;

        for (int i = 0; i < diskCount; i++)
        {
            GameObject newDisk = Instantiate(diskPrefab, diskSpawnPoint.position, Quaternion.identity);
            Disk diskComp = newDisk.GetComponent<Disk>();

            if (diskComp == null)
            {
                Debug.LogError("Disk component missing on prefab!");
                Destroy(newDisk);
                continue;
            }


            // Assign weight manually if in Weighted mode
            if (mode == GameConfig.GameMode.Weighted)
            {
                if (diskWeights != null && i < diskWeights.Length)
                    diskComp.weight = diskWeights[i];
                else
                    diskComp.weight = 1; // fallback if not enough weights set
            }
            else
            {
                diskComp.weight = 1;
            }

            // Smooth scaling from largest to smallest
            float t = (diskCount == 1) ? 0 : (float)i / (diskCount - 1);
            float scale = Mathf.Lerp(largest, smallest, t);
            newDisk.transform.localScale = new Vector3(scale, 0.2f, scale);

            // Apply color
            Renderer rend = newDisk.GetComponent<Renderer>();
            if (rend != null)
                rend.material.color = colors[i % colors.Length];

            // Stack correctly using renderer height
            float height = rend != null ? rend.bounds.size.y : 0.2f;
            Vector3 pos = startingPeg.snapPoint.position;
            pos.y += height * i + height / 2f;
            newDisk.transform.position = pos;

            // Assign to peg
            diskComp.currentPeg = startingPeg;
            startingPeg.AddDisk(newDisk.transform);
        }
    }
}