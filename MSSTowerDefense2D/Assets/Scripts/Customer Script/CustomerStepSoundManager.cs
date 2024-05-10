using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CustomerStepSoundManager : MonoBehaviour
{
    private List<AudioSource> playingSources = new List<AudioSource>();
    private const int maxPlayingSounds = 4;

    void Update()
    {
        UpdatePlayingSounds();
    }

    void UpdatePlayingSounds()
    {
        var customers = FindObjectsOfType<NormalCustomer>();
        var allSources = customers.SelectMany(customer => customer.GetComponents<AudioSource>()).ToList();
        var currentlyPlaying = allSources.Where(source => source.isPlaying).ToList();

        if (currentlyPlaying.Count > maxPlayingSounds)
        {
            var sourcesToMute = currentlyPlaying.OrderByDescending(source => source.time).Skip(maxPlayingSounds).ToList();
            foreach (var source in sourcesToMute)
            {
                source.mute = true;
            }

            var sourcesToUnmute = currentlyPlaying.OrderBy(source => source.time).Take(maxPlayingSounds).ToList();
            foreach (var source in sourcesToUnmute)
            {
                source.mute = false;
            }
        }
    }
}

