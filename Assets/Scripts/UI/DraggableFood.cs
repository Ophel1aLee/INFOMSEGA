using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 挂在每个“Food”按钮上，使其能被拖拽。
/// 拖拽时，会把按钮暂时移到 Canvas 根节点下，释放时回到原父物体并归位。
/// </summary>
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class DraggableFood : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public DietEnum dietType;

    private Canvas _rootCanvas;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Transform _originalParent;
    private Vector2 _originalAnchoredPos;

    void Awake()
    {
        // 找到所在 Canvas（向上遍历直到找到 Canvas）
        _rootCanvas = GetComponentInParent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 记录原父物体和位置
        _originalParent = transform.parent;
        _originalAnchoredPos = _rectTransform.anchoredPosition;

        // 将按钮临时移到 Canvas 根节点下，保证它在最上层可见
        transform.SetParent(_rootCanvas.transform);

        // 使其能穿透射线，否则会阻止 DropArea 接收事件
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 鼠标移动时，让 UI 跟随。需要除以 scaleFactor 以适配不同 Canvas 缩放
        Vector2 delta;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rootCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out delta
        );
        _rectTransform.anchoredPosition = delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 恢复父物体并归位
        transform.SetParent(_originalParent);
        _rectTransform.anchoredPosition = _originalAnchoredPos;

        // 让按钮重新可被射线检测
        _canvasGroup.blocksRaycasts = true;
    }
}
