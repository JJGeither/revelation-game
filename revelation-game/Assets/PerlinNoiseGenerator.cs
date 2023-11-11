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

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width * scale;
                float yCoord = (float)y / height * scale;

                float clumpedValue = GeneratePerlinNoise(xCoord, yCoord);

                // Apply elevation to color with increased contrast
                float elevation = Mathf.Pow(clumpedValue, 2) * amplitude; // Apply a power function (you can experiment with the exponent)

                // Simulate different terrain features based on elevation


                // Apply elevation to color (you can customize this mapping)
                Color color = new Color(elevation , elevation, elevation);

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
