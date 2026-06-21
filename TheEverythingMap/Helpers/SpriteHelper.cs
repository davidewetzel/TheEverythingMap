namespace TheEverythingMap.Helpers;

internal static class SpriteHelper
{
    public static Sprite CreateCircleSprite(int size)
    {
        Texture2D val = new(size, size, TextureFormat.ARGB32, false);
        Color[] array = new Color[size * size];
        float num2 = size / 2f;
        float num3 = size / 2f - 1f;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int num4 = i * size + j;
                float num5 = j - num2;
                float num6 = i - num2;
                if (num5 * num5 + num6 * num6 <= num3 * num3)
                {
                    array[num4] = Color.white;
                }
                else
                {
                    array[num4] = Color.clear;
                }
            }
        }
        val.SetPixels(array);
        val.Apply();

        return Sprite.Create(val, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f));
    }

    public static Sprite CreateClearSprite()
    {
        int size = 10;
        Texture2D val = new(size, size, TextureFormat.ARGB32, false);
        Color[] array = new Color[size * size];
        float num2 = size / 2f;
        float num3 = size / 2f - 1f;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int num4 = i * size + j;
                array[num4] = Color.clear;
            }
        }
        val.SetPixels(array);
        val.Apply();

        return Sprite.Create(val, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f));
    }

}
