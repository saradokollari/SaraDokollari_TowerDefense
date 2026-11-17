using UnityEngine;
using UnityEngine.Rendering;

public class BuildSpotHalo : MonoBehaviour
{
    [SerializeField] private Material haloMaterial;
    [SerializeField] private Color haloColor =
        new Color(0.1f, 0.8f, 1f, 1f);

    [SerializeField] private float size = 1.4f;
    [SerializeField, Range(0f, 1f)] private float opacity = 0.65f;
    [SerializeField] private float flickerSpeed = 2f;

    private GameObject halo;
    private Material runtimeMaterial;
    private Texture2D texture;
    private float timeOffset;

    private void Start()
    {
        if (haloMaterial == null)
        {
            Debug.LogError("Assign the halo material.", this);
            enabled = false;
            return;
        }

        const int resolution = 64;

        texture = new Texture2D(
            resolution, resolution, TextureFormat.RGBA32, false
        );

        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                Vector2 point = new Vector2(
                    x / (resolution - 1f) * 2f - 1f,
                    y / (resolution - 1f) * 2f - 1f
                );

                float distance = Mathf.Clamp01(point.magnitude);
                float alpha = Mathf.SmoothStep(1f, 0f, distance);

                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();

        runtimeMaterial = new Material(haloMaterial);
        runtimeMaterial.SetTexture("_BaseMap", texture);

        halo = GameObject.CreatePrimitive(PrimitiveType.Quad);
        halo.name = "Local Halo";
        halo.transform.SetParent(transform, false);
        halo.transform.localPosition = new Vector3(0f, -0.01f, 0f);
        halo.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        Collider collider = halo.GetComponent<Collider>();
        collider.enabled = false;
        Destroy(collider);

        MeshRenderer renderer = halo.GetComponent<MeshRenderer>();
        renderer.sharedMaterial = runtimeMaterial;
        renderer.shadowCastingMode = ShadowCastingMode.Off;
        renderer.receiveShadows = false;

        timeOffset = Random.Range(0f, 100f);
        UpdateHalo();
    }

    private void Update()
    {
        if (halo != null)
            UpdateHalo();
    }

    private void UpdateHalo()
    {
        float time = Time.unscaledTime * flickerSpeed + timeOffset;
        float flicker = Mathf.PerlinNoise(time, 0.5f);
        float pulse = 0.5f + 0.5f * Mathf.Sin(time * 2f);

        Color color = haloColor;
        color.a = opacity * Mathf.Lerp(0.45f, 1f, flicker);
        runtimeMaterial.SetColor("_BaseColor", color);

        float width = size * Mathf.Lerp(0.95f, 1.05f, pulse);
        halo.transform.localScale = new Vector3(width, width, 1f);
    }

    private void OnDestroy()
    {
        if (runtimeMaterial != null)
            Destroy(runtimeMaterial);

        if (texture != null)
            Destroy(texture);
    }
}