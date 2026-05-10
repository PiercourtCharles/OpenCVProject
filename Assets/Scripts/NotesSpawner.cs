using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotesSpawner : MonoBehaviour
{
    [SerializeField] MidiIntegration _midi;
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] Vector2 _timeBetweenNotes = new Vector2(1, 3);
    [SerializeField] List<ObjectTouch> _notes = new();
    [SerializeField] GameObject _notePrefab;
    [SerializeField] Transform _boundaryA;
    [SerializeField] Transform _boundaryB;
    [SerializeField] Transform _noteParent;
    [SerializeField] float _speed = 1;

    [Header("")]
    int _score = 0;
    float _actualTimeSpace = 1;
    float _timeValue = 0;
    [SerializeField] bool _isGameLaunched = false;

    //public void StartGame()
    //{
    //    _isGameLaunched = true;
    //}

    private void Update()
    {
        if (!_isGameLaunched)
            return;

        _timeValue += Time.deltaTime;

        if (_timeValue >= _actualTimeSpace)
        {
            _timeValue = 0;
            _actualTimeSpace = Random.Range(_timeBetweenNotes.x, _timeBetweenNotes.y);
            PlaceNote();
        }

        foreach (var item in _notes)
        {
            item.UpdateEvent(_speed);
        }
    }

    public void PlaceNote(/*Note note*/)
    {
        //float valuePos = note.NoteValue / 127f;
        float valuePos = Random.Range(0f, 1.1f);

        Vector3 pos = Vector3.Lerp(_boundaryA.position, _boundaryB.position, valuePos);

        Debug.Log($"{valuePos} : {pos}");

        var obj = Instantiate(_notePrefab, pos, Quaternion.identity, _noteParent).transform;
        obj.localPosition = new Vector3(obj.localPosition.x, obj.localPosition.y, 0);

        var note = obj.GetComponent<ObjectTouch>();
        if (note != null)
        {
            note.Spawner = this;
            _notes.Add(note);
        }
    }

    public void NoteDisappear(ObjectTouch note)
    {
        _notes.Remove(note);
        Destroy(note.gameObject);
    }

    public void NoteDie(ObjectTouch note)
    {
        _notes.Remove(note);
        Destroy(note.gameObject);

        _score++;

        if (_text != null)
            _text.text = _score.ToString();
    }

    public void ChangePlayMode()
    {
        _isGameLaunched = !_isGameLaunched;

        if (_isGameLaunched)
            _midi.GetFile().MPTK_Play();
        else
            _midi.GetFile().MPTK_Pause();
    }
}
