using System.Collections;
using UnityEngine;

public interface IItemEffect
{
    public IEnumerator Effect( GameObject target );
}
