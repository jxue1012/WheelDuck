using UnityEngine;
using DG.Tweening;

public class DrumInstance : MonoBehaviour
{

    public Transform Sprite;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var dir = (Vector3.zero - this.transform.parent.position).normalized;

        Vector2 force = dir * GameCenter.Instance.gameData.DrumForce;

        GameCenter.Instance.player.AddForce(force);

        GameCenter.Instance.AddScore(GameCenter.Instance.gameData.DrumScore);

        Sprite.DOScale(1.1f, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            Sprite.DOScale(1f, 0.25f).SetEase(Ease.InOutElastic);
        });

    }
}
