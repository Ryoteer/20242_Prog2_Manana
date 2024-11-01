using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAvatar : MonoBehaviour
{
    [Header("<color=#6A89A7>Audio</color>")]
    [SerializeField] private AudioSource[] _sources;
    [SerializeField] private AudioClip[] _stepClips;
    [SerializeField] private AudioClip[] _attackClips;
    private Player _parent;

    private void Start()
    {
        _parent = GetComponentInParent<Player>();
    }

    public void AreaAttack(int dmg = 0)
    {
        _parent.AreaAttack(dmg);
    }

    public void Attack(int dmg = 0)
    {
        _parent.Attack(dmg);
    }

    public void PierceAttack(int dmg = 0)
    {
        _parent.PierceAttack(dmg);
    }

    public void PlayStepClip()
    {
        if (_sources[0].isPlaying) _sources[0].Stop();

        _sources[0].clip = _stepClips[Random.Range(0, _stepClips.Length)];

        _sources[0].Play();
    }

    public void PlayAttackClip()
    {
        if (_sources[1].isPlaying) _sources[1].Stop();

        _sources[1].clip = _attackClips[Random.Range(0, 3 + 1)];

        _sources[1].Play();
    }

    public void PlayPiercingClip()
    {
        if (_sources[1].isPlaying) _sources[1].Stop();

        _sources[1].clip = _attackClips[4];

        _sources[1].Play();
    }

    public void PlayAreaAtkClip()
    {
        if (_sources[1].isPlaying) _sources[1].Stop();

        _sources[1].clip = _attackClips[Random.Range(5, 8 + 1)];

        _sources[1].Play();
    }
}
