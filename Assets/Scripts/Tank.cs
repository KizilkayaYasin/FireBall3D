using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tank : MonoBehaviour
{
    #region Constants

    private const float mATTACK_TIME = 0.2f;

    #endregion

    #region Fields

    public GameManager GameManager;
    public GameObject Barrel;

    private float mAttackTime;

    #endregion

    #region Property

    private bool mAttack
    {
        get { return mAttackTime <= 0.0f; }
    }

    #endregion

    #region Unity Methods

    private void Start()
    {
        Initialized();
    }

    private void Update()
    {
        if (mAttackTime > 0.0f)
        {
            mAttackTime -= Time.deltaTime;
        }

        if (mAttack && Input.GetMouseButton(0) && !GameManager.IsPause && GameManager.LocationArea.IsFrangibleObjesCreatived && !EventSystem.current.IsPointerOverGameObject())
        {
            GameManager.Value += GameManager.Value <= 10 ? 0.75f : 0;
            Shoot(GameManager.Value);
        }
    }

    #endregion

    #region Private Methods

    private void Initialized()
    {
        mAttackTime = mATTACK_TIME;
    }

    #endregion

    #region Public Method

    public void Shoot(float value)
    {
        GameObject bullet = BulletPool.Instance.Get().gameObject;
        bullet.GetComponent<Bullet>().GameManager = GameManager;
        bullet.GetComponent<Bullet>().Speed = value >= 9.75f && EventSystem.current.IsPointerOverGameObject() ? Bullet.mSPEED / 2.5f : Bullet.mSPEED;
        bullet.GetComponent<Bullet>().IsBreak = value >= 9.75f && EventSystem.current.IsPointerOverGameObject() ? true : false;
        bullet.transform.position = Barrel.transform.position;
        bullet.transform.rotation = Barrel.transform.rotation;
        mAttackTime = mATTACK_TIME;
        bullet.SetActive(true);
    }

    #endregion
}