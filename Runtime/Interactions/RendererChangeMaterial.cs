using UnityEngine;

public class RendererChangeMaterial : MonoBehaviour
{

    [SerializeField] private new Renderer renderer;

    public void ChangeMaterial(Material material)
    {
        renderer.material = material;
    }
    
}