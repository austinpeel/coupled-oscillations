using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CoordinateSpace2D : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Vector2 xRange = new Vector2(-1, 1);
    [SerializeField] private Vector2 yRange = new Vector2(-1, 1);

    [SerializeField] private RectTransform marker;
    [SerializeField] private bool snapToDiagonals;
    [SerializeField] private float snapTolerance = 0.1f;
    [SerializeField] private CoupledOscillationsSimulation sim;

    [Header("Dotted Line")]
    [SerializeField] private Sprite dot = default;
    [SerializeField] private float maxDotSpacing = 0.1f;
    [SerializeField] private float dotSize = 0.1f;
    [SerializeField] private Color dotColor = Color.black;
    [SerializeField] private bool drawDottedLines;
    private RectTransform xDots;
    private RectTransform yDots;

    private RectTransform rect;
    private bool mouseIsDown;

    private Camera eventCamera = null;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        if (sim) sim.Pause();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        mouseIsDown = true;
        eventCamera = eventData.pressEventCamera;
        if (sim) sim.Pause();

        if (drawDottedLines)
        {
            xDots = CreateDottedLineContainer("X Dots", 0);
            yDots = CreateDottedLineContainer("Y Dots", 1);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mouseIsDown = false;
        eventCamera = null;
        if (sim)
        {
            Vector2 uv = ScreenToNormalizedPosition(Input.mousePosition, eventCamera);
            if (snapToDiagonals)
            {
                uv = SnapToDiagonalUV(uv, snapTolerance);
            }
            Vector2 scaledPosition = NormalizedToScaledPosition(uv);
            sim.UpdateX(scaledPosition.x, scaledPosition.y);
            sim.UpdateXDot();
            sim.UpdateMassPositions();
            sim.UpdateSpringPositions();
            sim.Resume();
        }

        if (drawDottedLines)
        {
            Destroy(xDots.gameObject);
            xDots = null;
            Destroy(yDots.gameObject);
            yDots = null;
        }
    }

    private void Update()
    {
        if (mouseIsDown)
        {
            // Compute normalized (UV) coordinates of the mouse within this UI element
            Vector2 uv = ScreenToNormalizedPosition(Input.mousePosition, eventCamera);

            if (marker)
            {
                if (snapToDiagonals)
                {
                    uv = SnapToDiagonalUV(uv, snapTolerance);
                }
                marker.anchoredPosition = NormalizedToRectPosition(uv);
            }

            if (sim)
            {
                Vector2 scaledPosition = NormalizedToScaledPosition(uv);
                sim.UpdateX(scaledPosition.x, scaledPosition.y);
                sim.UpdateMassPositions();
                sim.UpdateSpringPositions();
            }

            if (drawDottedLines)
            {
                ClearChildren(xDots);
                ClearChildren(yDots);

                DrawDottedLine(0, uv, 0.5f * Vector2.one, xDots);
                DrawDottedLine(1, uv, 0.5f * Vector2.one, yDots);
            }
        }

        if (sim)
        {
            if (sim.paused || !marker) { return; }

            double[] x = sim.GetX1X2();
            marker.anchoredPosition = ScaledToRectPosition(new Vector2((float)x[0], (float)x[1]));
        }
    }

    private Vector2 ScreenToNormalizedPosition(Vector2 position, Camera camera)
    {
        Vector2 normalizedPosition = default;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, position, camera, out var localPosition))
        {
            normalizedPosition = Rect.PointToNormalized(rect.rect, localPosition);
        }

        return normalizedPosition;
    }

    private Vector2 SnapToDiagonalUV(Vector2 uv, float tol)
    {
        Vector2 xy = 2 * uv - Vector2.one;
        float difference = Mathf.Abs(xy.x) - Mathf.Abs(xy.y);
        if (Mathf.Abs(difference) < tol)
        {
            float value = 0.5f * (Mathf.Abs(xy.x) + Mathf.Abs(xy.y));
            xy.x = Mathf.Sign(xy.x) * value;
            xy.y = Mathf.Sign(xy.y) * value;
        }
        return 0.5f * (xy + Vector2.one);
    }

    private Vector2 NormalizedToScaledPosition(Vector2 uv)
    {
        float x = (xRange.y - xRange.x) * uv.x + xRange.x;
        float y = (yRange.y - yRange.x) * uv.y + yRange.x;
        return new Vector2(x, y);
    }

    private Vector2 ScaledToNormalizedPosition(Vector2 scaledPosition)
    {
        float u = (scaledPosition.x - xRange.x) / (xRange.y - xRange.x);
        float v = (scaledPosition.y - yRange.x) / (yRange.y - yRange.x);
        return new Vector2(u, v);
    }

    private Vector2 NormalizedToRectPosition(Vector2 uv)
    {
        return (uv - 0.5f * Vector2.one) * rect.rect.size;
    }

    private Vector2 RectToNormalizedPosition(Vector2 rectPosition)
    {
        return (rectPosition / rect.rect.size) + 0.5f * Vector2.one;
    }

    private Vector2 ScaledToRectPosition(Vector2 scaledPosition)
    {
        Vector2 uv = ScaledToNormalizedPosition(scaledPosition);
        return NormalizedToRectPosition(uv);
    }

    private void ClearChildren(RectTransform rt)
    {
        for (int i = rt.childCount; i > 0; i--)
        {
            DestroyImmediate(rt.GetChild(0).gameObject);
        }
    }

    private RectTransform CreateDottedLineContainer(string name, int siblingIndex)
    {
        var container = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        container.SetParent(transform);
        container.anchoredPosition = Vector2.zero;
        container.localScale = Vector3.one;
        container.SetSiblingIndex(siblingIndex);
        return container;
    }

    private void DrawDottedLine(int axis, Vector2 uv, Vector2 offset, RectTransform parent)
    {
        Vector2 delta = uv - offset;
        float height = axis == 0 ? delta.x : delta.y;
        float sign = Mathf.Sign(height);
        int numDots = Mathf.CeilToInt(Mathf.Abs(height) / maxDotSpacing);
        float spacing = height / numDots;
        for (int i = 0; i < numDots; i++)
        {
            RectTransform dotRT = new GameObject("Dot", typeof(Image)).GetComponent<RectTransform>();
            dotRT.SetParent(parent);
            dotRT.localScale = dotSize * Vector3.one;
            Vector2 position = axis == 0 ? new Vector2(offset.y + i * spacing, uv.y) : new Vector2(uv.x, offset.x + i * spacing);
            dotRT.anchoredPosition = NormalizedToRectPosition(position);
            Image image = dotRT.GetComponent<Image>();
            image.sprite = dot;
            image.raycastTarget = false;
            image.color = dotColor;
            image.preserveAspect = true;
            image.SetNativeSize();
        }
    }
}
