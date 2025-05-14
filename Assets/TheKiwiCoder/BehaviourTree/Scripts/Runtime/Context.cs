using UnityEngine;
using UnityEngine.AI;

namespace TheKiwiCoder
{

    // The context is a shared object every node has access to.
    // Commonly used components and subsytems should be stored here
    // It will be somewhat specfic to your game exactly what to add here.
    // Feel free to extend this class 
    public class Context {
        public GameObject gameObject;
        public Transform transform;
        public Animator animator;
        public Rigidbody physics;
        public RagDollPhysics myRagDolls;
        public NavMeshAgent agent;
        public SphereCollider sphereCollider;
        public BoxCollider boxCollider;
        public CapsuleCollider capsuleCollider;
        public CharacterController characterController;
        public CharacterMovement characterMovement;
        public Movement movement;
        // Add other game specific systems here

        public static Context CreateFromGameObject(GameObject gameObject) {
            // Fetch all commonly used components
            Context context = new Context();
            context.gameObject = gameObject;
            context.transform = gameObject.transform;
            context.animator = gameObject.GetComponent<Animator>();
            if (context.animator == null)
            {
                context.animator = context.gameObject.GetComponentInChildren<Animator>();
            }
            context.physics = gameObject.GetComponent<Rigidbody>();
            if (context.physics == null)
            {
                context.physics = context.gameObject.GetComponentInChildren<Rigidbody>();
            }
            context.agent = gameObject.GetComponent<NavMeshAgent>();
            context.sphereCollider = gameObject.GetComponent<SphereCollider>();
            context.boxCollider = gameObject.GetComponent<BoxCollider>();
            context.capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
            context.characterController = gameObject.GetComponent<CharacterController>();
            context.myRagDolls = gameObject.GetComponent<RagDollPhysics>();
            if (context.myRagDolls == null)
            {
                context.myRagDolls = gameObject.GetComponentInChildren<RagDollPhysics>();
            }
            context.characterMovement = gameObject.GetComponent<CharacterMovement>();
            context.movement = gameObject.GetComponent<Movement>();
            // Add whatever else you need here...

            return context;
        }
    }
}