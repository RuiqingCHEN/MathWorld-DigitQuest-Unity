using System.Collections;
using UnityEngine;

// Method1
public class BounceEffect : MonoBehaviour
{
    public float bounceHeight = 0.3f;
    public float bounceDuration = 0.4f;
    public int bounceCount = 2;

    public void StartBounce()
    {
        StartCoroutine(BounceHandler());
    }

    private IEnumerator BounceHandler()
    {
        Vector3 startPosition = transform.position;
        float localHeight = bounceHeight;
        float localDuration = bounceDuration;

        for(int i = 0; i < bounceCount; i++)
        {
            yield return Bounce(startPosition, localHeight, localDuration / 2);
            localHeight *= 0.5f;
            localDuration *= 0.8f;
        }

        transform.position = startPosition;
    }

    private IEnumerator Bounce(Vector3 start, float height, float duration)
    {
        Vector3 peak = start + Vector3.up * height;
        float elapsed = 0f;
        // Move upwards
        while(elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, peak, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;

        // Move Downwards
        while(elapsed < duration)
        {
            transform.position = Vector3.Lerp(peak, start, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}

// Method2
/*
public class BounceEffect : MonoBehaviour
{
    public AnimationCurve jumpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float jumpHeight = 0.3f;
    public float jumpDuration = 0.4f;
    public float dropRadius = 0.5f;

    public void StartBounce(Vector2 mousePosition)
    {
        Vector2 startPosition = transform.position;
        Vector2 mouseDirection = (mousePosition - startPosition).normalized;
        Vector2 randomOffset = mouseDirection * dropRadius;
        Vector2 endPosition = startPosition + randomOffset;
        
        StartCoroutine(JumpRoutine(startPosition, endPosition));
    }
    
    private IEnumerator JumpRoutine(Vector2 startPosition, Vector2 endPosition)
    {
        float timePassed = 0f;
        while (timePassed < jumpDuration)
        {
            timePassed += Time.deltaTime;
            float linearOverTime = timePassed / jumpDuration;
            float heightOverTime = jumpCurve.Evaluate(linearOverTime);
            float height = Mathf.Lerp(0f, jumpHeight, heightOverTime);
            
            transform.position = Vector2.Lerp(startPosition, endPosition, linearOverTime) + new Vector2(0f, height);
            yield return null;
        }
        
        transform.position = endPosition; 
    }
}
*/