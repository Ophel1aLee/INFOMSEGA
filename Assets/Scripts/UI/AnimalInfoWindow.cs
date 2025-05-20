using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimalInfoWindow : MonoBehaviour
{
    public Image animalImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI speciesText;
    public TextMeshProUGUI habitatText;
    public TextMeshProUGUI dietText;
    public Button closeButton;

    private bool _isInitialized = false;

    void Awake()
    {
        // 初始隐藏窗口
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 初始化一次：填充所有字段
    /// </summary>
    public void Initialize(AnimalData data)
    {
        if (_isInitialized) return;  // 只做一次

        // 加载并设置图片
        Texture2D tex = Resources.Load<Texture2D>(data.AnimalPicture);
        if (tex == null)
        {
            Debug.LogError($"找不到图片：{data.AnimalPicture}");
        }
        else
        {
            animalImage.sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f)
            );
        }

        // 填文本
        nameText.text = data.AnimalName;
        descText.text = data.AnimalDescription;
        speciesText.text = data.AnimalSpecies;
        habitatText.text = $"{data.AnimalHabitat}";
        dietText.text = $"{data.AnimalDiet}";

        // 绑定关闭按钮，只第一次
        closeButton.onClick.AddListener(Hide);

        _isInitialized = true;
    }

    /// <summary>
    /// 显示窗口
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 隐藏窗口
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
