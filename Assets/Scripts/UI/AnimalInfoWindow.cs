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
        // ��ʼ���ش���
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ��ʼ��һ�Σ���������ֶ�
    /// </summary>
    public void Initialize(AnimalData data)
    {
        if (_isInitialized) return;  // ֻ��һ��

        // ���ز�����ͼƬ
        Texture2D tex = Resources.Load<Texture2D>(data.AnimalPicture);
        if (tex == null)
        {
            Debug.LogError($"�Ҳ���ͼƬ��{data.AnimalPicture}");
        }
        else
        {
            animalImage.sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f)
            );
        }

        // ���ı�
        nameText.text = data.AnimalName;
        descText.text = data.AnimalDescription;
        speciesText.text = data.AnimalSpecies;
        habitatText.text = $"{data.AnimalHabitat}";
        dietText.text = $"{data.AnimalDiet}";

        // �󶨹رհ�ť��ֻ��һ��
        closeButton.onClick.AddListener(Hide);

        _isInitialized = true;
    }

    /// <summary>
    /// ��ʾ����
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// ���ش���
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
