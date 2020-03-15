using UnityEngine;

public class Trap : MonoBehaviour
{
    #region Fields

    public GameManager GameManager;
    public Barrier BarrierData;
    public Transform Transform;
    public Transform BodyTransform;
    public float Speed;
    public Vector3 Vector3;
    public float HesitationTime;
    public float AdvenceTime;

    #endregion

    #region Property

    private bool Hesitation
    {
        get
        {
            return HesitationTime <= 0.01f;
        }
    }

    #endregion

    #region Unity Method

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (GameManager.IsRetry && this.gameObject.activeInHierarchy)
        {
            this.transform.position = new Vector3(0,0,0);
            BodyTransform.position = new Vector3(0, 0, 0);
            BodyTransform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
            TrapPool.Instance.ReturnObject(this);
        }

        if (HesitationTime >= 0 && !Hesitation)
        {
            HesitationTime -= Time.deltaTime;
        }

        LookMidPoint();
        TurnAround();
        HesitationAdvence();
    }

    #endregion

    #region Private Methods

    private void Initialize()
    {
        HesitationTime = BarrierData.HesitationTime;
        AdvenceTime = BarrierData.AdvenceTime == 0 ? 1 : BarrierData.AdvenceTime;
        Speed = BarrierData.Speed;
    }

    private void LookMidPoint()
    {
        BodyTransform.LookAt(Transform, Vector3.down);
    }

    private void TurnAround()
    {
        BodyTransform.transform.RotateAround(Transform.position, Vector3, Speed * Time.deltaTime);
    }

    private void HesitationAdvence()
    {
        if (HesitationTime <= BarrierData.HesitationTime / 2 && !Hesitation)
        {
            Speed = 0;
            HesitationTime = HesitationTime / AdvenceTime;
        }
        else if (Hesitation)
        {
            Speed = BarrierData.Speed;
            HesitationTime = BarrierData.HesitationTime;
        }
    }

    #endregion
}