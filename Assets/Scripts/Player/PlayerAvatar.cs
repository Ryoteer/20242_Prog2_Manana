using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAvatar : MonoBehaviour
{
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
}
