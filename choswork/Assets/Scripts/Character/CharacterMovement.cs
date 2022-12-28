using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.PlayerSettings;
//public delegate void MyAction();

public class CharacterMovement : CharacterProperty
{
    Coroutine CoroutineAngle = null;
    //Coroutine CoroutineLerp = null;
    Coroutine attackCo = null;
    Coroutine CoRoot = null;

    protected void AttackTarget(Transform target)
    {
        StopAllCoroutines();
        attackCo = StartCoroutine(AttackRoot(target, myStat.AttackRange, myStat.AttackDelay));
    }
    protected void RotateToTarget(Vector3 pos, UnityAction done = null)
    {
        if (CoroutineAngle != null)
        {
            StopCoroutine(CoroutineAngle);
            CoroutineAngle = null;
        }
        CoroutineAngle = StartCoroutine(RotatingToPosition(pos));
    }
    protected void MoveToPosition(Vector3 pos, UnityAction done = null, bool Rot = true)
    {
        if(attackCo != null)
        {
            StopCoroutine(attackCo);
            attackCo = null;
        }
        if(CoRoot != null)
        {
            StopCoroutine(CoRoot);
            CoRoot = null;
        }
        CoRoot = StartCoroutine(RootMotionMoving(pos, done));

        if (Rot)
        {
            if (CoroutineAngle != null)
            {
                StopCoroutine(CoroutineAngle);
                CoroutineAngle = null;
            }
        }
        CoroutineAngle = StartCoroutine(RotatingToPosition(pos));
    }
    IEnumerator RotatingToPosition(Vector3 pos)
    {
        Vector3 dir = (pos - transform.position).normalized;
        dir.y = 0.0f;
        float Angle = Vector3.Angle(transform.forward, dir);
        float rotDir = 1.0f;
        if (Vector3.Dot(transform.right, dir) < 0.0f)
        {
            rotDir = -rotDir;
        }
        while (Angle > 0.0f)
        {
            if (!myAnim.GetBool("IsAttacking"))
            {
                float delta = myStat.RotSpeed * Time.deltaTime;

                if (delta > Angle)
                {
                    delta = Angle;
                }

                Angle -= delta;
                transform.Rotate(Vector3.up * delta * rotDir, Space.World);
            }

            yield return null;
        }
    }
    // 안씀
    IEnumerator LerpToPosition(Vector3 pos, Vector2 desireAnim, UnityAction done)
    {
        Vector3 dir = pos - transform.position;
        float dist = dir.magnitude;
        dir.Normalize();
        Vector2 desireDir = Vector2.zero;

        //달리기 시작
        while (dist > 0.0f)
        {
            if (!myAnim.GetBool("IsAttacking"))
            {
                desireDir = Vector2.Lerp(desireDir, desireAnim, Time.deltaTime * myStat.MoveSpeed);
                myAnim.SetFloat("x", desireDir.x);
                myAnim.SetFloat("y", desireDir.y);

                float delta = myStat.MoveSpeed * Time.deltaTime;
                if (delta > dist)
                {
                    delta = dist;
                }

                dist -= delta;
                transform.Translate(dir * delta, Space.World);
            }
            yield return null;

        }
        //달리기 끝
        desireDir = Vector2.Lerp(desireDir, new Vector2(0, 0), Time.deltaTime * myStat.MoveSpeed);
        myAnim.SetFloat("x", desireDir.x);
        myAnim.SetFloat("y", desireDir.y);
        //myAnim.SetBool("IsMoving", false);
        done?.Invoke();
    }
    // 루트모션 움직임
    IEnumerator RootMotionMoving(Vector3 pos, UnityAction done)
    {
        Vector3 dir = pos - transform.position;
        dir.y = 0.0f;
        float dist = dir.magnitude;
        dir.Normalize();
        //달리기 시작
        myAnim.SetBool("IsMoving", true);

        while (dist > 1.0f)
        {
            if (!myAnim.GetBool("IsAttacking"))
            {
                dir = pos - transform.position;
                dir.y = 0.0f;
                dist = dir.magnitude;
               
            }
            yield return null;
        }
        //달리기 끝
        myAnim.SetBool("IsMoving", false);
        done?.Invoke();
    }
    // 안씀
    IEnumerator MovingToPosition(Vector3 pos, UnityAction done)
    {
        Vector3 dir = pos - transform.position;
        float dist = dir.magnitude;
        dir.Normalize();

        //달리기 시작
        myAnim.SetBool("IsMoving", true);
        while (dist > 0.0f)
        {
            if (!myAnim.GetBool("IsAttacking"))
            {
                float delta = myStat.MoveSpeed * Time.deltaTime;
                if (delta > dist)
                {
                    delta = dist;
                }
                dist -= delta;
                transform.Translate(dir * delta, Space.World);
            }
            yield return null;

        }
        //달리기 끝
        myAnim.SetBool("IsMoving", false);
        done?.Invoke();
    }
    // 안씀
    IEnumerator AttackT(Transform target, float AttackRange, float AttackDelay)
    {
        float playTime = 0.0f;
        float delta = 0.0f;
        while(target != null)
        {
            if(!myAnim.GetBool("IsAttacking")) playTime += Time.deltaTime;
            // 이동
            Vector3 dir = target.position - transform.position;
            float dist = dir.magnitude;
            dir.Normalize();


            if (dist > AttackRange)
            {
                myAnim.SetBool("IsMoving", true);
                delta = myStat.MoveSpeed * Time.deltaTime;

                if (delta > dist)
                    delta = dist;

                transform.Translate(dir * delta, Space.World);
            }
            else
            {
                myAnim.SetBool("IsMoving", false);
                if(playTime >= AttackDelay)
                {
                    //공격
                    playTime = 0.0f;
                    myAnim.SetTrigger("Attack");
                }
            }

            // 회전
            delta = myStat.RotSpeed * Time.deltaTime;

            float Angle = Vector3.Angle(dir, transform.forward);
            float rotDir = 1.0f;

            if (Vector3.Dot(transform.right, dir) < 0.0f)
                rotDir = -rotDir;
            if (delta > Angle)
                delta = Angle;
            transform.Rotate(Vector3.up * delta * rotDir, Space.World);
            

            yield return null;
        }
        myAnim.SetBool("IsMoving", false);
    }
    // 루트모션 공격
    IEnumerator AttackRoot(Transform target, float AttackRange, float AttackDelay)
    {
        float playTime = 0.0f;
        float delta = 0.0f;
        while (target != null)
        {
            if (!myAnim.GetBool("IsAttacking")) playTime += Time.deltaTime;
            // 이동
            Vector3 dir = target.position - transform.position;
            dir.y = 0.0f;
            float dist = dir.magnitude;
            dir.Normalize();

            if (dist > AttackRange)
            {
                myAnim.SetBool("IsRunning", true); // 루트모션이니 애니메이션만 활성화
            }
            else
            {
                myAnim.SetBool("IsRunning", false);
                if (playTime >= AttackDelay)
                {
                    //공격
                    playTime = 0.0f;
                    myAnim.SetTrigger("Attack");
                }
            }
            if (!myAnim.GetBool("IsAttacking")) // 자연스러운 움직임을 위해 공격시엔 회전 비활성화
            {
                // 회전
                delta = myStat.RotSpeed * Time.deltaTime;

                float Angle = Vector3.Angle(dir, transform.forward);
                float rotDir = 1.0f;

                if (Vector3.Dot(transform.right, dir) < 0.0f)
                    rotDir = -rotDir;
                if (delta > Angle)
                    delta = Angle;
                transform.Rotate(Vector3.up * delta * rotDir, Space.World);

            }
            yield return null;
        }
        myAnim.SetBool("IsRunning", false);
    }
}
