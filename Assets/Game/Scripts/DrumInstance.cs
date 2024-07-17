using UnityEngine;
using DG.Tweening;

public class DrumInstance : MonoBehaviour
{
    public GameObject hitPrefab;
    public AudioClip hitSound;
    public Transform Sprite;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 collisionPosition = other.transform.position;

            var dir = (Vector3.zero - this.transform.parent.position).normalized;

            Vector2 force = dir * GameCenter.Instance.gameData.DrumForce;

            GameCenter.Instance.player.AddForce(force);

            GameCenter.Instance.AddScore(GameCenter.Instance.gameData.DrumScore);

            Sprite.DOScale(1.1f, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                Sprite.DOScale(1f, 0.25f).SetEase(Ease.InOutElastic);
            });

            InstantiateHitPrefab(collisionPosition);
            PlayHitSound(collisionPosition);
        }

    }

    private void InstantiateHitPrefab(Vector3 position)
    {
        GameObject hitInstance = Instantiate(hitPrefab, position, Quaternion.identity);
        Animator hitAnimator = hitInstance.GetComponent<Animator>();
        if (hitAnimator != null)
        {
            hitAnimator.Play("HitAnimation"); // Replace "HitAnimation" with the actual animation name
        }

        // Optionally, destroy the hit prefab after the animation is done
        Destroy(hitInstance, hitAnimator.GetCurrentAnimatorStateInfo(0).length);
    }

    private void PlayHitSound(Vector3 position)
    {
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, position);
        }
    }
}
