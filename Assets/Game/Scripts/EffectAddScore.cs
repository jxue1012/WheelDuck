using UnityEngine;
using DG.Tweening;
using TMPro;

public class EffectAddScore : MonoBehaviour
{
    
    public TextMeshPro tScore;

    public void Show(int score) {
        tScore.text = score.ToString();
        this.transform.position = GameCenter.Instance.player.transform.position;

        tScore.alpha = 0;
        this.transform.localScale = Vector3.zero;

        Vector3 randPoint = Random.insideUnitCircle;
        var dir = (randPoint - this.transform.position).normalized;
        var targetPos = this.transform.position + dir;

        var seq = DOTween.Sequence();
        seq.Append(transform.DOScale(1f, 0.5f))
        .Join(tScore.DOFade(1f, 0.5f))
        .Join(transform.DOMove(targetPos, 1.5f).SetEase(Ease.OutCirc))
        .Insert(1f, tScore.DOFade(0f, 0.5f))
        .Insert(1f, transform.DOScale(0f, 0.5f))
        .OnComplete(() => GameObject.Destroy(this.gameObject))
   
        .Play();
    }

}
