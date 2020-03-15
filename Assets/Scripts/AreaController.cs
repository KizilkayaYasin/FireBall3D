using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaController : MonoBehaviour
{
    #region Enums

    public enum TrapDistanceType
    {
        VeryClose,
        Near,
        Middle,
        Far,
        TooFar
    }
    public enum DirectionType
    {
        Up,
        Down,
        Left,
        Right,
        UpRight,
        UpLeft,
        DownRight,
        DownLeft
    }

    #endregion

    #region Fields

    public Dictionary<TrapDistanceType, float> TrapDistanceValue;
    public Dictionary<TrapDistanceType, int> TrapDistanceByScale;
    public Area AreaData;
    public TextMesh FrangibleObjeCountText;
    public List<GameObject> Traps;
    public List<GameObject> FrangibleObjes;
    public Transform[] MiddlePoints;
    public bool IsFrangibleObjesCreatived;
    public bool IsCreate;
    public GameManager GameManager;

    #endregion

    #region Unity Methods

    public void Start()
    {
        Initialized();
    }

    private void Update()
    {
        if (GameManager != null && GameManager.IsRetry == false)
        {
            if (!IsCreate && GameManager.LocationArea == this && !GameManager.IsPause)
            {
                StartCoroutine(FrangibleObjesCreate());
                IsCreate = true;
            }

            if (GameManager.IsPause && FrangibleObjes.Count == 0 && GameManager.LocationArea == this && GameManager.AreaControllers[GameManager.AreaControllers.Count - 1] != this)
            {
                if (Traps.Count > 0)
                {
                    StartCoroutine(TrapsSlowlyDestroy());
                }
                GameManager.TankAdvenceArea();
            }

            if (IsFrangibleObjesCreatived && FrangibleObjes.Count == 0 && GameManager.AreaControllers[GameManager.AreaControllers.Count - 1] == this)
            {
                GameManager.Panels[2].SetActive(false);
                GameManager.Panels[1].SetActive(true);
                GameManager.GameOverMenuButton[0].gameObject.SetActive(false);
                GameManager.GameOverMenuButton[1].gameObject.SetActive(true);

                if (int.Parse(GameManager.ScoreText.text.ToString()) > PlayerPrefs.GetInt("HighScore"))
                {
                    PlayerPrefs.SetInt("HighScore", int.Parse(GameManager.ScoreText.text.ToString()));
                }

                GameManager.GameOverScoreText.text = "Score :" + GameManager.ScoreText.text;

                IsFrangibleObjesCreatived = false;
            }
        }
    }

    #endregion

    #region Private Methods

    private void Initialized()
    {
        Traps = new List<GameObject>();
        TrapDistanceValue = new Dictionary<TrapDistanceType, float>()
        {
            {TrapDistanceType.VeryClose,2.5f},
            {TrapDistanceType.Near,3.0f},
            {TrapDistanceType.Middle,3.5f},
            {TrapDistanceType.Far,4.0f},
            {TrapDistanceType.TooFar,4.5f}
        };
        TrapDistanceByScale = new Dictionary<TrapDistanceType, int>()
        {
            {TrapDistanceType.VeryClose,75},
            {TrapDistanceType.Near,60},
            {TrapDistanceType.Middle,50},
            {TrapDistanceType.Far,60},
            {TrapDistanceType.TooFar,70}
        };
    }

    private void TrapsCreate()
    {
        for (int i = 0; i < AreaData.Barriers.Length; i++)
        {
            GameObject trap = TrapPool.Instance.Get().gameObject;
            trap.GetComponent<Trap>().GameManager = GameManager;
            TrapLocate(i, trap);
            trap.SetActive(true);
            Traps.Add(trap);
        }
    }

    private void TrapLocate(int TrapID, GameObject trapObje)
    {
        trapObje.GetComponent<Trap>().BarrierData = AreaData.Barriers[TrapID];
        trapObje.GetComponent<Trap>().Transform = MiddlePoints[0];
        Transform barrierTransform = trapObje.GetComponent<Trap>().BodyTransform;
        TrapDistanceType trapDistanceType = AreaData.Barriers[TrapID].TrapDistanceType;
        DirectionType directionType = AreaData.Barriers[TrapID].TrapDirectionType;
        float AddValue = TrapDistanceValue[trapDistanceType];
        barrierTransform.transform.localScale = new Vector3(AreaData.Barriers[TrapID].Size, barrierTransform.transform.localScale.y, barrierTransform.transform.localScale.z);
        float scaleValue = (barrierTransform.transform.localScale.x * TrapDistanceByScale[trapDistanceType] / 100);

        switch (directionType)
        {
            case DirectionType.Right:
                trapObje.transform.position = new Vector3(this.transform.position.x + AddValue, this.transform.position.y + 1, this.transform.position.z);
                break;
            case DirectionType.Left:
                trapObje.transform.position = new Vector3(this.transform.position.x - AddValue, this.transform.position.y + 1, this.transform.position.z);
                break;
            case DirectionType.Up:
                trapObje.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z + AddValue);
                break;
            case DirectionType.Down:
                trapObje.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z - AddValue);
                break;
            case DirectionType.UpRight:
                trapObje.transform.position = new Vector3(this.transform.position.x + (AddValue / 2 + 0.75f), this.transform.position.y + 1, this.transform.position.z + (AddValue / 2 + 0.75f));
                break;
            case DirectionType.UpLeft:
                trapObje.transform.position = new Vector3(this.transform.position.x - (AddValue / 2 + 0.75f), this.transform.position.y + 1, this.transform.position.z + (AddValue / 2 + 0.75f));
                break;
            case DirectionType.DownRight:
                trapObje.transform.position = new Vector3(this.transform.position.x + (AddValue / 2 + 0.75f), this.transform.position.y + 1, this.transform.position.z - (AddValue / 2 + 0.75f));
                break;
            case DirectionType.DownLeft:
                trapObje.transform.position = new Vector3(this.transform.position.x - (AddValue / 2 + 0.75f), this.transform.position.y + 1, this.transform.position.z - (AddValue / 2 + 0.75f));
                break;
        }

        if (trapDistanceType == TrapDistanceType.VeryClose || trapDistanceType == TrapDistanceType.Near)
        {
            barrierTransform.transform.localScale = new Vector3(barrierTransform.transform.localScale.x, scaleValue, barrierTransform.transform.localScale.z);
        }
        else if (trapDistanceType == TrapDistanceType.Far || trapDistanceType == TrapDistanceType.TooFar)
        {
            barrierTransform.transform.localScale = new Vector3(barrierTransform.transform.localScale.x, barrierTransform.transform.localScale.x - scaleValue, barrierTransform.transform.localScale.z);
        }
        else
        {
            barrierTransform.transform.localScale = new Vector3(barrierTransform.transform.localScale.x, scaleValue, barrierTransform.transform.localScale.z);
        }
    }

    #endregion

    #region Enumerator Methods

    private IEnumerator FrangibleObjesCreate()
    {
        GameManager.FrangibleObjeTransform.transform.position = this.transform.position;
        FrangibleObjeCountText.gameObject.SetActive(true);
        int standardColorId = 0;
        float onsetPositionY = this.transform.position.y;

        for (int i = 1; i <= AreaData.FrangibleObjeCount; i++)
        {
            yield return new WaitForSeconds((2f / AreaData.FrangibleObjeCount));
            standardColorId = i % 2 == 0 ? 0 : 1;
            onsetPositionY += 1f;
            FrangibleObjeCountText.text = i.ToString();
            GameObject frangibleObje = FrangibleObjePool.Instance.Get().gameObject;
            frangibleObje.GetComponent<FrangibleObje>().GameManager = GameManager;
            FrangibleObjes.Add(frangibleObje);
            frangibleObje.transform.position = new Vector3(this.transform.position.x, onsetPositionY, this.transform.position.z);
            frangibleObje.transform.rotation = this.transform.rotation;
            frangibleObje.GetComponent<MeshRenderer>().materials[0].color = AreaData.Materials[standardColorId].color;
            frangibleObje.SetActive(true);
        }

        TrapsCreate();
        IsFrangibleObjesCreatived = true;
        StopCoroutine(FrangibleObjesCreate());
    }

    private IEnumerator TrapsSlowlyDestroy()
    {
        IsFrangibleObjesCreatived = false;
        if (Traps[0].transform.position.y >= 0)
        {
            yield return new WaitForSeconds(0.05f);
            foreach (GameObject game in Traps)
            {
                game.transform.position = new Vector3(game.transform.position.x, game.transform.position.y - 0.08f, game.transform.position.z);
                game.GetComponent<Trap>().Speed = 0;
            }
        }
        else if (Traps[0].transform.position.y < 0)
        {
            for (int i = 0; i < Traps.Count; i++)
            {
                Traps[i].transform.position = new Vector3(0, 0, 0);
                Traps[i].GetComponent<Trap>().BodyTransform.transform.position = new Vector3(0, 0, 0);
                Traps[i].GetComponent<Trap>().BodyTransform.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                TrapPool.Instance.ReturnObject(Traps[i].GetComponent<Trap>());
                Traps.Remove(Traps[i]);
            }
            FrangibleObjeCountText.gameObject.SetActive(false);
            StopCoroutine(TrapsSlowlyDestroy());
        }
    }

    #endregion
}