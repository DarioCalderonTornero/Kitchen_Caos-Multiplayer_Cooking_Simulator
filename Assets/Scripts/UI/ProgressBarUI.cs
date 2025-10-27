using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] Image progressBarImage;
    private IHasProgress hasProgress;
    [SerializeField] private GameObject hasProgressGameObject;

    // Start is called before the first frame update
    void Start()
    {
        if (hasProgress != null)
        {
            Debug.LogError("Error");
        }

        hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();
        hasProgress.OnProgressChanged += HasProgress_OnProgressChanged;

        progressBarImage.fillAmount = 0;

        Hide();
    }

   

    private void HasProgress_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventsArgs e)
    {
        progressBarImage.fillAmount = e.progressNormalized;

        if (e.progressNormalized == 0 || e.progressNormalized == 1)
        {
            Hide();
        }

        else
        {
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
