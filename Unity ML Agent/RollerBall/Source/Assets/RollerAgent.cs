using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgent : Agent
{
    Rigidbody rBody;
    const int k_NoAction = 0;  // do nothing!
    const int k_Up = 1;
    const int k_Down = 2;
    const int k_Left = 3;
    const int k_Right = 4;
    void Start () {
    	GetComponent<Renderer>().material.color = Color.yellow;
        rBody = GetComponent<Rigidbody>();
    }

    public Transform Target;
    public override void OnEpisodeBegin()
    {
       // If the Agent fell, zero its momentum
        if (this.transform.localPosition.y < 0)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3( 0, 0.5f, 0);
        }

        // Move the target to a new spot
        Target.localPosition = new Vector3(Random.value * 8 - 4,
                                           0.5f,
                                           Random.value * 8 - 4);
    }

    public override void CollectObservations(VectorSensor sensor)
	{
	    // Target and Agent positions
	    sensor.AddObservation(Target.localPosition);
	    sensor.AddObservation(this.transform.localPosition);

	    // Agent velocity
	    sensor.AddObservation(rBody.velocity.x);
	    sensor.AddObservation(rBody.velocity.z);
	}

	public float force = 5;
	public override void OnActionReceived(ActionBuffers actionBuffers)
	{
        var action = actionBuffers.DiscreteActions[0];
        Vector3 controlSignal = Vector3.zero;
        switch (action)
        {
            case k_NoAction:
                // do nothing
                break;
            case k_Right:
                controlSignal.x += force;
                break;
            case k_Left:
            	controlSignal.x -= force;
                break;
            case k_Up:
                controlSignal.z += force;
                break;
            case k_Down:
                controlSignal.z -= force;
                break;
        }
        rBody.AddForce(controlSignal);

	    float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Rewards
	    AddReward(-0.01f);

	    // Reached target
	    if (distanceToTarget < 1.2f)
	    {
            AddReward(1.0f);
	        EndEpisode();
	    }

	    // Fell off platform
	    else if (this.transform.localPosition.y < 0)
	    {
	    	AddReward(-2.0f);
            // SetReward(-2.0f);
	        EndEpisode();
	    }
	}

	public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = k_NoAction;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            discreteActionsOut[0] = k_Right;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            discreteActionsOut[0] = k_Up;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            discreteActionsOut[0] = k_Left;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            discreteActionsOut[0] = k_Down;
        }
    }
	
}