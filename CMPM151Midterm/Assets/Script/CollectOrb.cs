using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityOSC;

public class CollectOrb : MonoBehaviour
{
    public int score = 0;
    public Text Score;
    // Start is called before the first frame update
    void Start()
    {
        Score.text = "Coin Collected: " + score;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Collect")) {
            // Debug.Log("Coin Collect");

            //************* Send the message to the client...
            OSCHandler.Instance.SendMessageToClient ("pd", "/unity/trigger", score);
            //*************
            score++;
            Score.text = "Coin Collected: " + score + "/10";
            Destroy(other.gameObject);

        }
    }
}
