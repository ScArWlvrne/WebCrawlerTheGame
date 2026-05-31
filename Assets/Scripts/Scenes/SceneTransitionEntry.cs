using System.Collections;
using UnityEngine;

public class SceneTransitionEntry : MonoBehaviour
{
    [SerializeField] private CircleHoleTransition transition;

    private IEnumerator Start()
    {
        transition.SetClosed();

        yield return null;

        yield return transition.Open();
    }
}