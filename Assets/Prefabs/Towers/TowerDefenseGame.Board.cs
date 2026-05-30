using UnityEngine;

public sealed partial class TowerDefenseGame
{
    private void BuildBoard()
    {
        // four lanes are built from simple runtime tiles
        float startX = -5.85f;
        float startY = 2.15f;
        float tileSpacingX = 1.12f;
        float laneSpacingY = 1.18f;

        for (int lane = 0; lane < LaneCount; lane++)
        {
            float y = startY - lane * laneSpacingY;
            CreateLaneStrip(lane, y);

            for (int column = 0; column < ColumnCount; column++)
            {
                float x = startX + column * tileSpacingX;
                GameObject tileObject = new GameObject("Tile L" + (lane + 1) + " C" + (column + 1));
                tileObject.transform.position = new Vector3(x, y, 0f);

                SpriteRenderer renderer = tileObject.AddComponent<SpriteRenderer>();
                renderer.sprite = squareSprite;
                renderer.color = (lane + column) % 2 == 0 ? new Color(0.2f, 0.43f, 0.32f) : new Color(0.18f, 0.36f, 0.28f);
                renderer.sortingOrder = -2;
                tileObject.transform.localScale = new Vector3(1.04f, 1.04f, 1f);

                BoxCollider2D collider = tileObject.AddComponent<BoxCollider2D>();
                collider.size = Vector2.one;

                TowerTileScript tileScript = tileObject.AddComponent<TowerTileScript>();
                tileScript.Initialize(lane, column, this);
                tiles[lane, column] = tileScript;
            }
        }
    }

    private void CreateLaneStrip(int lane, float y)
    {
        GameObject laneObject = new GameObject("Lane " + (lane + 1));
        laneObject.transform.position = new Vector3(-2.5f, y, 0.15f);
        laneObject.transform.localScale = new Vector3(12.7f, 1.08f, 1f);

        SpriteRenderer renderer = laneObject.AddComponent<SpriteRenderer>();
        renderer.sprite = squareSprite;
        renderer.color = lane % 2 == 0 ? new Color(0.11f, 0.26f, 0.21f) : new Color(0.1f, 0.22f, 0.19f);
        renderer.sortingOrder = -3;
    }

    private Sprite CreateSquareSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }
}
