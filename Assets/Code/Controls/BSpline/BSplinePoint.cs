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
        mOffset = transform.position - GetMouseWorldPos();

        bsPointIDragStarted.Raise(Index);
    }

    private Vector3 GetMouseWorldPos()
    {
        var mousePoint = Input.mousePosition;

        mousePoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + mOffset;
    }

    private void OnMouseUp()
    {
        bsPointIDragEnded.Raise(Index);
    }
}
