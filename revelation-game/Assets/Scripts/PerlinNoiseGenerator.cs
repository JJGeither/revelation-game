using UnityEngine;
using UnityEngine.UI;

public class PerlinNoiseGenerator : MonoBehaviour
{
    public enum GenerationMode
    {
        Terrain,
        Tree
    }


    [Header("Terrain Parameters")]
    [SerializeField] private int width = 256;
    [SerializeField] private int height = 256;
    [SerializeField] private float scale = 20f;
    [SerializeField] private float amplitude = 10f;
    [SerializeField] private float frequency = 1f;
    [SerializeField] private float persistence = 0.5f;
    [SerializeField] private int octaves = 4;
    [SerializeField] private float islandDistanceDropoff = 4;
    [SerializeField] private float waterLevel = .1f;
    int newNoise;
    public RawImage perlinImage;
    public GenerationMode generationMode = GenerationMode.Terrain;

    void Start()
    {
        newNoise = Random.Range(0, 10000);
        switch (generationMode)
        {
            case GenerationMode.Terrain:
                DisplayPerlinTerrain();
                break;
            case GenerationMode.Tree:
                DisplayPerlinTree();
                break;
            default:
                Debug.LogError("Unknown generation mode");
                break;
        }
    }

    private void Update()
    {
        switch (generationMode)
        {
            case GenerationMode.Terrain:
                DisplayPerlinTerrain();
                break;
            case GenerationMode.Tree:
                DisplayPerlinTree();
                break;
            default:
                Debug.LogError("Unknown generation mode");
                break;
        }
    }

    public void DisplayPerlinTree()
    {
        Texture2D treeTexture = GenerateTreeTexture();
        perlinImage.texture = treeTexture;
    }

    public void DisplayPerlinTree(int newNoise)
    {
        this.newNoise = newNoise;
        Texture2D treeTexture = GenerateTreeTexture();
        perlinImage.texture = treeTexture;
    }

    public void DisplayPerlinTerrain()
    {

        Texture2D terrainTexture = GenerateTerrainTexture();
        perlinImage.texture = terrainTexture;
    }

    public void DisplayPerlinTerrain(int newNoise)
    {
        this.newNoise = newNoise;
        Texture2D terrainTexture = GenerateTerrainTexture();
        perlinImage.texture = terrainTexture;
    }

    Texture2D GenerateTreeTexture()
    {
        Texture2D terrainTexture = new Texture2D(width, height);

        // Calculate the center of the image
        Vector2 center = new Vector2(width / 2f, height / 2f);
        Color[,] colors = new Color[width, height];

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
                elevation = Mathf.Round(elevation); // Round to the nearest integer (0 or 1)

                // Apply elevation to color (you can customize this mapping)
                colors[x, y] += new Color(0, elevation, 0);

                // Set pixel color in the texture
                terrainTexture.SetPixel(x, y, colors[x, y]);
            }
        }

        // Apply changes to the texture
        terrainTexture.Apply();

        return terrainTexture;
    }


    Texture2D GenerateTerrainTexture()
    {
        Texture2D terrainTexture = new Texture2D(width, height);

        // Calculate the center of the image
        Vector2 center = new Vector2(width / 2f, height / 2f);
        Color[,] colors = new Color[width, height];

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
                elevation = Mathf.Round(elevation * 10.0f) * 0.1f;

                // Apply elevation to color (you can customize this mapping)
                colors[x,y] += new Color(0, elevation, 0);

                if (elevation < waterLevel)
                {
                    colors[x,y] = new Color(0, 0, 1);
                    SetAdjacentElevation(x, y, colors, terrainTexture);
                }

                // Set pixel color in the texture
                terrainTexture.SetPixel(x, y, colors[x, y]);
            }
        }

        // Apply changes to the texture
        terrainTexture.Apply();

        return terrainTexture;
    }

    void SetAdjacentElevation(int x, int y, Color[,] colors, Texture2D terrainTexture)
    {
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i >= 0 && i < width && j >= 0 && j < height && colors[i,j].b != 1)
                {
                    colors[i, j] += new Color(1, 0, 0);
                    terrainTexture.SetPixel(i, j, colors[i, j]);
                }
            }
        }
    }


    float GeneratePerlinNoise(float x, float y)
    {

        // Use Perlin noise function with adjustable parameters
        return Mathf.PerlinNoise((x + newNoise) * frequency, (y + newNoise) * frequency) * persistence;
    }
}
