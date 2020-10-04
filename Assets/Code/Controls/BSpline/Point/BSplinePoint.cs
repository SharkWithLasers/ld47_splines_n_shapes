using ScriptableObjectArchitecture;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BSplinePoint : MonoBehaviour
{
    private Vector3 mOffset;
    private bool currentlyDragging;
    private float mZCoord;

    public int Index { get; private set; }

    // game events for mouse drag begun (i)... mouse drag ended (i)
    [SerializeField] private IntGameEvent bsPointIDragStarted;

    [SerializeField] private IntGameEvent bsPointIDragEnded;

    [SerializeField] private BSplinePointRenderer splinePointRenderer;
    private bool withinCollider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Construct(int index, Color color)
    {
        Index = index;

        splinePointRenderer.SetColor(color);
    }

    private void OnMouseEnter()
    {
        withinCollider = true;

        if (!currentlyDragging)
        {
            splinePointRenderer.MouseEnterHappened();
        }
    }

    private void OnMouseExit()
    {
        withinCollider = false;

        if (!currentlyDragging)
        {
            splinePointRenderer.MouseExitHappened();
        }
    }

    private void OnMouseDown()
    {
        mZCoord = Camera.main.ScreenToWorldPoint(transform.position).z;
        mOffset = transform.position - GetMouseWorldPosClamped();

        splinePointRenderer.MouseDownHappened();

        currentlyDragging = true;

        bsPointIDragStarted.Raise(Index);
    }

    private Vector3 GetMouseWorldPosClamped()
    {
        var mousePoint = Input.mousePosition;

        mousePoint.x = Mathf.Clamp(mousePoint.x, 20, Camera.main.pixelWidth - 20);
        mousePoint.y = Mathf.Clamp(mousePoint.y, 20, Camera.main.pixelHeight - 20);
        mousePoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPosClamped() + mOffset;
    }

    private void OnMouseUp()
    {
        bsPointIDragEnded.Raise(Index);

        currentlyDragging = false;

        splinePointRenderer.MouseUpHappened(withinCollider);
    }

    public void OnPlayerHitTarget()
    {
        splinePointRenderer.OnPlayerHitTarget();
    }
}
