using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour, ISwiped, IPinchSpread, IRotate
{
    [Header("Game Vars")]
    public int CurrentLife = 3;
    public int MaxLife = 3;
    public int CurrentScore = 0;
    private int currLimit;
    public int PrevScore;
    private bool gameActive;
    public float shakeLimit = 0.2f;
    private bool hasShakeys;

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
    [Header("Game Objects")]
    public Animator Noel;
    public GameObject GameoverUI;
    public GameObject TapButton;
    public GameObject GameUI;
    public NotifsHandler Annoyance;

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

    //Dynamic touch data
    private Vector2 start_pos;
    private Vector2 end_pos;

    private float gesture_time;
    private float time_limiter;

    private float tapTimer;
    private void Start()
    {
        currLimit = 1;
        GetRandomSequence(currLimit);
        time_limiter = 0;
        gameActive = true;

        tapTimer = MaxTime - (MaxTime * 0.25f);
        hasShakeys = false;
    }

    //TODO:(Delete this) Debug functions for PC based testing
    private void Update()
    {
        //Simulate touch input
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddHistoryNote(Notes.SWIPE_LEFT);
        } //Swipe right

        if (Input.GetKeyDown(KeyCode.W))
        {
            AddHistoryNote(Notes.SWIPE_UP);
        } //Swipe Up

        if (Input.GetKeyDown(KeyCode.S))
        {
            AddHistoryNote(Notes.SWIPE_DOWN);
        } //Swipe Down

        if (Input.GetKeyDown(KeyCode.D))
        {
            AddHistoryNote(Notes.SWIPE_RIGHT);
        } //Swipe Right

        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddHistoryNote(Notes.ROT_CW);
        } //Rot CW

        if (Input.GetKeyDown(KeyCode.E))
        {
            AddHistoryNote(Notes.ROT_CCW);
        } //Rot CCW

        if (Input.GetKeyDown(KeyCode.R))
        {
            AddHistoryNote(Notes.PINCH);
        } //Pinch

        if (Input.GetKeyDown(KeyCode.F))
        {
            AddHistoryNote(Notes.SPREAD);
        } //Spread

        //Simulate other sensors
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shakeys();
        } //Shake
    }

    //Creating touch logic here
    private void FixedUpdate()
    {
        //Count down
        if(gameActive) CurrentTime += Time.fixedDeltaTime;

        //TODO: Convert to tern
        if (CurrentTime >= tapTimer)
        {
            TapButton.SetActive(true);
        }

        else
        {   
            TapButton.SetActive(false);
        }

        if(CurrentTime >= MaxTime)
        {
            GetRandomSequence(currLimit);
            CurrentTime = 0;
            if (CurrentLife > 1)
            {
                CurrentLife--;
            }
            else
            {
                //TODO:End Game + Save Score + Send Notif
                if(CurrentLife > 0) CurrentLife--;
                KillNoel();
                GameOver();

                hasShakeys = false;
            }
        }

        float shakeCheckx = Input.acceleration.x;
        if (Mathf.Abs(shakeCheckx) >= shakeLimit)
        {
            Shakeys();
        }

        float shakeChecky = Input.acceleration.y;
        if (Mathf.Abs(shakeChecky) >= shakeLimit)
        {
            Shakeys();
        }

        //TODO: Invert if
        if (Input.touchCount > 0 && time_limiter <= 0)
        {
            //Swipe gesture
            if (Input.touchCount == 1)
            {
                Debug.Log("One Touch detected");
                setTouchOrigin();
            }

            else
            {
                aFingerTouch = Input.GetTouch(0);
                bFingerTouch = Input.GetTouch(1);

                Debug.Log("Double touch detected");
                if (aFingerTouch.phase == TouchPhase.Moved || bFingerTouch.phase == TouchPhase.Moved)
                {
                    Debug.Log("Pinch/Spread starts");
                    Vector2 prevPoint1 = GetPreviousPoint(aFingerTouch);
                    Vector2 prevPoint2 = GetPreviousPoint(bFingerTouch);

                    float currDistance = Vector2.Distance(aFingerTouch.position, bFingerTouch.position);
                    float prevDistance = Vector2.Distance(prevPoint1, prevPoint2);

                    if (Mathf.Abs(currDistance - prevDistance) >= (_spreadProperty.MinDistanceChange * Screen.dpi))
                    {
                        Debug.Log("Firing spread ev");
                        FireSpreadEvent(currDistance - prevDistance);
                    }
                }

                //TODO: Invert if
                if ((aFingerTouch.phase == TouchPhase.Moved || bFingerTouch.phase == TouchPhase.Moved) &&
                    Vector2.Distance(aFingerTouch.position, bFingerTouch.position) >= (_rotateProperty.MinDistance * Screen.dpi))
                {
                    Debug.Log("Rotation detected");
                    Vector2 prevPoint1 = GetPreviousPoint(aFingerTouch);
                    Vector2 prevPoint2 = GetPreviousPoint(bFingerTouch);

                    Vector2 diff_vector = aFingerTouch.position - bFingerTouch.position;
                    Vector2 prev_diff_vector = prevPoint1 - prevPoint2;

                    float angle = Vector2.Angle(prev_diff_vector, diff_vector);
                    if (angle >= _rotateProperty.MinChange)
                    {
                        Vector3 cross = Vector3.Cross(prev_diff_vector, diff_vector);

                        if (cross.z > 0)
                        {
                            FireRotateEvent(angle, RotationDirections.CCW);
                            Debug.Log($"Rotate Counter Cw{angle}");
                        }

                        else if (cross.z < 0)
                        {
                            FireRotateEvent(angle, RotationDirections.CW);
                            Debug.Log($"Rotate CW {angle}");
                        }
                    }
                }
            }
            
            time_limiter = 1;
        }

        else
        { 
            time_limiter -= 2;
           //Debug.Log(time_limiter);
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
        if (HistorySequence.Count < currLimit)
        {
            HistorySequence.Add(note);

            GameObject spawn = NoteScript.SpawnNote(NotePrefab, Sequence_History, note);
            currentNotes_History.Add(spawn);

            CheckLastNoteMatch();
        }
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
        //Check if the history sequence "can" be checked
        if(TargetSequence.Length >= HistorySequence.Count)
        {
            //If the sequences have enough to do "checking" compare last note
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

    /// Check entire note sequences against one another
    public void CheckNoteSequence()
    {
        hasShakeys = false;
        bool sequenceMatch = true;
        if (TargetSequence.Length != HistorySequence.Count)
        {
            sequenceMatch = false;
        }

        else
        {
            //For all notes currently present
            for (int i = 0; i < currLimit; i++)
            {
                //Check each note against each other
                if (HistorySequence[i] != TargetSequence[i])
                {
                    sequenceMatch = false;
                }
            }
        }

        if (sequenceMatch)
        {
            CurrentScore++;
            if (currLimit < 7)
            {
                currLimit++;
            }

            GetRandomSequence(currLimit);
        }

        else
        {
            CurrentLife--;

            if (CurrentLife <= 0)
            {
                KillNoel();
                GameOver();
            }
            
            GetRandomSequence(currLimit);
            CurrentTime = 0;
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
        //TODO: Convert to Switch
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
        Debug.Log("Pinch/Spread detected");
        //TODO:Convert to Tern
        if (args.DistanceDiff > 0)
        {
            //Spawn spread note
            AddHistoryNote(Notes.SPREAD);
        }

        else
        {
            //spawn pinch note
            AddHistoryNote(Notes.PINCH);
        }
    }

    public void onRotate(RotateEventArgs args)
    {
        if (args.RotationDirection == RotationDirections.CCW)
        {
            AddHistoryNote(Notes.ROT_CCW);
        }

        else if (args.RotationDirection == RotationDirections.CW)
        {
            AddHistoryNote(Notes.ROT_CW);
        }
    }

    ///uTiLiTy fUnCtiOnS LMAO
    private Vector2 GetPreviousPoint(Touch t)
    {
        return t.position - t.deltaPosition;
    }

    //Set touch params
    //TODO:(Delete this) from CheckSingleFingerGesture()
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
            Debug.Log("Initial Firing Swipe");
            end_pos = aFingerTouch.position;

            if (gesture_time <= _swipeProperty.MaxGestureTime && Vector2.Distance(start_pos, end_pos) >=
                (_swipeProperty.MinSwipeDistance * Screen.dpi))
            {
                FireSwipeFunction();
            }
        }

        else gesture_time += Time.deltaTime;
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

    private Vector2 GetMidPoint(Vector2 p1, Vector2 p2)
    {
        Vector2 ret = new Vector2((p1.x + p2.x) / 2, (p1.y + p2.y) / 2);
        return ret;
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

    //Procc pucker
    private void FireSpreadEvent(float dist_diff)
    {
        //TODO: Convert to tern
        if (dist_diff > 0)
        {
            Debug.Log("Spread");
        }

        else
        {
            Debug.Log("Pinch");
        }

        Vector2 midPoint = GetMidPoint(aFingerTouch.position, bFingerTouch.position);

        GameObject hitObj = GetHit(midPoint);

        SpreadEventArgs args = new SpreadEventArgs(aFingerTouch, bFingerTouch, dist_diff, hitObj);

        if (PinchSpreadArgs != null)
        {
            PinchSpreadArgs(this, args);
        }

        this.OnPinchSpread(args);
    }

    //Procc spin
    private void FireRotateEvent(float angle, RotationDirections dir)
    {
        Vector2 midPoint = GetMidPoint(aFingerTouch.position, bFingerTouch.position);

        GameObject hitObj = GetHit(midPoint);

        RotateEventArgs args = new RotateEventArgs(aFingerTouch, bFingerTouch, angle, dir, hitObj);

        if (RotateArgs != null)
        {
            RotateArgs(this, args);
        }

        this.onRotate(args);
    }

    //TODO: Program shake
    private void Shakeys()
    {
        Debug.Log("Your takeout is here");

        if (!hasShakeys)
        {
            ClearHistoryNotes();
            hasShakeys = true;
        }
    }

    //Game over functions
    public void GameOver()
    {
        //stop timer
        gameActive = false;

        //Summon game over UI
        GameoverUI.SetActive(true);
        GameUI.SetActive(false);
    }

    public void Replay()
    {
        currLimit = 1;
        GetRandomSequence(1);
        CurrentTime = 0;

        gameActive = true;
        CurrentLife = 3;
        PrevScore = CurrentScore;
        CurrentScore = 0;
        ReviveNoel();

        GameoverUI.SetActive(false);
        GameUI.SetActive(true);
    }

    public void SendNotif()
    {
        Annoyance.SendScoreNotif(PrevScore);
    }

    public void Leave()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
