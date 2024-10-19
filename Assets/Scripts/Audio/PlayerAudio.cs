using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    Rigidbody2D rb;

    AudioSource walkingSource;
    AudioSource shootingSource;
    AudioSource tookDamageSource;

    AudioClip[] dirtWalkingSounds;
    AudioClip[] swampWalkingSounds;
    AudioClip[] asphaltWalkingSounds;
    AudioClip[] forestWalkingSounds;
    AudioClip[] activeWalkingSounds;
    AudioClip[] shootingSounds;
    AudioClip tookDamageSound;
    AudioClip endSequenceTookDamageSound;
    public bool isWalking = false;

    // Start is called before the first frame update
    void Awake()
    {
        AudioSource[] temp = GetComponents<AudioSource>();
        walkingSource = temp[0];
        shootingSource = temp[1];
        tookDamageSource = temp[2];

        dirtWalkingSounds = Resources.LoadAll<AudioClip>("Audio/walking_dirt");
        swampWalkingSounds = Resources.LoadAll<AudioClip>("Audio/walking_swamp");
        asphaltWalkingSounds = Resources.LoadAll<AudioClip>("Audio/walking_asphalt");
        forestWalkingSounds = Resources.LoadAll<AudioClip>("Audio/walking_forest");
        shootingSounds = Resources.LoadAll<AudioClip>("Audio/laser_fire/Player");
        tookDamageSound = Resources.Load<AudioClip>("Audio/damage_taken/player_damage_taken");
        endSequenceTookDamageSound = Resources.Load<AudioClip>(
            "Audio/damage_taken/Ship/rocket_damage_taken"
        );

        activeWalkingSounds = dirtWalkingSounds;

        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!PlayerController.isEndingSequence && !isWalking && rb.velocity.magnitude > 0.2f)
        {
            isWalking = true;
            StartCoroutine(PlayWalkingSound());
        }
    }

    public void SetWalkingSound(TerrainTypes type) =>
        activeWalkingSounds = type switch
        {
            TerrainTypes.NORMAL => dirtWalkingSounds,
            TerrainTypes.ASPHALT => asphaltWalkingSounds,
            TerrainTypes.SWAMP => swampWalkingSounds,
            TerrainTypes.FOREST => forestWalkingSounds,
            _ => null,
        };

    public void ShootSoundBasic() =>
        shootingSource.PlayOneShot(shootingSounds[Random.Range(0, 2)], 10);

    public void ShootSoundAdvanced() =>
        shootingSource.PlayOneShot(shootingSounds[Random.Range(2, 4)], 10);

    IEnumerator PlayWalkingSound()
    {
        while (rb.velocity.magnitude > 0.1f)
        {
            walkingSource.PlayOneShot(
                activeWalkingSounds[Random.Range(0, activeWalkingSounds.Length)]
            );
            yield return new WaitForSeconds(Random.Range(0.2f, 0.7f));
        }
        isWalking = false;
    }

    public void TookDamageSound() => tookDamageSource.PlayOneShot(tookDamageSound);

    public void EndSequenceTookDamageSound() =>
        tookDamageSource.PlayOneShot(endSequenceTookDamageSound);
}
