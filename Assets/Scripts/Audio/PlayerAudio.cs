using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    Rigidbody2D rb;

    AudioSource walkingSource;
    AudioSource shootingSource;

    AudioClip[] walkingSounds;
    AudioClip shootingSound;
    public bool isWalking = false;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        walkingSounds = Resources.LoadAll<AudioClip>("Audio/walking_dirt");
        shootingSound = Resources.Load<AudioClip>("Audio/laser_fire/laser_fire");

        AudioSource[] temp = GetComponents<AudioSource>();
        walkingSource = temp[0];
        shootingSource = temp[1];
    }

    private void FixedUpdate()
    {
        if (!isWalking && rb.velocity.magnitude > 0.2f)
        {
            isWalking = true;
            StartCoroutine(PlayWalkingSound());
        }
    }

    public void ShootSound(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        shootingSource.PlayOneShot(shootingSound, 10);
    }

    IEnumerator PlayWalkingSound()
    {
        while (rb.velocity.magnitude > 0.1f)
        {
            walkingSource.PlayOneShot(walkingSounds[Random.Range(0, walkingSounds.Length)]);
            yield return new WaitForSeconds(Random.Range(0.2f, 0.7f));
        }
        isWalking = false;
    }
}
