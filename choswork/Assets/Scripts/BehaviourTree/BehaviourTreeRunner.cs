using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    BehaviourTree tree;
    // Start is called before the first frame update
    void Start()
    {
        tree = ScriptableObject.CreateInstance<BehaviourTree>();

        var log1 = ScriptableObject.CreateInstance<DebugLogNode>();
        log1.message = "Hello world 111";

        var pause1 = ScriptableObject.CreateInstance<WaitNode>();

        var log2 = ScriptableObject.CreateInstance<DebugLogNode>();
        log2.message = "Hello world 222";

        var pause2 = ScriptableObject.CreateInstance<WaitNode>();

        var log3 = ScriptableObject.CreateInstance<DebugLogNode>();
        log3.message = "Hello world 333";

        var pause3 = ScriptableObject.CreateInstance<WaitNode>();

        var sequence = ScriptableObject.CreateInstance<SequencerNode>();
        sequence.children.Add(log1);
        sequence.children.Add(pause1);
        sequence.children.Add(log2);
        sequence.children.Add(pause2);
        sequence.children.Add(log3);
        sequence.children.Add(pause3);

        var loop = ScriptableObject.CreateInstance<RepeatNode>();
        loop.child = sequence;

        tree.rootNode = loop;
    }

    // Update is called once per frame
    void Update()
    {
        tree.Update();
    }
}
