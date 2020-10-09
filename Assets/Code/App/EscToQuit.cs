using UnityEngine;

// and now other stuff
public class EscToQuit : MonoBehaviour
{
#if UNITY_EDITOR

    [SerializeField] private int frameRateTest = -1;
#endif

    //[SerializeField] private GameObject actualCursor;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        if (frameRateTest > 0)
        {
            Application.targetFrameRate = frameRateTest;
        }
#endif
        //Cursor.visible = false;
        //Cursor.SetCursor()
    }

    // Update is called once per frame
    void Update()
    {
        /*
        var worldPointBadZ = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        actualCursor.transform.position = new Vector3(
            worldPointBadZ.x,
            worldPointBadZ.y,
            0);
            */

        if (Application.platform != RuntimePlatform.WebGLPlayer
            && Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
