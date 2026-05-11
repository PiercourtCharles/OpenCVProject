using MidiPlayerTK;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class MidiIntegration : MonoBehaviour
{
    [SerializeField] List<MusicNote> _notes = new();

    [SerializeField] MidiFilePlayer _filePlayer;
    [SerializeField] GameObject _notePrefab;
    [SerializeField] Transform _boundaryA;
    [SerializeField] Transform _boundaryB;
    [SerializeField] Transform _boundaryC;
    [SerializeField] Transform _noteParent;

    [SerializeField] NotesSpawner _spawner;

    public void NoteDisplay(MidiFilePlayer midiPlayer)
    {
        _filePlayer = midiPlayer;
        _filePlayer.OnEventNotesMidi.AddListener(OnNotesMidi);
    }

    void OnNotesMidi(List<MPTKEvent> events)
    {
        foreach (MPTKEvent midiEvent in events)
        {
            MusicNote noteVar = new();

            noteVar.Tick = midiEvent.Tick;
            noteVar.NoteValue = midiEvent.Value;
            noteVar.NoteName = HelperNoteLabel.LabelFromMidi(noteVar.NoteValue);
            noteVar.Velocity = midiEvent.Velocity;
            noteVar.Duration = midiEvent.Length;

            _notes.Add(noteVar);

            //PlaceNote(noteVar);
            _spawner.PlaceNote(noteVar);
        }
    }

    //Debug notes display on screen, just uncomment ligne 40 to see
    public void PlaceNote(MusicNote note)
    {
        float valuePos = (float)note.Tick / _filePlayer.MPTK_TickLast;
        float valueNoteLerp = note.NoteValue / 127f;

        Vector3 pos = Vector3.Lerp(_boundaryA.position, _boundaryB.position, valuePos);
        float valueNote = Mathf.Lerp(_boundaryA.position.y, _boundaryC.position.y, valueNoteLerp);

        pos = new Vector3(pos.x, valueNote, pos.z);

        var obj = Instantiate(_notePrefab, pos, Quaternion.identity, _noteParent).transform;
        obj.localPosition = new Vector3(obj.localPosition.x, obj.localPosition.y, 0);
    }

    public MidiFilePlayer GetFile()
    {
        return _filePlayer;
    }
}

[Serializable]
public class MusicNote
{
    // Tick actuel de la Note
    public long Tick;

    // Valeur MIDI de la Note
    public int NoteValue;
    public string NoteName;

    // Velocity
    public int Velocity;

    // Durée en ticks
    public long Duration;
}
