using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eWolf.BookEffectV2.Interfaces;

public class BookInteractionController : MonoBehaviour
{
    public GameObject bookObject; // 書本遊戲物件
    private IBookControl _bookControl; // IBookControl的參考
    private int currentPage = 0; // 當前頁面
    private bool isReversing = false; // 判斷是否在翻回前一頁
    private bool isBookOpen = false;  // 書本是否已經打開
    private bool hasDroppedFirstBook = false; // 是否已經掉落了第一本書

    public Camera ovrCamera; // OVRCamera的參考
    public BookDrop bookDropScript; // 書本掉落控制腳本的參考

    private void Start()
    {
        _bookControl = bookObject.GetComponent<IBookControl>();
    }

    private void Update()
    {
        // 檢測滑鼠左鍵點擊並進行射線檢測
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = ovrCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == bookObject)
                {
                    OnHandTouch();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hand")) // 假設手部物件有"Hand"的Tag
        {
            OnHandTouch();
        }
    }

    private void OnHandTouch()
    {
        if (!isBookOpen)
        {
            _bookControl.OpenBook();
            isBookOpen = true;
            currentPage = 0;
            _bookControl.OpenBookAtPage(currentPage); // 打開書本到第0頁

            if (!hasDroppedFirstBook)
            {
                // 設定延遲5秒後掉落第一本書
                Invoke("DropFirstBookAfterDelay", 5.0f);
                hasDroppedFirstBook = true; // 確保只觸發一次掉落
            }
        }
        else
        {
            if (isReversing)
            {
                _bookControl.TurnPageBack();
                currentPage -= 2;
                if (currentPage <= 0)
                {
                    currentPage = 0;
                    isReversing = false; // 停止往前翻，重新開始循環
                }
            }
            else
            {
                _bookControl.TurnPage();
                currentPage += 2;
                if (currentPage >= 4)
                {
                    currentPage = 4;
                    isReversing = true; // 開始翻回前一頁
                }
            }
        }
    }

    private void DropFirstBookAfterDelay()
    {
        if (bookDropScript != null)
        {
            bookDropScript.DropFirstBook(); // 呼叫掉落第一本書的方法
        }
    }
}