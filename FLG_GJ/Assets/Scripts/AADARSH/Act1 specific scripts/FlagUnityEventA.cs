// Can be in its own file or at the top of the StoryManager script
using UnityEngine.Events;
using System;
using UnityEngine;

[Serializable]
public class FlagUnityEventA {
    public string flagName;
    [Tooltip("Called when this flag is set to TRUE.")]
    public UnityEvent OnSetTrue;
    [Tooltip("Called when this flag is set to FALSE.")]
    public UnityEvent OnSetFalse;
}