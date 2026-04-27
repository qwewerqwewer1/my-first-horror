using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ghost"))
        {
            Die();
        }
    }

    void Die()
    {
        // Пока просто перезагружаем сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}