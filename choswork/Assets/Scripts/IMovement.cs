using UnityEngine;
using UnityEngine.AI;

public interface IMovement : IRagdoll
{
    public Transform target { get; }
    public AIState aiState { get; }
    public RagDollState rdState { get; }
    public NavMeshPath myPath { get; }
    public NavMeshQueryFilter filter { get; }
}
