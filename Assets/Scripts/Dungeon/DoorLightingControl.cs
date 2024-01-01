using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class DoorLightingControl : MonoBehaviour
{
    private bool isLit = false;
    private Door door;

    private void Awake()
    {
        // Get components：获取组件
        door = GetComponentInParent<Door>();
    }

    /// <summary>
    /// Fade in door：淡入门
    /// </summary>
    public void FadeInDoor(Door door)
    {
        // Create new material to fade in：创建要淡入的新材质
        Material material = new Material(GameResources.Instance.variableLitShader);

        if (!isLit)
        {
            SpriteRenderer[] spriteRendererArray = GetComponentsInParent<SpriteRenderer>();

            foreach (SpriteRenderer spriteRenderer in spriteRendererArray)
            {
                StartCoroutine(FadeInDoorRoutine(spriteRenderer, material));
            }
            isLit = true;
        }
    }

    /// <summary>
    /// Fade in door coroutine：淡入门协程
    /// </summary>
    private IEnumerator FadeInDoorRoutine(SpriteRenderer spriteRenderer, Material material) //进入房间时，逐渐点亮房间
    {
        spriteRenderer.material = material;

        for (float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Slider", i);
            yield return null;

        }
        spriteRenderer.material = GameResources.Instance.litMaterial;
    }

    // Fade door in if triggered：如果触发，则将门淡入
    private void OnTriggerEnter2D(Collider2D collision)
    {
        FadeInDoor(door);
    }
}
