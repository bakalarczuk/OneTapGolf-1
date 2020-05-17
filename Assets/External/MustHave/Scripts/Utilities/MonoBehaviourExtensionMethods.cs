using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace MustHave.Utilities
{
    public static class MonoBehaviourExtensionMethods
    {
        public static Coroutine StartCoroutineActionAfterTime(this MonoBehaviour mono, UnityAction action, float delayInSeconds)
        {
            return mono.StartCoroutine(CoroutineUtils.ActionAfterTime(action, delayInSeconds));
        }

        public static Coroutine StartCoroutineActionAfterFrames(this MonoBehaviour mono, UnityAction action, int framesNumber)
        {
            return mono.StartCoroutine(CoroutineUtils.ActionAfterFrames(action, framesNumber));
        }

        public static Coroutine StartCoroutineActionAfterPredicate(this MonoBehaviour mono, UnityAction action, Func<bool> predicate)
        {
            return mono.StartCoroutine(CoroutineUtils.ActionAfterPredicate(action, predicate));
        }

        public static Coroutine StartCoroutineActionAfterCustomYieldInstruction(this MonoBehaviour mono, UnityAction action, CustomYieldInstruction yieldInstruction)
        {
            return mono.StartCoroutine(CoroutineUtils.ActionAfterCustomYieldInstruction(action, yieldInstruction));
        }

        public static Coroutine StartUpdateCoroutine(this MonoBehaviour mono, float duration, UnityAction<float, float> onUpdate)
        {
            return mono.StartCoroutine(CoroutineUtils.UpdateRoutine(duration, onUpdate));
        }

        public static Coroutine StartUpdateCoroutine(this MonoBehaviour mono, float duration, IEnumerator onStartRoutine, UnityAction<float, float> onUpdate, IEnumerator onEndRoutine)
        {
            return mono.StartCoroutine(CoroutineUtils.UpdateRoutine(duration, onStartRoutine, onUpdate, onEndRoutine));
        }

        public static Coroutine StartUpdateCoroutine(this MonoBehaviour mono, float duration, UnityAction onStart, UnityAction<float, float> onUpdate, UnityAction onEnd)
        {
            return mono.StartCoroutine(CoroutineUtils.UpdateRoutine(duration, onStart, onUpdate, onEnd));
        }
    }
}
