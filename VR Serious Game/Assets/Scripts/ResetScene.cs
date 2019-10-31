using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //this.gameObject.GetComponentInChildren<GameObject>().SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay(Collider otherCollider)
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick))
        {
            UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            UnityEngine.SceneManagement.SceneManager.LoadScene(scene.name);
        }
    }
}
