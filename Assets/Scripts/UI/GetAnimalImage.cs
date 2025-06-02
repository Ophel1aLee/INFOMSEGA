using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetAnimalImage : MonoBehaviour
{
    private Image _animalImage;

    void Start()
    {
        // 1. 在场景里找到 Canvas 下名为 "Animal" 的 Image
        var canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("[GetAnimalImage] 找不到 Canvas 对象");
            return;
        }

        var animalGO = canvas.transform.Find("Animal")?.gameObject;
        if (animalGO == null)
        {
            Debug.LogError("[GetAnimalImage] 找不到名为 \"Animal\" 的子对象");
            return;
        }

        _animalImage = animalGO.GetComponent<Image>();
        if (_animalImage == null)
        {
            Debug.LogError("[GetAnimalImage] 在 \"Animal\" 对象上找不到 Image 组件");
            return;
        }

        // 2. 从存档拿到当前要展示的动物 ID，然后在 MainManager 里取出对应的 AnimalData
        var saveData = SaveManager.Instance.CurrentSaveData;
        int id = saveData.CurrentCollectingID;

        if (!MainManager.Instance.AnimalData.TryGetValue(id, out var correctAnimal))
        {
            Debug.LogError($"[GetAnimalImage] MainManager 中找不到 ID={id} 的 AnimalData");
            return;
        }

        // 3. 用 AnimalData.AnimalPicture 从 Resources 加载 Texture2D 并生成 Sprite
        //    （确保 AnimalData.AnimalPicture 是 Resources 下的相对路径，例如 "Animals/panda"）
        Texture2D tex = Resources.Load<Texture2D>(correctAnimal.AnimalPicture);
        if (tex == null)
        {
            Debug.LogError($"[GetAnimalImage] Resources 下找不到图片：{correctAnimal.AnimalPicture}");
            return;
        }

        Sprite sprite = Sprite.Create(
            tex,
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f)
        );

        // 4. 将生成的 Sprite 赋给 Image，让它直接在 Canvas 中显示
        _animalImage.sprite = sprite;
        // 如果该 Image 对象一开始是禁用的，可以启用它：
        _animalImage.gameObject.SetActive(true);
    }
}
