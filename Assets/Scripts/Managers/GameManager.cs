using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Fields

    public Tank Tank;
    public Area[] AreaDatas;
    public Area[] Areas;
    public List<AreaController> AreaControllers;
    public AreaController LocationArea;
    private AreaController mLastPlace;

    public GameObject[] Panels;
    public GameObject FrangibleObjeTransform;
    public Image EnergyImage;
    public Button[] GameOverMenuButton;
    public Text ScoreText;
    public Text GameOverScoreText;
    public Text GameOverHighScoreText;

    public float Value;
    private float mTurnValue;

    public bool IsPause;
    public bool IsRetry;
    private bool mIsAdvence;
    private bool mIsTurn;
    private bool mIsFinish;

    #endregion

    #region Unity Methods

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (EnergyImage.fillAmount <= 1.0f)
        {
            EnergyImage.fillAmount = Value / 10.0f;
        }

        if (Value > 0 && Value < 9.75f)
        {
            Value -= Time.deltaTime;
        }
    }

    #endregion

    #region Public Methods

    public void TankAdvenceArea()
    {
        if (!mIsAdvence)
        {
            if (Tank.transform.position == new Vector3(LocationArea.MiddlePoints[0].position.x, 0.85f, LocationArea.MiddlePoints[0].position.z))
            {
                mIsTurn = true;
                mIsAdvence = true;
            }
            else
            {
                Tank.transform.position = Vector3.MoveTowards(Tank.transform.position, new Vector3(LocationArea.MiddlePoints[0].position.x, 0.85f, LocationArea.MiddlePoints[0].position.z), 0.25f);
            }
        }

        if (mIsTurn)
        {
            int valueY = mLastPlace.transform.rotation.y > 0 ? 1 : -1;

            if (Tank.transform.rotation.y <= mLastPlace.transform.rotation.y && valueY == -1 || Tank.transform.rotation.y >= mLastPlace.transform.rotation.y && valueY == 1)
            {
                mIsTurn = false;
                mIsFinish = true;
            }
            else
            {
                TankTurn(valueY);
            }
        }

        if (mIsFinish)
        {
            if (Tank.transform.position == mLastPlace.MiddlePoints[1].transform.position)
            {
                LocationArea = AreaControllers.IndexOf(LocationArea) + 1 < AreaControllers.Count ? AreaControllers[AreaControllers.IndexOf(LocationArea) + 1] : LocationArea;
                mLastPlace = AreaControllers.IndexOf(mLastPlace) + 1 < AreaControllers.Count ? AreaControllers[AreaControllers.IndexOf(mLastPlace) + 1] : mLastPlace;
                mIsFinish = false;
                mIsAdvence = false;
                IsPause = false;
            }
            else
            {
                Tank.transform.position = Vector3.MoveTowards(Tank.transform.position, mLastPlace.MiddlePoints[1].position, 0.25f);
            }
        }
    }

    #endregion

    #region Private Methods

    private void Initialize()
    {
        AreaControllers = new List<AreaController>();
        GetRandomNumberOfAreaData();
        EpisodeCreate();
    }

    private void EpisodeCreate()
    {
        for (int i = 0; i < Areas.Length; i++)
        {
            AreaKnee(i);
        }

        mLastPlace = AreaControllers[1];
        TankPlacementCurrentField();
    }

    private void AreaKnee(int areaId)
    {
        int turnRight = 1;

        AreaController areaPrefab = Instantiate(Areas[areaId].AreaPrefab).GetComponent<AreaController>();
        areaPrefab.GameManager = this;
        areaPrefab.AreaData = Areas[areaId];
        AreaControllers.Add(areaPrefab);

        if (areaId == 0)
        {
            LocationArea = AreaControllers[areaId];
            areaPrefab.transform.position = new Vector3(0, 0, 0);
            areaPrefab.transform.rotation = this.transform.rotation;
        }
        else
        {
            turnRight = Random.Range(1, 1000) % 2 == 0 ? 1 : -1;
            int turnValue = Random.Range(4, 8);

            AreaController previousArea = AreaControllers[areaId - 1];
            areaPrefab.transform.position = new Vector3(previousArea.transform.position.x + (turnValue * turnRight), previousArea.transform.position.y, previousArea.transform.position.z + 18);
            areaPrefab.transform.rotation = Quaternion.Euler(0, turnValue * 3 * turnRight, 0);
        }
    }

    private void TankPlacementCurrentField()
    {
        Tank.transform.position = new Vector3(LocationArea.MiddlePoints[1].transform.position.x, LocationArea.MiddlePoints[1].transform.position.y, LocationArea.MiddlePoints[1].transform.position.z);
        Tank.transform.rotation = LocationArea.transform.rotation;
    }

    private void TankTurn(int direction)
    {
        mTurnValue += direction * (Tank.transform.rotation.y + Time.deltaTime * 40);
        Tank.transform.rotation = Quaternion.Euler(0, mTurnValue, 0);
    }

    private void GetRandomNumberOfAreaData()
    {
        Areas = null;
        int dataCount = Random.Range(2, AreaDatas.Length);
        Areas = new Area[dataCount];

        for (int i = 0; i < dataCount; i++)
        {
            Areas[i] = AreaDatas[Random.Range(0, AreaDatas.Length)];
        }
    }

    #endregion

    #region Events

    public void OnPlayButtonClicked()
    {
        Panels[0].SetActive(false);
        Panels[2].SetActive(true);
    }

    public void OnContinueButtonClicked()
    {
        if (!IsRetry)
        {
            StartCoroutine(RetryGameStart());
        }
    }

    public void OnRetryButtonClicked()
    {
        if (!IsRetry)
        {
            ScoreText.text = 0.ToString();
            StartCoroutine(RetryGameStart());
        }
    }

    public void OnPowerButtonClicked()
    {
        if (Value > 9.75f && !IsPause && LocationArea.IsFrangibleObjesCreatived)
        {
            Tank.Shoot(Value);
            Value = 0.0f;
        }
    }

    #endregion

    #region Enumerators

    private IEnumerator RetryGameStart()
    {
        Value = 0.0f;
        IsRetry = true;

        for (int i = 0; i < AreaControllers.Count; i++)
        {
            Destroy(AreaControllers[i].gameObject);
        }

        AreaControllers = new List<AreaController>();

        LocationArea = null;
        LocationArea = null;
        mLastPlace = null;

        GetRandomNumberOfAreaData();
        EpisodeCreate();
        yield return new WaitForSeconds(0.5f);
        IsPause = false;
        IsRetry = false;
        Panels[1].SetActive(false);
        Panels[2].SetActive(true);
    }

    #endregion
}