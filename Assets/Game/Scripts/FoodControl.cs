using UnityEngine;

public class FoodControl : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameCenter.Instance.GameStatus == 1)
            {
                GameCenter.Instance.AddScore(GameCenter.Instance.gameData.FoodScore);
                GameCenter.Instance.floorControl.IncreaseFloorSpeed(GameCenter.Instance.gameData.FloorSpeedInc);
                Debug.Log("Speed = " + GameCenter.Instance.floorControl.Speed);
                Hide();
                GameCenter.Instance.floorControl.ShowFood();
            }
            else
            {
                GameCenter.Instance.floorControl.ShowFood();
            }


        }
    }

    public void Show(Vector3 position)
    {
        this.gameObject.SetActive(true);
        this.transform.position = position + new Vector3(0, 0, -1f);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

}
