using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace MustHave.Utilities
{
    public struct CoroutineUtils
    {
        public static IEnumerator ActionAfterPredicate(UnityAction action, Func<bool> predicate)
        {
            yield return new WaitWhile(predicate);
            action?.Invoke();
        }

        public static IEnumerator ActionAfterCustomYieldInstruction(UnityAction action, CustomYieldInstruction yieldInstruction)
        {
            yield return yieldInstruction;
            action?.Invoke();
        }

        public static IEnumerator ActionAfterTime(UnityAction action, float delayInSeconds)
        {
            yield return new WaitForSeconds(delayInSeconds);
            action?.Invoke();
        }

        public static IEnumerator ActionAfterFrames(UnityAction action, int framesNumber)
        {
            for (int i = 0; i < framesNumber; i++)
            {
                yield return new WaitForEndOfFrame();
            }
            action?.Invoke();
        }

        public static IEnumerator FixedUpdateRoutine(float duration, UnityAction<float, float> onUpdate)
        {
            yield return UpdateRoutine(duration, onUpdate, new WaitForFixedUpdate());
        }

        public static IEnumerator FixedUpdateRoutine(Func<bool> predicate, UnityAction<float> onUpdate)
        {
            yield return UpdateRoutine(predicate, onUpdate, new WaitForFixedUpdate());
        }

        public static IEnumerator UpdateRoutine(Func<bool> predicate, UnityAction<float> onUpdate, YieldInstruction yieldInstruction = null)
        {
            float startTime = Time.time;
            float elapsedTime = 0f;
            while (predicate())
            {
                onUpdate.Invoke(elapsedTime = Time.time - startTime);
                yield return yieldInstruction;
            }
        }

        public static IEnumerator UpdateRoutine(float duration, UnityAction<float, float> onUpdate, YieldInstruction yieldInstruction = null)
        {
            float startTime = Time.time;
            float elapsedTime = 0f;
            while ((elapsedTime = Time.time - startTime) < duration)
            {
                onUpdate.Invoke(elapsedTime, elapsedTime / duration);
                yield return yieldInstruction;
            }
        }

        public static IEnumerator UpdateRoutine(float duration, IEnumerator onStartRoutine, UnityAction<float, float> onUpdate, IEnumerator onEndRoutine)
        {
            if (onStartRoutine != null)
                yield return onStartRoutine;
            yield return UpdateRoutine(duration, onUpdate);
            if (onEndRoutine != null)
                yield return onEndRoutine;
        }

        public static IEnumerator UpdateRoutine(float duration, UnityAction onStart, UnityAction<float, float> onUpdate, UnityAction onEnd)
        {
            onStart?.Invoke();
            yield return UpdateRoutine(duration, onUpdate);
            onEnd?.Invoke();
        }
    }
}