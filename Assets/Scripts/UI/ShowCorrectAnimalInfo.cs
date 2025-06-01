using UnityEngine;
using UnityEngine.UI;

public class ShowCorrectAnimalInfo : MonoBehaviour
{
    public Button showInfoButton;    // “Show Info” 按钮
    public AnimalInfoWindow infoWindowPrefab;  // 窗口 Prefab

    private AnimalInfoWindow _windowInstance;
    private AnimalData _correctAnimal;
    private bool _isWindowVisible = false;

    private DifficultyEnum _currentDifficulty;

    void Start()
    {
        // 1. 取存档里的当前 ID
        var saveData = SaveManager.Instance.CurrentSaveData;
        int id = saveData.CurrentCollectingID;

        // 2. 从 MainManager 拿到 AnimalData（ScriptableObject）
        if (!MainManager.Instance.AnimalData.TryGetValue(id, out _correctAnimal))
        {
            Debug.LogError($"找不到 ID={id} 的 AnimalData");
            showInfoButton.interactable = false;
            return;
        }

        // 3. 绑定按钮
        showInfoButton.onClick.AddListener(OnShowInfo);

        // 4. 获取当前难度
        _currentDifficulty = saveData.CurrentDifficulty;
        if (_currentDifficulty > DifficultyEnum.Easy)
        {
            // 如果难度大于 Easy，则禁用按钮
            // showInfoButton.interactable = false;
            showInfoButton.gameObject.SetActive(false);
            Debug.Log("Hide \"Show Info\" button for difficulty: " + _currentDifficulty);
        }
    }

    private void OnShowInfo()
    {
        if (_windowInstance == null)
        {
            _windowInstance = Instantiate(infoWindowPrefab, showInfoButton.transform.root);
            _windowInstance.Initialize(_correctAnimal);

            // 绑定关闭事件
            _windowInstance.closeButton.onClick.AddListener(OnHideInfo);
        }

        if (_isWindowVisible)
        {
            _windowInstance.Hide();
            showInfoButton.gameObject.SetActive(true);
        }
        else
        {
            showInfoButton.gameObject.SetActive(false);
            _windowInstance.Show();
        }

        _isWindowVisible = !_isWindowVisible;
    }

    private void OnHideInfo()
    {
        _windowInstance.Hide();
        showInfoButton.gameObject.SetActive(true);
        _isWindowVisible = false;
    }
}
