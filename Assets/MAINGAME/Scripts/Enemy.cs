using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public Vector3 forwardRotationPoint;
    public Vector3 backRotationPoint;
    public Vector3 leftRotationPoint;
    public Vector3 rightRotationPoint;
    public Vector3 center;
    public Transform forwardRotationPointDebug;
    public Transform backRotationPointDebug;
    public Transform leftRotationPointDebug;
    public Transform rightRotationPointDebug;
    public bool rolling;
    public bool isRevert = false;
    bool isStartGame = true;
    Vector3 point;
    Vector3 axis;
    public List<string> listMovements = new List<string>();
    string moveDir;
    char[] moveArray;
    int moveID;
    GameController gameController;
    public ParticleSystem rollingEffect;
    bool canL = true;
    bool canR = true;
    bool canF = true;
    bool canB = true;
    public LayerMask wallLayer;
    float range;
    Vector3 lastRandomChoice;
    bool isCircle = false;

    void OnEnable()
    {
        var gameControllerGet = GameObject.FindGameObjectWithTag("Player");
        gameController = gameControllerGet.GetComponent<GameController>();
        var currentLevel = PlayerPrefs.GetInt("currentLevel");
        BoundCalculator();
    }

    public void BoundCalculator()
    {
        var xList = new List<float>();
        var yList = new List<float>();
        var zList = new List<float>();
        float x = 0;
        float y = 0;
        float z = 0;
        int count = 0;
        try
        {
            foreach (var item in transform.GetChild(0).GetComponentsInChildren<BoxCollider>())
            {
                if (item != null)
                {
                    count++;
                    x += item.transform.position.x;
                    y += item.transform.position.y;
                    z += item.transform.position.z;
                    xList.Add(item.transform.position.x);
                    yList.Add(item.transform.position.y);
                    zList.Add(item.transform.position.z);
                }
            }
        }
        catch { }
        xList.Sort();
        yList.Sort();
        zList.Sort();
        center = new Vector3(x / count, y / count, z / count);
        rollingEffect.transform.position = new Vector3(center.x, 0.5f, center.z);
        rollingEffect.Play();
        range = yList[yList.Count - 1] - 0.1f;

        forwardRotationPoint = new Vector3((xList[0] + xList[zList.Count - 1]) / 2, yList[0] - 0.5f, zList[zList.Count - 1] + 0.5f);
        backRotationPoint = new Vector3((xList[0] + xList[zList.Count - 1]) / 2, yList[0] - 0.5f, zList[0] - 0.5f);
        leftRotationPoint = new Vector3(xList[0] - 0.5f, yList[0] - 0.5f, (zList[0] + zList[zList.Count - 1]) / 2);
        rightRotationPoint = new Vector3(xList[xList.Count - 1] + 0.5f, yList[0] - 0.5f, (zList[0] + zList[zList.Count - 1]) / 2);

        canF = true;
        canB = true;
        canL = true;
        canR = true;
        RaycastHit hit;
        if (Physics.Raycast(forwardRotationPoint, forwardRotationPointDebug.transform.position.normalized, out hit, range, wallLayer))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                canF = false;
            }
            else
            {
                canF = true;
            }
        }
        if (Physics.Raycast(backRotationPoint, backRotationPointDebug.transform.position.normalized, out hit, range, wallLayer))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                canB = false;
            }
            else
            {
                canB = true;
            }
        }
        if (Physics.Raycast(leftRotationPoint, leftRotationPointDebug.transform.position.normalized, out hit, range, wallLayer))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                canL = false;
            }
            else
            {
                canL = true;
            }
        }
        if (Physics.Raycast(rightRotationPoint, rightRotationPointDebug.transform.position.normalized, out hit, range, wallLayer))
        {
            if (hit.collider.CompareTag("Wall"))
            {
                canR = false;
            }
            else
            {
                canR = true;
            }
        }
        //Debug.Log(canF + " " + canB + " " + canL + " " + canR);
        rolling = false;
    }

    public void Step()
    {
        if (axis == Vector3.right && canF)
        {
            lastRandomChoice = axis;
            StartCoroutine(Roll(forwardRotationPoint));
        }
        else if (axis == Vector3.left && canB)
        {
            lastRandomChoice = axis;
            StartCoroutine(Roll(backRotationPoint));
        }
        else if (axis == Vector3.back && canR)
        {
            lastRandomChoice = axis;
            StartCoroutine(Roll(rightRotationPoint));
        }
        else if (axis == Vector3.forward && canL)
        {
            lastRandomChoice = axis;
            StartCoroutine(Roll(leftRotationPoint));
        }
        else if (canF && lastRandomChoice != Vector3.left)
        {
            axis = Vector3.right;
            lastRandomChoice = axis;
            StartCoroutine(Roll(forwardRotationPoint));
        }
        else if (canB && lastRandomChoice != Vector3.right)
        {
            axis = Vector3.left;
            lastRandomChoice = axis;
            StartCoroutine(Roll(backRotationPoint));
        }
        else if (canR && lastRandomChoice != Vector3.forward)
        {
            axis = Vector3.back;
            lastRandomChoice = axis;
            StartCoroutine(Roll(rightRotationPoint));
        }
        else if (canL && lastRandomChoice != Vector3.back)
        {
            axis = Vector3.forward;
            lastRandomChoice = axis;
            StartCoroutine(Roll(leftRotationPoint));
        }
        else
        {
            rolling = false;
        }
    }

    private IEnumerator Roll(Vector3 rotationPoint)
    {
        point = rotationPoint;
        float angle = 90;
        float a = 0;
        rolling = true;

        while (angle > 0)
        {
            if(angle < 30)
            {
                a = Time.deltaTime * speed * 1.25f;
            }
            else
            a = Time.deltaTime * speed;
            transform.RotateAround(point, axis, a);
            angle -= a;
            yield return null;
        }
        transform.RotateAround(point, axis, angle);
        BoundCalculator();
    }

    private void Update()
    {
        Debug.DrawRay(forwardRotationPoint, forwardRotationPointDebug.transform.position.normalized * range, Color.red);
        Debug.DrawRay(backRotationPoint, backRotationPointDebug.transform.position.normalized * range, Color.red);
        Debug.DrawRay(leftRotationPoint, leftRotationPointDebug.transform.position.normalized * range, Color.red);
        Debug.DrawRay(rightRotationPoint, rightRotationPointDebug.transform.position.normalized * range, Color.red);
    }

    public void MovePatrol()
    {
        moveDir = listMovements[gameController.currentMove];
        Debug.Log(gameController.currentMove);
        gameController.currentMove++;
        moveArray = moveDir.ToCharArray();
        moveID = 0;
        BoundCalculator();
        StartCoroutine(MoveRoutines(moveArray[moveID]));
    }

    public void MoveFollow()
    {
        BoundCalculator();
        StartCoroutine(FollowPlayer());
    }

    IEnumerator MoveRoutines(char dir)
    {
        if (dir == 'F')
        {
            if (isRevert)
                axis = Vector3.left;
            else
                axis = Vector3.right;
        }
        else if(dir == 'B')
        {
            if(isRevert)
                axis = Vector3.right;
            else
                axis = Vector3.left;
        }
        else if (dir == 'L')
        {
            if(isRevert)
                axis = Vector3.back;
            else
                axis = Vector3.forward;
        }
        else if (dir == 'R')
        {
            if(isRevert)
                axis = Vector3.forward;
            else
                axis = Vector3.back;
        }
        else if(dir == 'C')
        {
            isCircle = true;
            moveID = 0;
        }
        Debug.Log(moveID);
        Step();
        while(rolling)
        {
            yield return null;
        }
        if (!isRevert && moveID < moveArray.Length - 1)
        {
            isRevert = false;
            moveID++;
        }
        else if (!isRevert && moveID == moveArray.Length - 1)
        {
            isRevert = true;
        }
        else if(isRevert && moveID <= 0)
        {
            isRevert = false;
            moveID = 0;
        }
        else if(isRevert && moveID <= 1)
        {
            moveID = 0;
        }
        else
        {
            moveID--;
        }
        yield return new WaitForSeconds(0.2f);
        if (gameController.isStartGame)
        {
            StartCoroutine(MoveRoutines(moveArray[moveID]));
        }
    }

    IEnumerator FollowPlayer()
    {
        var randomDir = UnityEngine.Random.Range(0, 100);
        if (randomDir > 50)
        {
            if (Mathf.Abs(center.x - gameController.transform.position.x) > 0.6f && Mathf.Abs(center.x - gameController.transform.position.x) > Mathf.Abs(center.z - gameController.transform.position.z))
            {
                axis = new Vector3(0, 0, (center.x - gameController.transform.position.x)).normalized;
                if (axis == lastRandomChoice * -1 && Mathf.Abs(center.z - gameController.transform.position.z) > 0.6f)
                {
                    axis = new Vector3((gameController.transform.position.z - center.z), 0, 0).normalized;
                }
            }
            else if (Mathf.Abs(center.z - gameController.transform.position.z) > 0.6f && Mathf.Abs(center.z - gameController.transform.position.z) > Mathf.Abs(center.x - gameController.transform.position.x))
            {
                axis = new Vector3((gameController.transform.position.z - center.z), 0, 0).normalized;
                if (axis == lastRandomChoice * -1 && Mathf.Abs(center.x - gameController.transform.position.x) > 0.6f)
                {
                    axis = new Vector3(0, 0, (center.x - gameController.transform.position.x)).normalized;
                }
            }
        }
        else
        {
            if (Mathf.Abs(center.z - gameController.transform.position.z) > 0.6f && Mathf.Abs(center.z - gameController.transform.position.z) > Mathf.Abs(center.x - gameController.transform.position.x))
            {
                axis = new Vector3((gameController.transform.position.z - center.z), 0, 0).normalized;
                if (axis == lastRandomChoice * -1 && Mathf.Abs(center.x - gameController.transform.position.x) > 0.6f)
                {
                    axis = new Vector3(0, 0, (center.x - gameController.transform.position.x)).normalized;
                }
            }
            else if (Mathf.Abs(center.x - gameController.transform.position.x) > 0.6f && Mathf.Abs(center.x - gameController.transform.position.x) > Mathf.Abs(center.z - gameController.transform.position.z))
            {
                axis = new Vector3(0, 0, (center.x - gameController.transform.position.x)).normalized;
                if (axis == lastRandomChoice * -1 && Mathf.Abs(center.z - gameController.transform.position.z) > 0.6f)
                {
                    axis = new Vector3((gameController.transform.position.z - center.z), 0, 0).normalized;
                }
            }
        }
        Step();
        while (rolling)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        if (!gameController.isSwitchPhase && gameController.isStartGame)
        {
            StartCoroutine(FollowPlayer());
        }
    }
}
