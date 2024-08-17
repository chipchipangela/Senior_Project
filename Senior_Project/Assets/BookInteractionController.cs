using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eWolf.BookEffectV2.Interfaces;

public class BookInteractionController : MonoBehaviour
{
    public GameObject bookObject; // �ѥ��C������
    private IBookControl _bookControl; // IBookControl���Ѧ�
    private int currentPage = 0; // ��e����
    private bool isReversing = false; // �P�_�O�_�b½�^�e�@��
    private bool isBookOpen = false;  // �ѥ��O�_�w�g���}
    private bool hasDroppedFirstBook = false; // �O�_�w�g�����F�Ĥ@����

    public Camera ovrCamera; // OVRCamera���Ѧ�
    public BookDrop bookDropScript; // �ѥ���������}�����Ѧ�

    private void Start()
    {
        _bookControl = bookObject.GetComponent<IBookControl>();
    }

    private void Update()
    {
        // �˴��ƹ������I���öi��g�u�˴�
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
        if (other.CompareTag("Hand")) // ���]�ⳡ����"Hand"��Tag
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
            _bookControl.OpenBookAtPage(currentPage); // ���}�ѥ����0��

            if (!hasDroppedFirstBook)
            {
                // �]�w����5��ᱼ���Ĥ@����
                Invoke("DropFirstBookAfterDelay", 5.0f);
                hasDroppedFirstBook = true; // �T�O�uĲ�o�@������
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
                    isReversing = false; // ����e½�A���s�}�l�`��
                }
            }
            else
            {
                _bookControl.TurnPage();
                currentPage += 2;
                if (currentPage >= 4)
                {
                    currentPage = 4;
                    isReversing = true; // �}�l½�^�e�@��
                }
            }
        }
    }

    private void DropFirstBookAfterDelay()
    {
        if (bookDropScript != null)
        {
            bookDropScript.DropFirstBook(); // �I�s�����Ĥ@���Ѫ���k
        }
    }
}