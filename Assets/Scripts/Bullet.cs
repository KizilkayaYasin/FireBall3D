using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region Constants

    private const string mTRAP = "Trap";
    private const string mFRANGIBLEOBJE = "FrangibleObje";
    public const float mSPEED = 20.0f;

    #endregion

    #region Fields

    public GameManager GameManager;
    private Transform mFrangibleTransform;
    public Animator BulletAnimator;
    public float Speed;
    public bool IsBreak;

    #endregion

    //Test commit

    #region Unity Methods

    private void Start()
    {
        mFrangibleTransform = GameManager.FrangibleObjeTransform.transform;
    }

    private void Update()
    {
        BulletAnimator.SetBool("IsBreak", IsBreak);
        transform.Translate(Vector3.right * Speed * Time.deltaTime);

        if (GameManager.LocationArea.IsFrangibleObjesCreatived == false)
        {
            BulletPool.Instance.ReturnObject(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(mFRANGIBLEOBJE))
        {
            BulletPool.Instance.ReturnObject(this);

            if (GameManager.LocationArea.FrangibleObjeCountText.gameObject.activeInHierarchy && GameManager.LocationArea.FrangibleObjes.Count > 0)
            {
                int value = IsBreak ? 3 : 1;
                for (int i = 0; i < value; i++)
                {
                    if (GameManager.LocationArea.FrangibleObjes.Count > 0)
                    {
                        GameManager.ScoreText.text = (int.Parse(GameManager.ScoreText.text.ToString()) + 2).ToString();
                        FrangibleObjePool.Instance.ReturnObject(GameManager.LocationArea.FrangibleObjes[0].GetComponent<FrangibleObje>());
                        mFrangibleTransform.position = new Vector3(mFrangibleTransform.position.x, mFrangibleTransform.position.y - 1, mFrangibleTransform.position.z);
                        GameManager.LocationArea.FrangibleObjes.Remove(GameManager.LocationArea.FrangibleObjes[0]);
                        GameManager.LocationArea.FrangibleObjeCountText.text = GameManager.LocationArea.FrangibleObjes.Count.ToString();
                        GameManager.IsPause = GameManager.LocationArea.FrangibleObjes.Count == 0 ? true : false;
                    }
                }
            }
        }
        else if (other.CompareTag(mTRAP))
        {
            BulletPool.Instance.ReturnObject(this);

            if (IsBreak)
            {
                TrapPool.Instance.ReturnObject(other.GetComponentInParent<Trap>());
            }
            else
            {
                GameManager.LocationArea.IsFrangibleObjesCreatived = false;
                GameManager.IsPause = true;
                GameManager.Panels[1].SetActive(true);
                GameManager.Panels[2].SetActive(false);
                GameManager.GameOverMenuButton[0].gameObject.SetActive(true);
                GameManager.GameOverMenuButton[1].gameObject.SetActive(false);

                if (int.Parse(GameManager.ScoreText.text.ToString()) > PlayerPrefs.GetInt("HighScore"))
                {
                    PlayerPrefs.SetInt("HighScore", int.Parse(GameManager.ScoreText.text.ToString()));
                }

                GameManager.GameOverScoreText.text = "Score :" + GameManager.ScoreText.text;
                GameManager.GameOverHighScoreText.text = "Best : " + PlayerPrefs.GetInt("HighScore");
            }
        }
    }

    #endregion
}