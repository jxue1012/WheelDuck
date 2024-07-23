using UnityEngine;

public class FoodControl : MonoBehaviour
{
    public GameObject eatPrefab;
    public AudioClip eatSound;
    public ParticleSystem windParticleSystem;
    public ParticleSystem stoneParticleSystem;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 collisionPosition = transform.position;

            if (GameCenter.Instance.GameStatus == 1)
            {
                GameCenter.Instance.AddScore(GameCenter.Instance.gameData.FoodScore);
                GameCenter.Instance.floorControl.IncreaseFloorSpeed(GameCenter.Instance.gameData.FloorSpeedInc);
                Debug.Log("Speed = " + GameCenter.Instance.floorControl.Speed);
                Hide();
                GameCenter.Instance.floorControl.ShowFood();

                IncreaseWindRateOverTime(5);
                IncreaseStoneStartSpeed(5);
            }
            else
            {
                GameCenter.Instance.floorControl.ShowFood();
            }

            InstantiateEatPrefab(collisionPosition);
            PlayEatSound(collisionPosition);
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

    private void InstantiateEatPrefab(Vector3 position)
    {
        GameObject eatInstance = Instantiate(eatPrefab, position, Quaternion.identity);
        Animator eatAnimator = eatInstance.GetComponent<Animator>();
        if (eatAnimator != null)
        {
            eatAnimator.Play("EatAnimation"); // Replace "EatAnimation" with the actual animation name
        }

        // Optionally, destroy the eat prefab after the animation is done
        Destroy(eatInstance, eatAnimator.GetCurrentAnimatorStateInfo(0).length);
    }

    private void PlayEatSound(Vector3 position)
    {
        if (eatSound != null)
        {
            AudioSource.PlayClipAtPoint(eatSound, position);
        }
    }

    private void IncreaseWindRateOverTime(float amount)
    {
        if (windParticleSystem != null)
        {
            var emission = windParticleSystem.emission;
            emission.rateOverTime = new ParticleSystem.MinMaxCurve(emission.rateOverTime.constant + amount);
        }
    }

    private void IncreaseStoneStartSpeed(float amount)
    {
        if (stoneParticleSystem != null)
        {
            var mainModule = stoneParticleSystem.main;
            mainModule.startSpeed = new ParticleSystem.MinMaxCurve(mainModule.startSpeed.constant + amount);
        }
    }
}
