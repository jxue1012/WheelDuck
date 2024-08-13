using UnityEngine;

public class SfxDie : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("Destroy", 1.5f);
    }

    public void Destroy()
    {
        GameObject.Destroy(this.gameObject);
    }

}
