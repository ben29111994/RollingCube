using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using UnityEngine.EventSystems;
using GPUInstancer;

public class GameController : MonoBehaviour
{
    [Header("Variable")]
    public static GameController instance;
    float h;
    float v;
    public static bool isDrag = false;
    public float speed;
    public GameObject center;
    public GameObject forwardRotationPoint;
    public GameObject backRotationPoint;
    public GameObject leftRotationPoint;
    public GameObject rightRotationPoint;
    public Transform forwardPoint;
    public Transform backPoint;
    public Transform leftPoint;
    public Transform rightPoint;
    public bool rolling;
    int maxLevel = 100;
    public bool isStartGame = false;
    Vector3 point;
    Vector3 axis;
    GameObject playerSpawn;
    public int progressCount1;
    public int progressCount2;
    public bool isControl = true;
    public List<Color> mainColor = new List<Color>();
    public Color theme;
    bool canL = true;
    bool canR = true;
    bool canF = true;
    bool canB = true;
    public bool isUsingEnemy = false;
    public bool isUsingEnemyPatrol = false;
    public bool isBotFollowTask1;
    public bool isBotFollowTask2;
    public Vector3 switchPoint1;
    public Vector3 switchPoint2;
    public bool isSwitchPhase = false;
    public bool isPhase2 = false;
    public GameObject currentEnemy;
    Vector3 lastAxis;
    float a = 0;
    Color32 currentColor;
    int currentColorIndex;
    bool isPassPoint1 = false;
    public int currentMove;
    bool isClearMap = false;

    [Header("UI")]
    public GameObject winPanel;
    public GameObject losePanel;
    public Slider levelProgress;
    public Text currentLevelText;
    public Text nextLevelText;
    public Text moneyText;
    int currentLevel;
    public static int score;
    public static int money;
    public int progress;
    public Canvas canvas;
    public GameObject shopMenu;
    public GameObject gameMenu;
    public Text bestScoreText;
    public GameObject bonusPopup;
    public InputField levelInput;
    public Text title;

    [Header("Objects")]
    public GameObject plusVarPrefab;
    public GameObject conffeti;
    public GameObject playerPrefab;
    public GameObject enemy;
    public GameObject enemyPatrol;
    public List<GameObject> enemyPatrolList = new List<GameObject>();
    public List<GameObject> enemyFollowList = new List<GameObject>();
    public ParticleSystem bloodExplode;
    public List<GameObject> botList = new List<GameObject>();
    public List<GameObject> listTask1 = new List<GameObject>();
    public List<GameObject> listTask2 = new List<GameObject>();

    private void OnEnable()
    {
        //PlayerPrefs.DeleteAll();
        Application.targetFrameRate = 60;
        theme = mainColor[Random.Range(0, mainColor.Count)];
        playerPrefab.GetComponent<Renderer>().material.color = theme;
        var temp1 = Instantiate(botList[/*Random.Range(0, botList.Count)*/11]);
        temp1.transform.parent = enemy.transform;
        currentMove = PlayerPrefs.GetInt("currentMove");
        //var temp2 = Instantiate(botList[Random.Range(0, botList.Count)]);
        //temp2.transform.parent = enemyPatrol.transform;
    }

    void Start()
    {
        var getColor = bloodExplode.main;
        getColor.startColor = theme;
        title.color = theme;
        currentLevel = PlayerPrefs.GetInt("currentLevel");
        currentLevelText.text = currentLevel.ToString();
        nextLevelText.text = (currentLevel + 1).ToString();
        isControl = true;
        isDrag = false;
        progress = progressCount1 + progressCount2;
        levelProgress.maxValue = progress;
        levelProgress.value = 0;
        //StartCoroutine(delayChangeFogColor());
    }

    public void BoundCalculator()
    {
        center.transform.position = transform.position;
        var xList = new List<float>();
        var yList = new List<float>();
        var zList = new List<float>();
        xList.Add(transform.position.x);
        yList.Add(transform.position.y);
        zList.Add(transform.position.z);

        forwardRotationPoint.transform.position = new Vector3((xList[0] + xList[zList.Count - 1]) / 2, yList[0], zList[zList.Count - 1] + 0.5f);
        backRotationPoint.transform.position = new Vector3((xList[0] + xList[zList.Count - 1]) / 2, yList[0], zList[0] - 0.5f);
        leftRotationPoint.transform.position = new Vector3(xList[0] - 0.5f, yList[0], (zList[0] + zList[zList.Count - 1]) / 2);
        rightRotationPoint.transform.position = new Vector3(xList[xList.Count - 1] + 0.5f, yList[0], (zList[0] + zList[zList.Count - 1]) / 2);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, backPoint.position, out hit, 5))
        {
            if (hit.collider.tag == "Wall" || (hit.collider.tag == "Task2" && !isPhase2))
            {
                canB = false;
            }
            else
            {
                canB = true;
            }
        }
        if (Physics.Raycast(transform.position, forwardPoint.position, out hit, 5))
        {
            if (hit.collider.tag == "Wall" || (hit.collider.tag == "Task2" && !isPhase2))
            {
                canF = false;
            }
            else
            {
                canF = true;
            }
        }
        if (Physics.Raycast(transform.position, rightPoint.position, out hit, 5))
        {
            if (hit.collider.tag == "Wall" || (hit.collider.tag == "Task2" && !isPhase2))
            {
                canR = false;
            }
            else
            {
                canR = true;
            }
        }
        if (Physics.Raycast(transform.position, leftPoint.position, out hit, 5))
        {
            if (hit.collider.tag == "Wall" || (hit.collider.tag == "Task2" && !isPhase2))
            {
                canL = false;
            }
            else
            {
                canL = true;
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && gameMenu.activeSelf)
        {
            ButtonStartGame();
        }
        //Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(transform.position.x, 20, transform.position.z - 5), Time.deltaTime * 5);
        if (isStartGame && !isSwitchPhase)
        {
            if (Input.GetMouseButtonUp(0))
            {
                h = 0;
                v = 0;
                isDrag = false;
            }

            if (Input.GetMouseButton(0))
            {
                while (rolling)
                {
                    return;
                }
#if UNITY_EDITOR
                h = Input.GetAxis("Mouse X");
                v = Input.GetAxis("Mouse Y");
#endif
#if UNITY_IOS
                if (Input.touchCount > 0)
                {
                    h = Input.touches[0].deltaPosition.x / 8;
                    v = Input.touches[0].deltaPosition.y / 8;
                }
#endif
                if (v > 0 && Mathf.Abs(v) - Mathf.Abs(h) > 0 && canF && Mathf.Abs(v) > 0.1f)
                {
                    var temp = Vector3.right;
                    if (rolling)
                    {
                        if (temp != lastAxis)
                        {
                            a = Time.deltaTime * speed * 2;
                        }
                        return;
                    }
                    axis = Vector3.right;
                    point = forwardRotationPoint.transform.position;
                    isDrag = true;
                    StartCoroutine(Roll());
                }
                else if (v < 0 && Mathf.Abs(v) - Mathf.Abs(h) > 0 && canB && Mathf.Abs(v) > 0.1f)
                {
                    var temp = Vector3.left;
                    if (rolling)
                    {
                        if (temp != lastAxis)
                        {
                            a = Time.deltaTime * speed * 2;
                        }
                        return;
                    }
                    axis = Vector3.left;
                    point = backRotationPoint.transform.position;
                    isDrag = true;
                    StartCoroutine(Roll());
                }
                else if (h > 0 && Mathf.Abs(h) - Mathf.Abs(v) > 0 && canR && Mathf.Abs(h) > 0.1f)
                {
                    var temp = Vector3.back;
                    if (rolling)
                    {
                        if (temp != lastAxis)
                        {
                            a = Time.deltaTime * speed * 2;
                        }
                        return;
                    }
                    axis = Vector3.back;
                    point = rightRotationPoint.transform.position;
                    isDrag = true;
                    StartCoroutine(Roll());
                }
                else if (h < 0 && Mathf.Abs(h) - Mathf.Abs(v) > 0 && canL && Mathf.Abs(h) > 0.1f)
                {
                    var temp = Vector3.forward;
                    if (rolling)
                    {
                        if (temp != lastAxis)
                        {
                            a = Time.deltaTime * speed * 2;
                        }
                        return;
                    }
                    axis = Vector3.forward;
                    point = leftRotationPoint.transform.position;
                    isDrag = true;
                    StartCoroutine(Roll());
                }
                else if (isDrag)
                {
                    if (axis == Vector3.right && canF)
                    {
                        point = forwardRotationPoint.transform.position;
                        StartCoroutine(Roll());
                    }
                    else if (axis == Vector3.left && canB)
                    {
                        point = backRotationPoint.transform.position;
                        StartCoroutine(Roll());
                    }
                    else if (axis == Vector3.back && canR)
                    {
                        point = rightRotationPoint.transform.position;
                        StartCoroutine(Roll());
                    }
                    else if (axis == Vector3.forward && canL)
                    {
                        point = leftRotationPoint.transform.position;
                        StartCoroutine(Roll());
                    }
                }
            }
        }
    }

    public void Step()
    {
        if (axis == Vector3.right && canF)
        {
            point = forwardRotationPoint.transform.position;
            StartCoroutine(Roll());
        }
        else if (axis == Vector3.left && canB)
        {
            point = backRotationPoint.transform.position;
            StartCoroutine(Roll());
        }
        else if (axis == Vector3.back && canR)
        {
            point = rightRotationPoint.transform.position;
            StartCoroutine(Roll());
        }
        else if (axis == Vector3.forward && canL)
        {
            point = leftRotationPoint.transform.position;
            StartCoroutine(Roll());
        }
        else
            rolling = false;
    }

    private IEnumerator Roll()
    {
        float angle = 180;
        a = Time.deltaTime * speed;
        rolling = true;

        while (angle > 0)
        {
            transform.RotateAround(point, axis, a);
            angle -= a;
            yield return null;
        }
        transform.RotateAround(point, axis, angle);

        center.transform.position = transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, backPoint.position, out hit, 5))
        {
            if (hit.collider.CompareTag("Wall") || (hit.collider.tag == "Task2" && !isPhase2))
            {
                canB = false;
            }
            else
            {
                canB = true;
            }
        }
        if (Physics.Raycast(transform.position, forwardPoint.position, out hit, 5))
        {
            if (hit.collider.CompareTag("Wall") || (hit.collider.tag == "Task2" && !isPhase2))
            {
                canF = false;
            }
            else
            {
                canF = true;
            }
        }
        if (Physics.Raycast(transform.position, rightPoint.position, out hit, 5))
        {
            if (hit.collider.CompareTag("Wall") || (hit.collider.tag == "Task2" && !isPhase2))
            {
                canR = false;
            }
            else
            {
                canR = true;
            }
        }
        if (Physics.Raycast(transform.position, leftPoint.position, out hit, 5))
        {
            if (hit.collider.CompareTag("Wall") || (hit.collider.tag == "Task2" && !isPhase2))
            {
                canL = false;
            }
            else
            {
                canL = true;
            }
        }
        rolling = false;
    }

    float ClampAngle(float angle, float from, float to)
    {
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360 + from);
        return Mathf.Min(angle, to);
    }

    public void Scoring(Vector3 pos)
    {
        PlusEffect(pos);
        progress--;
        levelProgress.value = levelProgress.maxValue - progress;
        if (progress == 0)
        {
            Win();
        }
    }

    public void SwitchPhase()
    {
        isSwitchPhase = true;
        //isStartGame = false;
        MapFlowEffect();
    }

    IEnumerator FollowSwitchPoint(Vector3 point)
    {
        while (rolling)
        {
            yield return null;
        }
        if (Mathf.Abs(center.transform.position.z - point.z) > 0.5f && canF && canB)
        {
            axis = new Vector3((point.z - center.transform.position.z), 0, 0).normalized;
        }
        else if (Mathf.Abs(center.transform.position.x - point.x) > 0.5f && canL && canR)
        {
            axis = new Vector3(0, 0, (center.transform.position.x - point.x)).normalized;
        }
        else
        {
            Debug.Log("Trigger");
            if (canF)
            {
                axis = Vector3.right;
            }
            else if (canL)
            {
                axis = Vector3.forward;
            }
            else if (canR)
            {
                axis = Vector3.back;
            }
            else if (canB)
            {
                axis = Vector3.left;
            }
        }
        Step();
        while (rolling)
        {
            yield return null;
        }
        if (Mathf.Abs(center.transform.position.x - switchPoint1.x) > 0.5f || Mathf.Abs(center.transform.position.z - switchPoint1.z) > 0.5f && !isPassPoint1)
        {
            isPhase2 = true;
            Debug.Log("1");
            StartCoroutine(FollowSwitchPoint(switchPoint1));
        }
        else if (Mathf.Abs(center.transform.position.x - switchPoint2.x) > 0.5f || Mathf.Abs(center.transform.position.z - switchPoint2.z) > 0.5f)
        {
            conffeti.SetActive(true);
            isPassPoint1 = true;
            Camera.main.transform.DOMoveZ(10.5f, 2);
            Debug.Log("2");
            StartCoroutine(FollowSwitchPoint(switchPoint2));
        }
        else
        {
            Debug.Log("3");
            isSwitchPhase = false;
            isStartGame = true;
            enemyFollowList.TrimExcess();
            enemyFollowList[0].GetComponent<Enemy>().MoveFollow();
            //if (currentEnemy == "Enemy")
            //{
            //    enemy.transform.DOMoveY(-50, 1);
            //    Destroy(enemy, 1);
            //    enemyPatrol.GetComponent<Enemy>().MovePatrol();
            //}
            //else if (currentEnemy == "EnemyPatrol")
            //{
            //    enemyPatrol.transform.DOMoveY(-50, 1);
            //    Destroy(enemyPatrol, 1);
            //    enemy.GetComponent<Enemy>().MoveFollow();
            //}
        }

    }

    void MapFlowEffect()
    {
        var time = 0.2f;
        foreach (var item in listTask1)
        {
            if (!item.GetComponent<Tile>().isStandOn)
            {
                item.transform.DOMoveY(item.transform.position.y + 2/*Random.Range(1,3)*/, time/*Random.Range(0.1f, 0.3f)*/).SetLoops(2, LoopType.Yoyo);
                item.GetComponent<MeshRenderer>().material.DOColor(Color.white, time).SetLoops(2, LoopType.Yoyo);
            }
            time += 0.003f;
        }
        var getColor = bloodExplode.main;
        getColor.startColor = Color.red;
        bloodExplode.transform.position = currentEnemy.transform.position;
        bloodExplode.Play();
        enemyFollowList.Remove(currentEnemy);
        Destroy(currentEnemy);
        StartCoroutine(FollowSwitchPoint(switchPoint1));
    }

    public void Lose()
    {
        if (isStartGame)
        {
            isStartGame = false;
            losePanel.SetActive(true);
        }
    }

    public void Win()
    {
        if (isStartGame)
        {
            losePanel.SetActive(false);
            isStartGame = false;
            currentLevel++;
            if (currentLevel > LevelGenerator.instance.list2DMaps.Count - 1)
            {
                currentLevel = 0;
            }
            PlayerPrefs.SetInt("currentLevel", currentLevel);
            PlayerPrefs.SetInt("currentMove", currentMove);
            winPanel.SetActive(true);
        }
    }

    public void PlusEffect(Vector3 pos)
    {
        if (!UnityEngine.iOS.Device.generation.ToString().Contains("5"))
        {
            MMVibrationManager.Haptic(HapticTypes.LightImpact);
        }
        var plusVar = Instantiate(plusVarPrefab);
        plusVar.transform.SetParent(canvas.transform);
        plusVar.transform.localScale = new Vector3(1, 1, 1);
        plusVar.transform.position = worldToUISpace(canvas, pos);
        plusVar.SetActive(true);
        plusVar.transform.DOMoveY(plusVar.transform.position.y + Random.Range(50, 90), 0.5f);
        Destroy(plusVar, 0.5f);
    }

    public Vector3 worldToUISpace(Canvas parentCanvas, Vector3 worldPos)
    {
        //Convert the world for screen point so that it can be used with ScreenPointToLocalPointInRectangle function
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 movePos;

        //Convert the screenpoint to ui rectangle local point
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out movePos);
        //Convert the local point to world point
        return parentCanvas.transform.TransformPoint(movePos);
    }

    public void ButtonStartGame()
    {
        //AddRemoveInstances.instance.Setup();
        //MapFlowEffect();
        BoundCalculator();
        gameMenu.SetActive(false);
        isStartGame = true;
        if (enemyPatrolList.Count > 0)
        {
            foreach (var item in enemyPatrolList)
            {
                item.GetComponent<Enemy>().MovePatrol();
            }
        }
        if (enemyFollowList.Count > 0)
        {
            if (enemyFollowList[0].transform.position.z < 10)
            {
                enemyFollowList[0].GetComponent<Enemy>().MoveFollow();
            }
            else
                enemyFollowList[1].GetComponent<Enemy>().MoveFollow();
        }
        //if (enemy.transform.position.z < 0)
        //{
        //    enemy.GetComponent<Enemy>().MoveFollow();
        //    currentEnemy = "Enemy";
        //}
        //else if (enemyPatrol.transform.position.z < 0)
        //{
        //    enemyPatrol.GetComponent<Enemy>().MovePatrol();
        //    currentEnemy = "EnemyPatrol";
        //}
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(0);
    }

    public void OnChangeMap()
    {
        if (levelInput != null)
        {
            int level = int.Parse(levelInput.text.ToString());
            PlayerPrefs.SetInt("currentLevel", level);
            SceneManager.LoadScene(0);
        }
    }

    public void ButtonNextLevel()
    {
        //currentLevel++;
        //PlayerPrefs.SetInt("currentLevel", currentLevel);
        Win();
        SceneManager.LoadScene(0);
    }

    public void ButtonPreviousLevel()
    {
        currentLevel--;
        PlayerPrefs.SetInt("currentLevel", currentLevel);
        SceneManager.LoadScene(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            if (isStartGame)
            {
                Lose();
                var getColor = bloodExplode.main;
                getColor.startColor = theme;
                bloodExplode.transform.position = transform.position;
                bloodExplode.Play();
                GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    IEnumerator delayChangeFogColor()
    {
        currentColorIndex++;
        if (currentColorIndex > mainColor.Count - 1)
        {
            currentColorIndex = 0;
        }
        currentColor = mainColor[currentColorIndex];

        DOTween.To(() => RenderSettings.fogColor, x => RenderSettings.fogColor = x, currentColor, 5);
        yield return new WaitForSeconds(5);
        StartCoroutine(delayChangeFogColor());
    }
}
