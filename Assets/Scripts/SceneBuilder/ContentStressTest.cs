using System.Collections;
using UnityEngine;

public class ContentStressTest : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds5 = new(5f);
    [SerializeField] private ContentLoader contentLoader;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        while (true)
        {
            yield return _waitForSeconds5; // Wait for 5 seconds before loading content again
            float loadTimeA = contentLoader.LoadContent("PageA_500");
            Debug.Log($"Loaded PageA in {loadTimeA} ms");

            yield return _waitForSeconds5; // Wait for 5 seconds before loading content again
            float loadTimeB = contentLoader.LoadContent("PageB_500");
            Debug.Log($"Loaded PageB in {loadTimeB} ms");
        }
    }
}
