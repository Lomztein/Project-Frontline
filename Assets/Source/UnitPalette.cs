using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;

public class UnitPalette
{
    public static string STEEL_PALETTE_PATH = "Textures/Palettes/steel";
    public static string BASE_MATERIAL_PATH = "Materials/UnitBodyBaseMaterial";
    public static Texture2D GetSteelPalette() => Resources.Load<Texture2D>(STEEL_PALETTE_PATH);
    public static Material GetBaseMaterial() => Resources.Load<Material>(BASE_MATERIAL_PATH);

    public Texture2D PaletteTexture { get; private set; }
    public Material UnitBodyMaterial { get; private set; }

    private UnitPalette(Texture2D palette, Material material)
    {
        PaletteTexture = palette;
        UnitBodyMaterial = material;
    }

    public static UnitPalette GeneratePalette (Texture2D factionPalette, Texture2D teamPalette)
    {
        Texture2D texture = ConcatonateHorizontally(factionPalette, teamPalette, GetSteelPalette());
        Material material = GenerateUnitBodyMaterial(factionPalette, teamPalette);
        return new UnitPalette(texture, material);
    }

    private static Material GenerateUnitBodyMaterial(Texture2D factionPalette, Texture2D teamPalette)
    {
        Texture2D texture = ConcatonateHorizontally(factionPalette, teamPalette, GetSteelPalette());
        Material newMaterial = Object.Instantiate(GetBaseMaterial());
        newMaterial.mainTexture = texture;
        return newMaterial;
    }

    private static Texture2D ConcatonateHorizontally(params Texture2D[] textures)
    {
        Assert.IsTrue(textures.Length > 0, "Texture array cannot be empty.");
        // First, ensure all textures are of equal height.
        int height = textures[0].height;
        Assert.IsTrue(textures.All(x => x.height == height));
        int width = textures.Sum(x => x.width);

        int horOffset = 0;
        Texture2D newTexture = new Texture2D(width, height);
        for (int i = 0; i < textures.Length; i++)
        {
            Texture2D cur = textures[i];

            for (int x = 0; x < cur.width; x++)
            {
                for (int y = 0; y < cur.height; y++)
                {
                    newTexture.SetPixel(x + horOffset, y, cur.GetPixel(x, y));
                }
            }

            horOffset += cur.width;
        }

        newTexture.Apply();
        return newTexture;
    }
}
