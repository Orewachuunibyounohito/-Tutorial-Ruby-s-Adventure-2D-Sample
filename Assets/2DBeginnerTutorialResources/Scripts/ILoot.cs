using System.Collections;
using UnityEngine;

public interface ILoot
{   public static float LOOTSPEED = 5.0f;

    public IEnumerator MoveToLooter( Transform looter, RubyItem item );
}
