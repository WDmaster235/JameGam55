using UnityEngine;
using UnityEngine.UI;

public sealed partial class TowerDefenseGame
{
    private void BuildUi()
    {
        GameObject canvasObject = new GameObject("Tower Defense UI", typeof(RectTransform));
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(1280f, 720f);

        GameObject topPanel = CreatePanel(canvasRect, "Top Bar", new Color(0.08f, 0.1f, 0.12f, 0.86f));
        RectTransform topRect = topPanel.GetComponent<RectTransform>();
        topRect.anchorMin = new Vector2(0f, 1f);
        topRect.anchorMax = new Vector2(1f, 1f);
        topRect.pivot = new Vector2(0.5f, 1f);
        topRect.anchoredPosition = Vector2.zero;
        topRect.sizeDelta = new Vector2(0f, 72f);

        milkText = CreateText(topRect, "MilkText", "Milk: 0", 28, TextAnchor.UpperLeft);
        SetRect(milkText.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(18f, -6f), new Vector2(220f, 32f), new Vector2(0f, 1f));

        GameObject milkBack = CreatePanel(topRect, "Milk Bar Back", new Color(0.16f, 0.18f, 0.21f, 1f));
        RectTransform milkBackRect = milkBack.GetComponent<RectTransform>();
        SetRect(milkBackRect, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(18f, -48f), new Vector2(300f, 18f), new Vector2(0f, 0.5f));

        GameObject milkFillObject = CreatePanel(milkBackRect, "Milk Bar Fill", new Color(0.72f, 0.93f, 0.97f, 1f));
        milkFill = milkFillObject.GetComponent<Image>();
        milkFill.type = Image.Type.Filled;
        milkFill.fillMethod = Image.FillMethod.Horizontal;
        milkFill.fillOrigin = 0;
        SetRect(milkFill.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));

        waveText = CreateText(topRect, "WaveText", "Wave 0", 24, TextAnchor.UpperRight);
        SetRect(waveText.rectTransform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-18f, -8f), new Vector2(210f, 28f), new Vector2(1f, 1f));

        statusText = CreateText(topRect, "StatusText", "Choose a tower, then click a tile.", 20, TextAnchor.MiddleRight);
        SetRect(statusText.rectTransform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-18f, -46f), new Vector2(650f, 28f), new Vector2(1f, 0.5f));

        countdownText = CreateText(canvasRect, "WaveCountdown", "", 88, TextAnchor.MiddleCenter);
        countdownText.color = new Color(1f, 0.94f, 0.54f);
        countdownText.gameObject.SetActive(false);
        SetRect(countdownText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(620f, 220f), new Vector2(0.5f, 0.5f));

        GameObject towerBar = CreatePanel(canvasRect, "Tower Bar", new Color(0.08f, 0.1f, 0.12f, 0.86f));
        RectTransform towerBarRect = towerBar.GetComponent<RectTransform>();
        towerBarRect.anchorMin = new Vector2(0f, 0f);
        towerBarRect.anchorMax = new Vector2(1f, 0f);
        towerBarRect.pivot = new Vector2(0.5f, 0f);
        towerBarRect.anchoredPosition = Vector2.zero;
        towerBarRect.sizeDelta = new Vector2(0f, 82f);

        float buttonX = 18f;
        foreach (TowerDefinition towerDefinition in GameDefinitions.AllTowers)
        {
            TowerKind kind = towerDefinition.Kind;
            Button button = CreateButton(towerBarRect, towerDefinition.DisplayName + "\n" + towerDefinition.Cost + " Milk", 19, towerDefinition.MainColor);
            RectTransform buttonRect = button.GetComponent<RectTransform>();
            SetRect(buttonRect, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(buttonX, 0f), new Vector2(168f, 60f), new Vector2(0f, 0.5f));
            button.onClick.AddListener(delegate { SelectBuild(kind); });
            towerButtons.Add(kind, button);
            buttonX += 178f;
        }

        GameObject detailPanel = CreatePanel(canvasRect, "Tower Detail", new Color(0.08f, 0.1f, 0.12f, 0.86f));
        RectTransform detailRect = detailPanel.GetComponent<RectTransform>();
        detailRect.anchorMin = new Vector2(1f, 0f);
        detailRect.anchorMax = new Vector2(1f, 0f);
        detailRect.pivot = new Vector2(1f, 0f);
        detailRect.anchoredPosition = new Vector2(-16f, 98f);
        detailRect.sizeDelta = new Vector2(296f, 238f);

        selectedText = CreateText(detailRect, "SelectedText", "Selected: none", 18, TextAnchor.UpperLeft);
        SetRect(selectedText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(12f, -10f), new Vector2(-24f, 30f), new Vector2(0f, 1f));

        detailText = CreateText(detailRect, "DetailText", "Click a placed tower.", 18, TextAnchor.UpperLeft);
        SetRect(detailText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(12f, -44f), new Vector2(-24f, 120f), new Vector2(0f, 1f));

        upgradeButton = CreateButton(detailRect, "Upgrade", 18, new Color(0.86f, 0.78f, 0.35f));
        SetRect(upgradeButton.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(12f, 58f), new Vector2(130f, 40f), new Vector2(0f, 0f));
        upgradeButton.onClick.AddListener(UpgradeSelectedTower);

        rechargeButton = CreateButton(detailRect, "Recharge", 18, new Color(0.42f, 0.78f, 0.95f));
        SetRect(rechargeButton.GetComponent<RectTransform>(), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-142f, 58f), new Vector2(130f, 40f), new Vector2(0f, 0f));
        rechargeButton.onClick.AddListener(RechargeSelectedTower);

        sellButton = CreateButton(detailRect, "Sell", 18, new Color(0.94f, 0.5f, 0.45f));
        SetRect(sellButton.GetComponent<RectTransform>(), new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(12f, 12f), new Vector2(-24f, 38f), new Vector2(0f, 0f));
        sellButton.onClick.AddListener(SellSelectedTower);
    }

    private GameObject CreatePanel(Transform parent, string objectName, Color color)
    {
        GameObject panel = new GameObject(objectName, typeof(RectTransform));
        panel.transform.SetParent(parent, false);
        Image image = panel.AddComponent<Image>();
        image.color = color;
        return panel;
    }

    private Text CreateText(Transform parent, string objectName, string text, int fontSize, TextAnchor anchor)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform));
        textObject.transform.SetParent(parent, false);
        Text uiText = textObject.AddComponent<Text>();
        uiText.font = defaultFont;
        uiText.text = text;
        uiText.fontSize = fontSize;
        uiText.alignment = anchor;
        uiText.color = Color.white;
        uiText.horizontalOverflow = HorizontalWrapMode.Wrap;
        uiText.verticalOverflow = VerticalWrapMode.Truncate;
        return uiText;
    }

    private Button CreateButton(Transform parent, string label, int fontSize, Color color)
    {
        GameObject buttonObject = CreatePanel(parent, label, color);
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonObject.GetComponent<Image>();

        Text buttonText = CreateText(buttonObject.transform, "Text", label, fontSize, TextAnchor.MiddleCenter);
        buttonText.color = Color.black;
        SetRect(buttonText.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));

        return button;
    }

    private void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta, Vector2 pivot)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;
        rect.pivot = pivot;
    }
}
