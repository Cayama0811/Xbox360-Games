using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public GameObject Xbox360Text;
    public GameObject StartText;
    public GameObject EnterText;

	// Use this for initialization
	void Start () {
        #if UNITY_XBOX360
        Xbox360Text.SetActive(true);
        StartText.SetActive(true);
        EnterText.SetActive(false);
        #else
        Xbox360Text.SetActive(false);
        StartText.SetActive(false);
        EnterText.SetActive(true);
        #endif
    }
	
	// Update is called once per frame
	void Update () {
        #if UNITY_XBOX360
        if (Input.GetKey(KeyCode.JoystickButton7))
        {
            SceneManager.LoadScene("PortalMainScene");
        }
        #else
        if (Input.GetKey(KeyCode.Return))
        {
            SceneManager.LoadScene("PortalMainScene");
        }
        #endif
	}
}
