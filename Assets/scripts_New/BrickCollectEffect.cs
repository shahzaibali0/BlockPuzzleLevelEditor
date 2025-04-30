using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BrickCollectEffect : MonoBehaviour
{
    [Header("Brick Settings")]
    public BrickPool brickPool;
    public Transform BrickStart;
    public Transform BrickEnd;

    [Header("Animation Settings")]
    public int BrickAmount = 100;
    public float totalDelay = 0.5f;
    public float moveDuration = 0.8f;
    public Ease moveEase = Ease.OutQuad;
    public Ease scaleEase = Ease.OutBack;

    private float coinPerDelay;
    private int finishedCount;
    private List<SingleBrickUi> activeBricks = new List<SingleBrickUi>();
    public BrickType CurrentType;

    private void Start()
    {
        brickPool.Initialize();
    }

    [Button(ButtonSizes.Medium)]
    public void OnGetButtonClicked()
    {
        finishedCount = 0;
        coinPerDelay = totalDelay / BrickAmount;

        // Return all active bricks to pool first
        foreach (var brick in activeBricks)
        {
            brickPool.Return(brick);
        }
        activeBricks.Clear();

        for (int i = 0; i < BrickAmount; i++)
        {
            float targetDelay = i * coinPerDelay;
            ShowCoin(targetDelay);
        }
    }

    private void ShowCoin(float delay)
    {
        var brickObject = brickPool.Get();
        activeBricks.Add(brickObject);

        // Random offset
        Vector3 offset = new Vector3(
            Random.Range(-100f, 100f),
            Random.Range(-100f, 100f),
            0f
        );

        // Set brick icon
        BricksIcons bricksIcons = DataManager.Instance.buildingsData.bricksIcons
            .FirstOrDefault(b => b.BrickType == CurrentType);
        if (bricksIcons != null)
        {
            brickObject.Brick.sprite = bricksIcons.Icon;
        }

        // Convert world position to screen position
        Vector3 startPos = Camera.main.WorldToScreenPoint(BrickStart.position) + offset;
        brickObject.transform.position = startPos;
        brickObject.transform.localScale = Vector3.one * 0.1f;

        // Animate scale and move
        brickObject.transform.DOScale(Vector3.one, moveDuration).SetDelay(delay).SetEase(scaleEase);
        brickObject.transform.DOMove(BrickEnd.position, moveDuration).SetDelay(delay).SetEase(moveEase).OnComplete(() =>
        {
            finishedCount++;
            brickPool.Return(brickObject);
            activeBricks.Remove(brickObject);

            if (finishedCount >= BrickAmount)
            {
                OnAnimationDone();
            }
        });
    }

    private void OnAnimationDone()
    {
        Debug.Log("All brick animations completed!");
        // You can trigger sound, score updates, or UI changes here
    }
}