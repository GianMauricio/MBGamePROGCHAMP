using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
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

    private void Start()
    {
        GetRandomSequence(1);
    }

    private void FixedUpdate()
    {
        //Count down
        CurrentTime += Time.fixedDeltaTime;
        if(CurrentTime >= MaxTime)
        {
            CurrentTime = 0;
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
            TargetSequence[i] = (Notes)Random.Range(0,8);
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

    private void SpawnTargetNote(Notes note)
    {
        GameObject spawn = NoteScript.SpawnNote(NotePrefab, Sequence_Holder, note);
        currentNotes_Holder.Add(spawn);
    }

    public void ClearNotes(List<GameObject> note_holder)
    {
        for(int i = 0; i < note_holder.Count; i++)
        {
            Destroy(note_holder[i]);
        }

        note_holder.Clear();
    }
}
