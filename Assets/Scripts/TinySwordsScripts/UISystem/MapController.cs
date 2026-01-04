using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform content;  
    public RectTransform mapParent;
    public GameObject areaPrefab;
    public RectTransform playerIcon;

    [Header("Colors")]
    public Color defaultColor = Color.gray; // Areas on our map that we're not in 
    public Color currentAreaColor = Color.green; // Active area color

    [Header("Map Settings")]
    public GameObject mapBounds; // Parent of area colliders
    public PolygonCollider2D initialArea; // Initial starting area
    public float mapScale = 10f; // Adjust map size on UI

    private PolygonCollider2D[] mapAreas; // Children of MapBounds
    private Dictionary<string, RectTransform> uiAreas = new Dictionary<string, RectTransform>(); // Map each PolygonCollider2D to corresponding RectTransform

    public static MapController Instance { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        mapAreas = mapBounds.GetComponentsInChildren<PolygonCollider2D>();
    }

    public void GenerateMap(PolygonCollider2D newCurrentArea = null)
    {
        PolygonCollider2D currentArea = newCurrentArea != null ? newCurrentArea : initialArea;
        ClearMap();
        foreach (PolygonCollider2D area in mapAreas)
        {
            CreateAreaUI(area, area == currentArea);
        }

        SetContentSize();
        
        MovePlayerIcon(currentArea.name);
    }

    private void ClearMap()
    {
        foreach (Transform child in mapParent)
        {
            Destroy(child.gameObject);
        }
        uiAreas.Clear();
    }

    private void CreateAreaUI(PolygonCollider2D area, bool isCurrent)
    {
        // Instatiate prefab for image
        GameObject areaImage = Instantiate(areaPrefab, mapParent);
        RectTransform rectTransform = areaImage.GetComponent<RectTransform>();
        // Get bounds
        Bounds bounds = area.bounds;
        // Scale UI image fit map and bounds
        rectTransform.sizeDelta = new Vector2(bounds.size.x * mapScale, bounds.size.y * mapScale);
        rectTransform.anchoredPosition = bounds.center * mapScale;
        // Set color based on current or not
        areaImage.GetComponent<Image>().color = isCurrent ? currentAreaColor : defaultColor;
        // Add to dictionary
        uiAreas[area.name] = rectTransform;

    }

    public void UpdateCurrentArea(string newCurrentArea)
    {
        // Update Colour
        foreach (KeyValuePair<string, RectTransform> area in uiAreas)
        {
            area.Value.GetComponent<Image>().color = area.Key == newCurrentArea ? currentAreaColor : defaultColor;
        }
        MovePlayerIcon(newCurrentArea);
    }
    private void MovePlayerIcon(string newCurrentArea)
    {
        if (uiAreas.TryGetValue(newCurrentArea, out RectTransform areaUI))
        {
            Vector2 mapParentOffset = mapParent.localPosition;
            // If current area was found set the icon position to center of area
            playerIcon.anchoredPosition = areaUI.anchoredPosition + mapParentOffset;
        }
    }

    private void SetContentSize()
    {
        if (mapAreas.Length == 0) return;

        // 计算所有区域的总边界
        Bounds totalBounds = mapAreas[0].bounds;
        for (int i = 1; i < mapAreas.Length; i++)
        {
            totalBounds.Encapsulate(mapAreas[i].bounds);
        }

        // 设置Content大小
        Vector2 contentSize = new Vector2(
            totalBounds.size.x * mapScale,
            totalBounds.size.y * mapScale
        );

        content.sizeDelta = contentSize;
        
        Vector3 boundsCenter = totalBounds.center;
        mapParent.localPosition = new Vector3(
            -boundsCenter.x * mapScale,
            -boundsCenter.y * mapScale,
            0
        );
    }
}
