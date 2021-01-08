using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameHandler : MonoBehaviour, ISwiped, IPinchSpread
{
    [Header("Game Vars")]
    public int CurrentLife = 3;
    public int MaxLife = 3;
    public int CurrentScore = 0;

    /// <summary>
    /// Target Sequence of notes to be done by the player
    /// </summary>
    [Header("Sequences")]
    public Notes[] TargetSequence;
    /// <summary>
    /// Current Sequence of notes done by the player
    /// </summary>
    public List<Notes> HistorySequence = new List<Notes>();

    /// <summary>
    /// Current Time in the sequence
    /// </summary>
    [Header("Timers")]
    public float CurrentTime = 0;
    /// <summary>
    /// Time Limit to input the sequence
    /// </summary>
    public float MaxTime = 10;

    /// <summary>
    /// Note prefab to spawn in either the target sequence or the history sequence
    /// </summary>
    [Header("Notes")]
    public GameObject NotePrefab;
    /// <summary>
    /// Transform to hold the Target Sequence notes
    /// </summary>
    public Transform Sequence_Holder;
    /// <summary>
    /// Transform to hold the history of notes done by the player
    /// </summary>
    public Transform Sequence_History;

    /// <summary>
    /// Game character- reacts if you pressed the correct note in the sequence or not
    /// </summary>
    public Animator Noel;

    /// <summary>
    /// Current prefabs in the Sequence holder
    /// </summary>
    private List<GameObject> currentNotes_Holder = new List<GameObject>();

    /// <summary>
    /// Current prefabs in the History holder
    /// </summary>
    private List<GameObject> currentNotes_History = new List<GameObject>();

    /// <summary>
    /// All variables below will hold touch logic of varying types
    /// </summary>

    //Touch params
    public SwipeProperty _swipeProperty;
    public SpreadProperty _spreadProperty;
    public RotateProperty _rotateProperty;

    //Event params
    public event EventHandler<SwipeEventArgs> SwipeArgs;
    public event EventHandler<SpreadEventArgs> PinchSpreadArgs;
    public event EventHandler<RotateEventArgs> RotateArgs;

    //TouchData
    private Touch aFingerTouch;
    private Touch bFingerTouch;
    private Touch cFingerTouch;

    //Dynamic touch data
    private Vector2 start_pos;
    private Vector2 end_pos;

    private float gesture_time;

    private void Start()
    {
        GetRandomSequence(1);
    }


    //Creating touch logic here
    private void FixedUpdate()
    {
        //Count down
        CurrentTime += Time.fixedDeltaTime;
        if(CurrentTime >= MaxTime)
        {
            CurrentTime = 0;
        }

        if (Input.touchCount > 0)
        {
            //Swipe gesture
            if (Input.touchCount == 1)
            {
                Debug.Log("One Touch detected");
                setTouchOrigin();
            }
        }
    }

    /// <summary>
    /// Generates a new sequence for the player to follow
    /// Also clears the current notes in the history
    /// </summary>
    /// <param name="limit">Max number of notes to generate</param>
   public void GetRandomSequence(int limit = 7)
   {
        limit = Mathf.Max(limit, 1);
        CurrentTime = 0;

        ClearHistoryNotes();

        TargetSequence = new Notes[limit];

        for(int i = 0; i < limit; i++)
        {
            TargetSequence[i] = (Notes)UnityEngine.Random.Range(0,8);
        }

        SpawnTargetSequence();
   }

    /// <summary>
    /// Spawns a note in the history holder and adds them to the history sequence
    /// Also checks if the last note spawned matches the position in the sequence
    /// </summary>
    /// <param name="note">Note to spawn</param>
    public void AddHistoryNote(Notes note)
    {
        HistorySequence.Add(note);

        GameObject spawn = NoteScript.SpawnNote(NotePrefab, Sequence_History, note);
        currentNotes_History.Add(spawn);

        CheckLastNoteMatch();
    }

    /// <summary>
    /// Clears all the notes in the history as well as in the array
    /// </summary>
    public void ClearHistoryNotes()
    {
        HistorySequence.Clear();
        ClearNotes(currentNotes_History);
    }

    /// <summary>
    /// Checks if the last note matches each other
    /// Mainly for animation purposes
    /// </summary>
    public void CheckLastNoteMatch()
    {
        if(TargetSequence.Length >= HistorySequence.Count)
        {
            if(TargetSequence[HistorySequence.Count - 1] == HistorySequence[HistorySequence.Count - 1])
            {
                Noel.SetTrigger("Attack");
            }

            else
            {
                Noel.SetTrigger("Hurt");
            }
        }
    }

    /// <summary>
    /// Plays the dead animation for the character
    /// </summary>
    public void KillNoel()
    {
        Noel.SetTrigger("Dead");
    }

    /// <summary>
    /// Plays the revive animation for the character
    /// Only usable when dead was played earlier
    /// </summary>
    public void ReviveNoel()
    {
        Noel.SetTrigger("Revive");
    }

    /// <summary>
    /// Spawns the prefabs in the target sequence
    /// </summary>
    private void SpawnTargetSequence()
    {
        ClearNotes(currentNotes_Holder);
        for(int i = 0; i < TargetSequence.Length; i++)
        {
            SpawnTargetNote(TargetSequence[i]);
        }
    }

    /// <summary>
    /// Call NoteScript to create a new note based on enum
    /// </summary>
    /// <param name="note"></param>
    private void SpawnTargetNote(Notes note)
    {
        GameObject spawn = NoteScript.SpawnNote(NotePrefab, Sequence_Holder, note);
        currentNotes_Holder.Add(spawn);
    }

    /// <summary>
    /// Clear all currently active notes in a note holder game object
    /// </summary>
    /// <param name="note_holder"></param>
    public void ClearNotes(List<GameObject> note_holder)
    {
        for(int i = 0; i < note_holder.Count; i++)
        {
            Destroy(note_holder[i]);
        }

        note_holder.Clear();
    }

    ///Main game play functions
    public void OnSwipe(SwipeEventArgs args)
    {
        //Get the direction of the swipe
        if (args.Direction == Directions.UP)
        {
            //Create the note based on the swipe direction
            Debug.Log("Up");
            AddHistoryNote(Notes.SWIPE_UP);
        }

        else if (args.Direction == Directions.DOWN)
        {
            Debug.Log("Down");
            AddHistoryNote(Notes.SWIPE_DOWN);
        }

        else if (args.Direction == Directions.LEFT)
        {
            Debug.Log("Left");
            AddHistoryNote(Notes.SWIPE_LEFT);
        }

        else if (args.Direction == Directions.RIGHT)
        {
            Debug.Log("Right");
            AddHistoryNote(Notes.SWIPE_RIGHT);
        }
    }

    public void OnPinchSpread(SpreadEventArgs args)
    {

    }

    ///uTiLiTy fUnCtiOnS LMAO
    //Set touch params
    public void setTouchOrigin()
    {
        aFingerTouch = Input.GetTouch(0);
        
        if (aFingerTouch.phase == TouchPhase.Began)
        {
            start_pos = aFingerTouch.position;
            gesture_time = 0;
            Debug.Log("Origin set");
        }

        if (aFingerTouch.phase == TouchPhase.Ended)
        {
            end_pos = aFingerTouch.position;

            if (gesture_time <= _swipeProperty.MaxGestureTime && Vector2.Distance(start_pos, end_pos) >=
                (_swipeProperty.MinSwipeDistance * Screen.dpi))
            {
                FireSwipeFunction();
            }
        }
    }

    //Smacc object via raycast
    private GameObject GetHit(Vector2 screenPos)
    {
        Ray r = Camera.main.ScreenPointToRay(start_pos);
        RaycastHit hit = new RaycastHit();
        GameObject hitObj = null;

        if (Physics.Raycast(r, out hit, Mathf.Infinity))
        {
            hitObj = hit.collider.gameObject;
        }

        return hitObj;
    }

    //Procc swipe input
    private void FireSwipeFunction()
    {
        Debug.Log("SWIPE");

        Vector2 diff = end_pos - start_pos;

        GameObject hitObject = GetHit(start_pos);
        Directions dir;

        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            if (diff.x > 0)
            {
              // Debug.Log("Right");
                dir = Directions.RIGHT;
            }
            else
            {
              //  Debug.Log("Left");
                dir = Directions.LEFT;
            }
        }
        else
        {
            if (diff.y > 0)
            {
                //Debug.Log("up");
                dir = Directions.UP;
            }

            else
            {
                //Debug.Log("down");
                dir = Directions.DOWN;
            }
        }

        SwipeEventArgs args = new SwipeEventArgs(start_pos, diff, dir, hitObject);

        if (SwipeArgs != null)
        {
            SwipeArgs(this, args);
        }

        this.OnSwipe(args);
    }
}
