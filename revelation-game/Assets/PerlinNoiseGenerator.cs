using UnityEngine;
using UnityEngine.UI;

public class PerlinNoiseGenerator : MonoBehaviour
{
    [Header("Terrain Parameters")]
    [SerializeField] private int width = 256;
    [SerializeField] private int height = 256;
    [SerializeField] private float scale = 20f;
    [SerializeField] private float amplitude = 10f;
    [SerializeField] private float frequency = 1f;
    [SerializeField] private float persistence = 0.5f;
    [SerializeField] private int octaves = 4;
    [SerializeField] private float islandDistanceDropoff = 4;

    public RawImage terrainDisplay;

    void Start()
    {
        DisplayPerlinTerrain();
    }

    private void Update()
    {
        DisplayPerlinTerrain();
    }

    void DisplayPerlinTerrain()
    {
        Texture2D terrainTexture = GenerateTerrainTexture();
        terrainDisplay.texture = terrainTexture;
    }

    Texture2D GenerateTerrainTexture()
    {
        Texture2D terrainTexture = new Texture2D(width, height);

        // Calculate the center of the image
        Vector2 center = new Vector2(width / 2f, height / 2f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width * scale;
                float yCoord = (float)y / height * scale;

                // Calculate distance from the current pixel to the center
                float distanceToCenter = Vector2.Distance(new Vector2(x, y), center) / islandDistanceDropoff;

                float clumpedValue = GeneratePerlinNoise(xCoord, yCoord);

                // Modulate elevation based on the distance to the center
                float elevation = Mathf.Pow(clumpedValue, 2) * amplitude * (1f - distanceToCenter / Mathf.Max(center.x, center.y));

                // Simulate different terrain features based on elevation

                // Apply elevation to color (you can customize this mapping)
                Color color = new Color(elevation, elevation, elevation);

                // Set pixel color in the texture
                terrainTexture.SetPixel(x, y, color);
            }
        }

        // Apply changes to the texture
        terrainTexture.Apply();

        return terrainTexture;
    }

    float GeneratePerlinNoise(float x, float y)
    {
        // Use Perlin noise function with adjustable parameters
        return Mathf.PerlinNoise(x * frequency, y * frequency) * persistence;
    }
}
