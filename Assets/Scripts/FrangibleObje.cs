using UnityEngine;

public class FrangibleObje : MonoBehaviour
{
    public GameManager GameManager;

    private void Update()
    {
        if (GameManager.IsRetry && this.gameObject.activeInHierarchy)
        {
            FrangibleObjePool.Instance.ReturnObject(this);
        }
    }
}