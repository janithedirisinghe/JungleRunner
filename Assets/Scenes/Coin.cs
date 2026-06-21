using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 5;

    void Update()
    {
        transform.Rotate(0f, 90f * Time.deltaTime, 0f);   // gentle spin
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddScore(value);
            gameObject.SetActive(false);   // collected
        }
    }
}