using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Timeline.TimelinePlaybackControls;

namespace TheKiwiCoder {

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
                context.myRagDolls = context.gameObject.GetComponentInChildren<RagDollPhysics>();
            }
            context.RagDollSet(false);
            // Add whatever else you need here...

            return context;
        }
        public void RagDollSet(bool v)
        {
            physics.isKinematic = v;
            capsuleCollider.isTrigger = v;
            animator.enabled = !v;
            myRagDolls?.RagDollOnOff(v);
        }
    }
}