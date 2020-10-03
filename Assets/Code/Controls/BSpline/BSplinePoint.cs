using ScriptableObjectArchitecture;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BSplinePoint : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;

    public int Index { get; private set; }

    // game events for mouse drag begun (i)... mouse drag ended (i)
    [SerializeField] private IntGameEvent bsPointIDragStarted;

    [SerializeField] private IntGameEvent bsPointIDragEnded;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetIndex(int index)
    {
        Index = index;
    }

    private void OnMouseEnter()
    {
        SetClickableRender();
    }

    private void SetClickableRender()
    {
    }

    private void OnMouseExit()
    {
        SetNotClickableRender();
    }

    private void SetNotClickableRender()
    {
    }

    private void OnMouseDown()
    {
        mZCoord = Camera.main.ScreenToWorldPoint(transform.position).z;
        mOffset = transform.position - GetMouseWorldPosClamped();

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
    }
}
