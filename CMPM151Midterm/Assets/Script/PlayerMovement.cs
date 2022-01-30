using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityOSC;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Rigidbody rb;
    private float directionY;

    // Variable for collecting and showing orb
    public int score = 0;
    public Text Score;

    //************* Need to setup this server dictionary...
	Dictionary<string, ServerLog> servers = new Dictionary<string, ServerLog> ();
	//*************
    
    // Variable for movement
    float moveSpeed = 10f;
    float gravity = 9.81f;
    float jumpSpeed = 2.3f;


    void Start() 
    {
        Application.runInBackground = true; //allows unity to update when not in focus

		//************* Instantiate the OSC Handler...
	    OSCHandler.Instance.Init ();
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/trigger", "ready");
        //*************

        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        setCount();
    }

    // Has to be Update or jumping will be fucked
    void Update() 
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(x, 0, z);
        if (controller.isGrounded) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                directionY = jumpSpeed;
            }
            else {
                directionY = -1f;
            }
        }

        if (!controller.isGrounded) {
            moveSpeed = 5.5F;
        } else {
            moveSpeed = 10F;
        }


        directionY -= gravity * Time.deltaTime;

        direction.y = directionY;

        controller.Move(direction * moveSpeed * Time.deltaTime);



        //************* Routine for receiving the OSC...
		OSCHandler.Instance.UpdateLogs();
		Dictionary<string, ServerLog> servers = new Dictionary<string, ServerLog>();
		servers = OSCHandler.Instance.Servers;

        foreach (KeyValuePair<string, ServerLog> item in servers) {
			// If we have received at least one packet,
			// show the last received from the log in the Debug console
			if (item.Value.log.Count > 0) {
				int lastPacketIndex = item.Value.packets.Count - 1;

				//get address and data packet
				Score.text = item.Value.packets [lastPacketIndex].Address.ToString ();
				Score.text += item.Value.packets [lastPacketIndex].Data [0].ToString ();

			}
		}
		//*************
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Collect")) {
            // Debug.Log("Coin Collect");

            // //************* Send the message to the client...
            // OSCHandler.Instance.SendMessageToClient ("pd", "/unity/trigger", score);
            // //*************
            score++;
            setCount();
            Destroy(other.gameObject);
        }
    }


    void setCount() {
        Score.text = "Coin Collected: " + score + "/10";
        //************* Send the message to the client...
        // Reason for score is so Pd know how many orbs collected and what sound should be played
        OSCHandler.Instance.SendMessageToClient ("pd", "/unity/trigger", score);
        //*************
    }
}
