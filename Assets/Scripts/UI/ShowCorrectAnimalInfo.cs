using UnityEngine;
using UnityEngine.UI;

public class ShowCorrectAnimalInfo : MonoBehaviour
{
    public Button showInfoButton;    // ��Show Info�� ��ť
    public AnimalInfoWindow infoWindowPrefab;  // ���� Prefab

    private AnimalInfoWindow _windowInstance;
    private AnimalData _correctAnimal;
    private bool _isWindowVisible = false;

    void Start()
    {
        // 1. ȡ�浵��ĵ�ǰ ID
        var saveData = SaveManager.Instance.CurrentSaveData;
        int id = saveData.CurrentCollectingID;

        // 2. �� MainManager �õ� AnimalData��ScriptableObject��
        if (!MainManager.Instance.AnimalData.TryGetValue(id, out _correctAnimal))
        {
            Debug.LogError($"�Ҳ��� ID={id} �� AnimalData");
            showInfoButton.interactable = false;
            return;
        }

        // 3. �󶨰�ť
        showInfoButton.onClick.AddListener(OnShowInfo);
    }

    private void OnShowInfo()
    {
        if (_windowInstance == null)
        {
            _windowInstance = Instantiate(infoWindowPrefab, showInfoButton.transform.root);
            _windowInstance.Initialize(_correctAnimal);

            // �󶨹ر��¼�
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
