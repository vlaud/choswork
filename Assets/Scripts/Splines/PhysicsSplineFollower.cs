using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;  // 필요할 수 있음, float3 사용 시

public class PhysicsSplineFollowerOfficial : MonoBehaviour
{
    public SplineContainer splineContainer;
    private Spline currentSpline;
    public Rigidbody rb;
    public float speed = 5f;     // 경로를 따라 가는 속도
    public float speedLimit = 10f; // 속도 제한
    public float rotationSpeed = 5f; // 회전 속도
    [ReadOnly] public float curSpeed = 0f; // 현재 속도 (디버그용)
    [ReadOnly] public bool isBoosting = false; // 부스트 상태 (디버그용)

    void OnGUI()
    {
        GUILayout.Label("Current Speed: " + curSpeed);
        GUILayout.Label("Is Boosting: " + isBoosting);
    }

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        currentSpline = splineContainer.Splines[0];
    }

    void FixedUpdate()
    {
        if (splineContainer == null || rb == null) return;
        curSpeed = rb.linearVelocity.magnitude;
        var native = new NativeSpline(currentSpline);
        SplineUtility.GetNearestPoint(native, transform.position, out float3 nearest, out float t);
        Vector3 worldNearest = splineContainer.transform.TransformPoint(nearest);
        transform.position = worldNearest;

        // 스플라인의 접선과 위 벡터를 로컬 공간에서 계산
        float3 localTangent = native.EvaluateTangent(t);
        float3 localUp = native.EvaluateUpVector(t);

        // 월드 공간으로 변환
        Vector3 worldTangent = splineContainer.transform.TransformDirection(localTangent);
        Vector3 worldUp = splineContainer.transform.TransformDirection(localUp);

        var remappedForward = new Vector3(0, 0, 1);
        var remappedUp = new Vector3(0, 1, 0);
        var axisRemappedRotation = Quaternion.Inverse(Quaternion.LookRotation(remappedForward, remappedUp));

        // transform.rotation = Quaternion.LookRotation(worldTangent, worldUp) * axisRemappedRotation;
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(worldTangent, worldUp) * axisRemappedRotation, Time.fixedDeltaTime * rotationSpeed);
        Vector3 carForward = transform.forward;

        if (Vector3.Dot(rb.linearVelocity, transform.forward) < 0)
            carForward = -carForward;

        rb.linearVelocity = rb.linearVelocity.magnitude * carForward;

        Move();
    }

    private void Move()
    {
        if (rb.linearVelocity.magnitude < speedLimit)
        {
            rb.AddForce(transform.forward * speed);
            isBoosting = true;
        }
        else
        {
            isBoosting = false;
        }
    }
}
