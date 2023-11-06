using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    bool toggle;
    public void setPopupDeactive()
    {
        Invoke("deactive", 4.0f);
    }

    private void deactive()
    {
        this.gameObject.SetActive(false);
    }
}
