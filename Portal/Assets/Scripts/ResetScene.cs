using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ResetScene : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        //this was made so if the player falls to the void (which they do a lot) they dont need to restart the whole ass game for it
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
