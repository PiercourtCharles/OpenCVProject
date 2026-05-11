using UnityEngine;

/// <summary>
/// Object falling and desappear by being touched
/// </summary>
public class ObjectTouch : MonoBehaviour
{
    public NotesSpawner Spawner;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Spawner.NoteDisappear(this);

        var counterFinder = collision.transform.GetComponent<OutlinesFinder>();

        if (counterFinder != null)
            Spawner.NoteDie(this);
    }

    public void UpdateEvent(float value)
    {
        transform.position -= transform.up * Time.deltaTime * value;
    }
}