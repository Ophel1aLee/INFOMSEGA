using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    [Header("拖拽设置")]
    [Tooltip("拖拽速度")]
    public float dragSpeed = 2.0f;
    
    [Tooltip("是否反转拖拽方向")]
    public bool invertDrag = true;
    
    [Header("边界设置")]
    [Tooltip("是否启用移动边界")]
    public bool useBounds = true;
    
    [Tooltip("X轴移动范围（最小值）")]
    public float minX = -387f;
    [Tooltip("X轴移动范围（最大值）")]
    public float maxX = 387f;
    [Tooltip("Y轴移动范围（最小值）")]
    public float minY = -360f;
    [Tooltip("Y轴移动范围（最大值）")]
    public float maxY = 360f;

    private Vector2 dragOrigin;
    private bool isDragging = false;
    private RectTransform canvasRectTransform;
    private Vector2 originalPosition;

    private void Awake()
    {
        // 获取Canvas的RectTransform组件
        canvasRectTransform = GetComponent<RectTransform>();
        if (canvasRectTransform == null)
        {
            Debug.LogError("CameraDrag脚本需要附加到带有RectTransform组件的UI物体上！");
            enabled = false;
            return;
        }
        
        // 记录初始位置
        originalPosition = canvasRectTransform.anchoredPosition;
    }

    private void Update()
    {
        // 当按下鼠标左键时开始拖拽
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragOrigin = Input.mousePosition;
        }

        // 当释放鼠标左键时停止拖拽
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // 在拖拽过程中移动Canvas
        if (isDragging)
        {
            Vector2 currentPos = Input.mousePosition;
            Vector2 difference = currentPos - dragOrigin;
            
            // 根据设置决定是否反转拖拽方向
            if (invertDrag)
            {
                difference = -difference;
            }

            // 计算新的位置
            Vector2 newAnchoredPosition = canvasRectTransform.anchoredPosition + difference * (Time.deltaTime * dragSpeed);

            // 限制移动范围
            if (useBounds)
            {
                newAnchoredPosition.x = Mathf.Clamp(newAnchoredPosition.x, minX, maxX);
                newAnchoredPosition.y = Mathf.Clamp(newAnchoredPosition.y, minY, maxY);
            }

            // 更新位置
            canvasRectTransform.anchoredPosition = newAnchoredPosition;
            
            // 更新拖拽起始点
            dragOrigin = currentPos;
        }
    }

    // 重置位置到初始状态
    public void ResetPosition()
    {
        canvasRectTransform.anchoredPosition = originalPosition;
    }

    // 在Scene视图中绘制边界（仅在编辑器中可见）
    private void OnDrawGizmos()
    {
        if (useBounds && canvasRectTransform != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 center = transform.TransformPoint(new Vector3((maxX + minX) * 0.5f, (maxY + minY) * 0.5f, 0));
            Vector3 size = new Vector3(maxX - minX, maxY - minY, 0);
            Gizmos.DrawWireCube(center, size);
        }
    }
} 