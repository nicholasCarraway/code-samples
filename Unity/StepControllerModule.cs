// Sample from the VR Dance Game
// This script is attached to the ground plane
// Some code has been omitted

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepControllerModule : MonoBehavior
{
    public TrackPlayer trackPlayer;
    // FootData.cs is attached to each respective foot object
    public FootData left;
    public FootData right;

    // A step is detected when a foot collides with the ground
    // FootData.Down is set to false when its foot
    // leaves a lane trigger and FootData.Lanes.Count is 0
    void OnCollisionEnter(Collision other){
        if (other.gameObject.CompareTag("Left Foot") && !left.Down){
            // sets left.Down to true
            // updates variables related to other actions
            left.Step();
            CheckStep(left, trackPlayer.NextLeftBeat());
        }
        else if (other.gameObject.CompareTag("Right Foot") && !right.Down){
            // sets right.Down to true
            // updates variables related to other actions
            right.Step();
            CheckStep(right, trackPlayer.NextRightBeat());
        }
    }

    private void CheckStep(FootData step, GameObject beat)
    {
        // it is possible that the next beat object is empty
        // null beats might occur if there are no beat objects during a step
        if (beat == null){
            // ClearStep cleans up after a hit/miss
            // it updates pivot data for CheckPivot
            // if the beat exists it removes the beat from the list and destroys its object
            ClearStep(step, beat);
            return;
        }

        // get the lane (variable for readability)
        int laneIndex = beat.GetComponent<BeatController>().LaneIndex;

        // step.Lanes is a HashSet<int> that is updated whenever a foot enters/exits a lane trigger
        // check that the foot has landed in the correct lane
        if (step.Lanes.Countains(laneIndex))
        {
            // check that the step has the correct rotation
            if (step.Rotation == beat.GetComponent<BeatController>().Rotation)
            {
                // successful hit, check the accuracy

                // get the target beat position (variable for readability)
                float beatPos = beat.GetComponent<BeatController>().BeatIndex;

                // accuracy is the difference in beats between the target beat and the timing of the step
                // may update this to be based on milliseconds instead of beats (TrackPlayerModule has a SecondsPerBeat property)
                float accuracy = Mathf.Abs(beatPos - trackPlayer.GetTrackPosInBeats());

                // timingWindow is 0.5f by default
                // if the accuracy is within the threshhold
                // register a hit with the GameController
                if (accuracy <= timingWindow)
                {
                    // GameController.Hit(float accuracy) calculates the score of the hit
                    // determines the type of hit (Good, Great, Perfect)
                    // updates the score, combo, and respective hit count
                    // updates UI
                    GameController.Hit(accuracy);
                }
            }
            else // wrong rotation, bad hit
            {
                // GameController.BadHit() updates longest combo
                // resets current combo
                // updates bad hit count
                // updates UI
                GameController.BadHit();
            }

            // ClearStep cleans up after a hit/miss
            // it updates pivot data for CheckPivot
            // if the beat exists it removes the beat from the list and destroys its object
            ClearStep(step, beat);
        }
    }
}