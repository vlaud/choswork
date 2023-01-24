using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using static UnityEditor.PlayerSettings;
//public delegate void MyAction();

public class CharacterMovement : CharacterProperty
{
    Coroutine CoroutineAngle = null;
    //Coroutine CoroutineLerp = null;
    Coroutine attackCo = null;
    Coroutine CoRoot = null;
    public void RePath(NavMeshPath myPath, Vector3 pos, UnityAction done = null)
    {
        StopAllCoroutines();
        //Debug.Log("목표지점: " + pos);
        StartCoroutine(MovingByPath(myPath, pos, done));
    }
    IEnumerator MovingByPath(NavMeshPath myPath, Vector3 pos, UnityAction done)
    {
        int cur = 1;
        NavMesh.CalculatePath(transform.position, pos, NavMesh.AllAreas, myPath);
        Vector3[] list = myPath.corners;

        Vector3 dir = pos - transform.position;
        float dist = dir.magnitude;
        dir.Normalize();

        while (dist > 1.0f) // 도착 판정
        {
            dir = pos - transform.position;
            dist = dir.magnitude;

            if (cur < list.Length)
            {
                NavMesh.CalculatePath(transform.position, pos, NavMesh.AllAreas, myPath); // 실시간으로 네비메쉬 검사
                list = myPath.corners;
                for (int i = 0; i < list.Length - 1; ++i)
                {
                    Debug.DrawLine(list[i], list[i + 1], Color.red);
                }
                //Debug.Log("현재 코너: " + cur + "번째: " + list[cur] + "총 코너: " + list.Length);
                MoveToPosition(list[cur], () => cur++);
            }
            yield return null;
        }
        done?.Invoke();
    }
    protected void AttackTarget(NavMeshPath myPath, Transform target)
    {
        StopAllCoroutines();
        attackCo = StartCoroutine(AttackRoot(myPath, target, myStat.AttackRange, myStat.AttackDelay));
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
        while (Angle > Mathf.Epsilon)
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
        float dist = dir.magnitude;
        dir.Normalize();
        //달리기 시작
        myAnim.SetBool("IsMoving", true);

        while (dist > 0.0f)
        {
            if (!myAnim.GetBool("IsAttacking"))
            {
                dir = pos - transform.position;
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
    IEnumerator AttackRoot(NavMeshPath myPath, Transform target, float AttackRange, float AttackDelay)
    {
        float playTime = 0.0f;
        float delta = 0.0f;
        while (target != null)
        {
            if (!myAnim.GetBool("IsAttacking")) playTime += Time.deltaTime;
            // 이동
            NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, myPath);
            Vector3[] list = myPath.corners;
            Vector3 dir = target.position - transform.position;
            float dist = dir.magnitude;
            dir.Normalize();
            int cur = 1;
            for (int i = 0; i < list.Length - 1; ++i)
            {
                Debug.DrawLine(list[i], list[i + 1], Color.red);
            }
            if (dist > AttackRange)
            {
                myAnim.SetBool("IsRunning", true);

                if (cur < list.Length)
                {
                    dir = list[cur++] - transform.position;
                }
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
                dir.y = 0.0f;
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
