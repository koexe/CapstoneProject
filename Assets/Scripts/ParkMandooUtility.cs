using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;

public static class ParkMandooUtility 
{
    public static Rect GetOverlapRect(Rect rectA, Rect rectB)
    {
        if (rectA.Overlaps(rectB))
        {
            float minX = Mathf.Max(rectA.xMin, rectB.xMin);
            float maxX = Mathf.Min(rectA.xMax, rectB.xMax);
            float minY = Mathf.Max(rectA.yMin, rectB.yMin);
            float maxY = Mathf.Min(rectA.yMax, rectB.yMax);

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        // 겹치는 게 없으면 크기 0짜리 Rect 반환
        return Rect.zero;
    }
}


public static class CoroutineUtil
{
    // 공용 객체 (불변이므로 여러 코루틴에서 공유 가능)
    public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
    public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();

    // 초 단위로 캐시된 WaitForSeconds
    private static Dictionary<float, WaitForSeconds> waitForSecondsCache = new Dictionary<float, WaitForSeconds>();

    /// <summary>
    /// 지정된 시간만큼 대기하는 WaitForSeconds 캐시 반환
    /// </summary>
    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        if (!waitForSecondsCache.TryGetValue(seconds, out var wait))
        {
            wait = new WaitForSeconds(seconds);
            waitForSecondsCache[seconds] = wait;
        }
        return wait;
    }

    public static Task ToTask(this MonoBehaviour _monoBehaviour, IEnumerator _coroutine, CancellationToken _cancellationToken = default, int _timeoutMilliseconds = -1)
    {
        var tcs = new TaskCompletionSource<object>();
        Coroutine runner = _monoBehaviour.StartCoroutine(RunCoroutine(_coroutine, tcs, _cancellationToken));

        // 타임아웃 설정
        if (_timeoutMilliseconds > 0)
        {
            _ = TimeoutCoroutine(tcs, _timeoutMilliseconds);
        }

        // 취소 설정
        if (_cancellationToken != default)
        {
            _cancellationToken.Register(() => {
                if (!tcs.Task.IsCompleted)
                {
                    tcs.TrySetCanceled();
                    if (runner != null)
                    {
                        _monoBehaviour.StopCoroutine(runner);
                    }
                }
            });
        }

        return tcs.Task;
    }

    private static IEnumerator RunCoroutine(IEnumerator _coroutine, TaskCompletionSource<object> _tcs, CancellationToken _cancellationToken)
    {
        while (true)
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            if (_coroutine.MoveNext())
            {
                yield return _coroutine.Current;
            }
            else
            {
                _tcs.TrySetResult(null);
                yield break;
            }
        }
    }

    private static async Task TimeoutCoroutine(TaskCompletionSource<object> _tcs, int _timeoutMilliseconds)
    {
        await Task.Delay(_timeoutMilliseconds);
        if (!_tcs.Task.IsCompleted)
        {
            _tcs.TrySetException(new TaskCanceledException("Coroutine timed out"));
        }
    }
}
