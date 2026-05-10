using UnityEngine;

public class ObjectTouch : MonoBehaviour
{
    public NotesSpawner Spawner;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Eh");
        Spawner.NoteDisappear(this);

        var counterFinder = collision.transform.parent.GetComponent<CounterFinder>();
        if (counterFinder != null)
            Spawner.NoteDie(this);
    }

    public void UpdateEvent(float value)
    {
        transform.position -= transform.up * Time.deltaTime * value;
    }
}
