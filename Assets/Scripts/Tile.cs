using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private GameObject highlight;

    public bool isWalkable = true;
    public int x, y;

    // A* Pathfinding properties
    public int GCost { get; set; }
    public int HCost { get; set; }
    public int FCost => GCost + HCost;
    public Tile Parent { get; set; }

    

    private void OnMouseEnter()
    {
        highlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        highlight.SetActive(false);
    }

    public void SetColor(Color color)
    {
        renderer.color = color;
    }

    public void SetMaterial(Material material)
    {
        renderer.material = material;
    }
}
