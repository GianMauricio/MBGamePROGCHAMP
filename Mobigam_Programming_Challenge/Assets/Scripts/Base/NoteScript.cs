using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Types of "Notes"
/// </summary>
public enum Notes
{
    INVALID = -1,

    SWIPE_UP = 0,
    SWIPE_DOWN = 1,
    SWIPE_LEFT = 2,
    SWIPE_RIGHT = 3,

    SPREAD = 4,
    PINCH = 5,

    ROT_CW = 6,
    ROT_CCW = 7
}

public class NoteScript : MonoBehaviour
{
    /// <summary>
    /// Sprites of the notes
    /// </summary>
    public Sprite[] NoteSprites;

    /// <summary>
    /// Currently held note
    /// </summary>
    public Notes AssignedNote = Notes.SWIPE_UP;

    /// <summary>
    /// Holder of the note sprite
    /// </summary>
    public Image Note_Image;

    /// <summary>
    /// Assign a specific note to this holder
    /// </summary>
    /// <param name="toAssign">Note to assign</param>
    public void AssignNote (Notes toAssign)
    {
        AssignedNote = toAssign;

        if (toAssign == Notes.INVALID)
            Note_Image.sprite = null;
        else
            Note_Image.sprite = NoteSprites[(int)AssignedNote];
    }

    /// <summary>
    /// Spawn a note in the given parent
    /// </summary>
    /// <param name="notePrefab">Note Prefab</param>
    /// <param name="parent">Parent of the note</param>
    /// <param name="toAssign">Note to assign</param>
    public static GameObject SpawnNote(GameObject notePrefab, Transform parent, Notes toAssign)
    {
        var obj = GameObject.Instantiate(notePrefab, parent, false);
        NoteScript note = obj.GetComponent<NoteScript>();

        note.AssignNote(toAssign);

        return obj;
    }
}
