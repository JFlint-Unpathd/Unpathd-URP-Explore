using UnityEngine;

public class AnimGif : MonoBehaviour {

    [SerializeField] private Sprite[] frames;
    [SerializeField] private float fps = 10.0f;

    private SpriteRenderer spriteRenderer;

    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update () {
        int index = (int)(Time.time * fps);
        index = index % frames.Length;
        spriteRenderer.sprite = frames[index];
    }
}
