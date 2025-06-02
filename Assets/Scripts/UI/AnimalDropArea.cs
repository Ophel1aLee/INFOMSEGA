using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 挂在画面中央表示动物的 UI 元素（通常是一个 Image 或其父物体）上，实现 IDropHandler。
/// 当玩家把带有 DraggableFood 的按钮放到这里，就调用 AnimalFeeding 的判断逻辑。
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class AnimalDropArea : MonoBehaviour, IDropHandler
{
    [Tooltip("把挂着 AnimalFeeding 脚本的同一个 GameObject（或其他管理器）拖到这里")]
    public AnimalFeeding feedingManager;

    public void OnDrop(PointerEventData eventData)
    {
        // 拖拽过来的 GameObject
        GameObject droppedGO = eventData.pointerDrag;
        if (droppedGO == null) return;

        // 看看它是否有 DraggableFood 组件
        var draggable = droppedGO.GetComponent<DraggableFood>();
        if (draggable != null && feedingManager != null)
        {
            // 调用 Manager 的判断方法
            feedingManager.FoodChoose(draggable.dietType);
        }
    }
}
