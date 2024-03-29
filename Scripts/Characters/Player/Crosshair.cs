using UnityEngine;

public class Crosshair : Singleton<Crosshair>
{
    private Vector2 crosshairPosition;
    public Vector2 position {
        get => crosshairPosition;
    }
    private SpriteRenderer spriteRend;
    public float spriteY {
        get => spriteRend.size.y;
    }
    private Vector2 playerPos;
    private Vector2 screenSize;
    protected override void Awake() {
        base.Awake();
        Cursor.visible = false;
        spriteRend = this.GetComponentInChildren<SpriteRenderer>();
        // Game resolution cannot be changed while playing, as it lets the player move the crosshair out of the borders
        screenSize = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
    }

    public void ResizeToScreen() {
        Vector2 newSize = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        if(newSize != screenSize) {
            screenSize = newSize;
        }
    }

    void Update() {
        playerPos = transform.parent.position;
        // Get the mouse position
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        crosshairPosition = mousePosition;
        // Prevent the crosshair from leaving the screen 
        float minX = playerPos.x - screenSize.x + spriteRend.size.x / 2;
        float maxX = playerPos.x + screenSize.x - spriteRend.size.x / 2;
        float minY = playerPos.y - screenSize.y + spriteRend.size.y / 2;
        float maxY = playerPos.y + screenSize.y - spriteRend.size.y / 2;
        if(mousePosition.x < minX) crosshairPosition.x = minX;
        if(mousePosition.x > maxX) crosshairPosition.x = maxX;
        if(mousePosition.y < minY) crosshairPosition.y = minY;
        if(mousePosition.y > maxY) crosshairPosition.y = maxY;
        /*Debug.Log("Screen pos: " + screenSize);
        Debug.Log("Mouse pos: " + mousePosition);
        Debug.Log("Player pos: " + playerPos);*/
        // Set the crosshair's position to camera position
        transform.position = crosshairPosition;
    }
}