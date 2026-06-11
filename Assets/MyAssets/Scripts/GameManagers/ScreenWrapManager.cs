using UnityEngine;

public class ScreenWrapManager : Singleton<ScreenWrapManager>
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float padding = 0.5f;

    public float LeftBound { get; private set; }
    public float RightBound { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        if(targetCamera == null) 
            targetCamera = Camera.main;
        
        CalculateBounds();
    }

    private void CalculateBounds()
    {
        float camHeight = targetCamera.orthographicSize;
        float camWidth = camHeight * targetCamera.aspect;

        LeftBound = targetCamera.transform.position.x - camWidth - padding;
        RightBound = targetCamera.transform.position.x + camWidth + padding;
    }

    public Vector3 WrapPosition(Vector3 position)
    {
        if(position.x > RightBound)
            position.x =  LeftBound;
        
        else if(position.x < LeftBound)
            position.x = RightBound;
        
        return position;
    }
}
