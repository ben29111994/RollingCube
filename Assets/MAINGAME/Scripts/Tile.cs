using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    public int ID;
    public Color tileColor;
    private Renderer meshRenderer;
    public GameObject explodeEffect;
    public GameObject enemyPrefab;
    GameController gameController;
    bool isCheck = false;
    public bool isStandOn = false;

    private void OnEnable()
    {
        var gameControllerGet = GameObject.FindGameObjectWithTag("Player");
        gameController = gameControllerGet.GetComponent<GameController>();
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        if (meshRenderer == null)
            meshRenderer = GetComponent<Renderer>();
    }

    public void SetTransfrom(Vector3 pos,Vector3 scale)
    {
        transform.localPosition = pos;
        transform.localScale = new Vector3(scale.x,scale.y,scale.z);
    }

    public void SetColor(Color inputColor)
    {
        tileColor = inputColor;
        //Player
        if (inputColor == new Color32(0, 255, 0, 255))
        {
            //Debug.Log("Player: " + ColorUtility.ToHtmlStringRGB(inputColor));
            inputColor = Color.white;
            gameController.transform.position = new Vector3(transform.localPosition.x, 1, transform.localPosition.z);
            tag = "Task1";
            transform.position = new Vector3(transform.position.x, transform.position.y - 10, transform.position.z);
            transform.localScale = new Vector3(1, 21, 1);
            gameController.progressCount1++;
            gameController.listTask1.Add(gameObject);
        }
        //Bot Follow
        else if (inputColor == new Color32(63, 72, 204, 255))
        {
            //Debug.Log("Bot Follow: " + ColorUtility.ToHtmlStringRGB(inputColor));
            inputColor = Color.white;
            if (!gameController.isUsingEnemy)
            {
                gameController.enemy.SetActive(true);
                gameController.isUsingEnemy = true;
                gameController.enemy.transform.position = new Vector3(transform.localPosition.x, 1, transform.localPosition.z);
                gameController.enemyFollowList.Add(gameController.enemy);
                if (transform.position.z < -10)
                {
                    tag = "Task1";
                    gameController.progressCount1++;
                    gameController.currentEnemy = gameController.enemy;
                }
                else
                {
                    tag = "Task2";
                    gameController.progressCount2++;
                }
            }
            else
            {
                enemyPrefab = gameController.enemy;
                var enemy = Instantiate(enemyPrefab);
                enemy.transform.parent = null;
                enemy.transform.position = new Vector3(transform.localPosition.x, 1, transform.localPosition.z);
                gameController.enemyFollowList.Add(enemy);
                if (transform.position.z < -10)
                {
                    tag = "Task1";
                    gameController.progressCount1++;
                    gameController.currentEnemy = enemy;
                }
                else
                {
                    tag = "Task2";
                    gameController.progressCount2++;
                }
            }
            transform.position = new Vector3(transform.position.x, transform.position.y - 10, transform.position.z);
            transform.localScale = new Vector3(1, 21, 1);
            gameController.listTask1.Add(gameObject);
        }
        //Bot Patrol
        else if (inputColor == new Color32(255, 0, 0, 255))
        {
            //Debug.Log("Bot Patrol: " + ColorUtility.ToHtmlStringRGB(inputColor));
            inputColor = Color.white;
            if (!gameController.isUsingEnemyPatrol)
            {
                gameController.enemyPatrol.SetActive(true);
                gameController.isUsingEnemyPatrol = true;
                gameController.enemyPatrol.transform.position = new Vector3(transform.localPosition.x, 1, transform.localPosition.z);
                gameController.enemyPatrolList.Add(gameController.enemyPatrol);
                if (transform.position.z < -10)
                {
                    tag = "Task1";
                    gameController.progressCount1++;
                    gameController.currentEnemy = gameController.enemyPatrol;
                }
                else
                {
                    tag = "Task2";
                    gameController.progressCount2++;
                }
            }
            else
            {
                enemyPrefab = gameController.enemyPatrol;
                var enemy = Instantiate(enemyPrefab);
                enemy.transform.parent = null;
                enemy.transform.position = new Vector3(transform.localPosition.x, 1, transform.localPosition.z);
                gameController.enemyPatrolList.Add(enemy);
                if (transform.position.z < -10)
                {
                    tag = "Task1";
                    gameController.progressCount1++;
                    gameController.currentEnemy = enemy;
                }
                else
                {
                    tag = "Task2";
                    gameController.progressCount2++;
                }
            }
            transform.position = new Vector3(transform.position.x, transform.position.y - 10, transform.position.z);
            transform.localScale = new Vector3(1, 21, 1);
            gameController.listTask1.Add(gameObject);
        }
        //Task 1
        else if (inputColor == Color.black)
        {
            //Debug.Log("Map 1: " + ColorUtility.ToHtmlStringRGB(inputColor));
            inputColor = Color.white;
            transform.position = new Vector3(transform.position.x, transform.position.y - 10, transform.position.z);
            transform.localScale = new Vector3(1, 21, 1);
            tag = "Task1";
            gameController.progressCount1++;
            gameController.listTask1.Add(gameObject);
        }
        //Task 2
        else if (inputColor == new Color32(155, 155, 155, 255))
        {
            //Debug.Log("Map 2: " + ColorUtility.ToHtmlStringRGB(inputColor));
            inputColor = Color.white;
            transform.position = new Vector3(transform.position.x, transform.position.y - 10, transform.position.z);
            transform.localScale = new Vector3(1, 21, 1);
            tag = "Task2";
            gameController.progressCount2++;
            gameController.listTask2.Add(gameObject);
        }
        //Bridge
        else if (inputColor == new Color32(255, 0, 255, 255))
        {
            //Debug.Log("Cau: " + ColorUtility.ToHtmlStringRGB(inputColor));
            inputColor = Color.white;
            transform.position = new Vector3(transform.position.x, transform.position.y - 10, transform.position.z);
            transform.localScale = new Vector3(1, 21, 1);
            tag = "Task2";
            gameController.progressCount2++;
            gameController.listTask2.Add(gameObject);
        }
        //Point1
        else if (inputColor == new Color32(0, 255, 255, 255))
        {
            //Debug.Log("Dau cau: " + ColorUtility.ToHtmlStringRGB(inputColor));
            inputColor = Color.white;

            //gameController.enemy.SetActive(true);
            //gameController.isUsingEnemy = true;
            //gameController.enemy.transform.position = new Vector3(transform.localPosition.x, 1, transform.localPosition.z);
            //gameController.enemyFollowList.Add(gameController.enemy);

            transform.position = new Vector3(transform.position.x, transform.position.y - 10, transform.position.z);
            transform.localScale = new Vector3(1, 21, 1);
            tag = "Task1";
            gameController.progressCount1++;
            gameController.switchPoint1 = transform.localPosition;
            gameController.listTask1.Add(gameObject);
        }
        //Point2
        else if (inputColor == new Color32(255, 255, 0, 255))
        {
            //Debug.Log("Cuoi cau: " + ColorUtility.ToHtmlStringRGB(inputColor));
            inputColor = Color.white;

            //enemyPrefab = gameController.enemy;
            //var enemy = Instantiate(enemyPrefab);
            //enemy.transform.parent = null;
            //enemy.transform.position = new Vector3(transform.localPosition.x, 1, transform.localPosition.z);
            //gameController.enemyFollowList.Add(enemy);

            transform.position = new Vector3(transform.position.x, transform.position.y - 10, transform.position.z);
            transform.localScale = new Vector3(1, 21, 1);
            tag = "Task2";
            gameController.progressCount2++;
            gameController.switchPoint2 = transform.localPosition;
            gameController.listTask2.Add(gameObject);
        }
        //Wall
        else if (inputColor == Color.white)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 20, transform.position.z);
            transform.localScale = new Vector3(1, 21, 1);
            inputColor = Color.white;
            tag = "Wall";
            gameObject.layer = 10;
            GetComponent<BoxCollider>().size = new Vector3(0.99f, 50, 0.99f);
            GetComponent<MeshRenderer>().enabled = false;
            Destroy(transform.GetChild(0).gameObject);
        }
        meshRenderer.material.color = inputColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && CompareTag("Task1") && !isCheck)
        {
            var temp = Instantiate(explodeEffect, new Vector3(transform.position.x, 0.6f, transform.position.z), Quaternion.identity);
            var getColor = temp.GetComponent<ParticleSystem>().main;
            getColor.startColor = gameController.theme;
            isCheck = true;
            gameController.Scoring(other.transform.position);
            gameController.progressCount1--;
            if(gameController.progressCount1 <= 0)
            {
                gameController.SwitchPhase();
            }
            tileColor = gameController.theme;
            meshRenderer.material.color = tileColor;
            isStandOn = true;
        }
        if (other.CompareTag("Player") && CompareTag("Task2") && !isCheck)
        {
            var temp = Instantiate(explodeEffect, new Vector3(transform.position.x, 0.6f, transform.position.z), Quaternion.identity);
            var getColor = temp.GetComponent<ParticleSystem>().main;
            getColor.startColor = gameController.theme;
            isCheck = true;
            gameController.Scoring(other.transform.position);
            gameController.progressCount2--;
            tileColor = gameController.theme;
            meshRenderer.material.color = tileColor;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isStandOn = false;
        }
    }
}
