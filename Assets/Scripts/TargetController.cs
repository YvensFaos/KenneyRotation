using Lean.Pool;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    private string selfTag;

    [SerializeField] private int points;
    [SerializeField] private GameObject particleEffect;

    private void Awake()
    {
        selfTag = tag;
    }

    private void OnTriggerEnter(Collider other)
    {
        Solve(other.gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        Solve(other.gameObject);
    }

    private void Solve(GameObject other)
    {
        string otherTag = other.tag;
        GameLogic.Instance.GetPoints(points, otherTag.Equals(selfTag));
        if (particleEffect != null)
        {
            LeanPool.Spawn(particleEffect, transform.position, Quaternion.identity);    
        }
        Destroy(gameObject);
        Destroy(other);
    }
}