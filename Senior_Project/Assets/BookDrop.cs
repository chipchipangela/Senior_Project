using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookDrop : MonoBehaviour
{
    public GameObject bookPrefab; // �ѥ���Prefab
    public float firstDropDistance = 0.6f; // �Ĥ@���ѱ������Z���]60�����^
    public float secondDropDistance = 1.5f; // �ĤG���ѱ������Z���]150�����^
    public float dropHeight = 4.0f; // �ѥ�����l����
    public float groundHeight = 0.0f; // �a��������
    public AudioClip dropSound; // �ѥ��������n��

    private Transform headsetTransform; // �Y�㪺��m

    public float zStartPosition = 9.0f; // �������_�lZ�y��
    public float zEndPosition = -9.0f; // ���������IZ�y��
    public float xRangeMin = -12.0f; // �����d��X�̤p��
    public float xRangeMax = 12.0f; // �����d��X�̤j��
    public float yMaxHeight = 2.0f; // ���|�ѥ����̤j����
    public int numberOfBooks = 200; // �����ѥ����`�ƶq
    public float dropInterval = 0.05f; // �ѥ����������j�ɶ�

    void Start()
    {
        // ���OVRCameraRig�����Y��Transform
        headsetTransform = GameObject.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor").transform;
    }

    public void DropFirstBook()
    {
        // �b����x��X��x�T�{��k�O�_�QĲ�o
        Debug.Log("Dropping first book");

        // �p��Ĥ@���ѱ�������m
        Vector3 dropPosition = headsetTransform.position - headsetTransform.forward * firstDropDistance;
        dropPosition.y = dropHeight; // �ѥ��ͦ��b����4

        // �ͦ��Ĥ@����
        GameObject firstBook = Instantiate(bookPrefab, dropPosition, Quaternion.identity);

        // ���񱼸��n��
        AudioSource.PlayClipAtPoint(dropSound, dropPosition);

        // �A������5��A�M�ᱼ���ĤG����
        Invoke("DropSecondBook", 5.0f);
    }

    void DropSecondBook()
    {
        // ���s����Y�㪺��m
        headsetTransform = GameObject.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor").transform;

        // �p��ĤG���ѱ�������m
        Vector3 dropPosition = headsetTransform.position - headsetTransform.forward * secondDropDistance;
        dropPosition.y = dropHeight; // �ѥ��ͦ��b����4

        // �ͦ��ĤG����
        GameObject secondBook = Instantiate(bookPrefab, dropPosition, Quaternion.identity);

        // ���񱼸��n��
        AudioSource.PlayClipAtPoint(dropSound, dropPosition);

        // �A�L3���}�l�j�q�ѥ�����
        Invoke("StartSequentialBookDrop", 3.0f);
    }

    void StartSequentialBookDrop()
    {
        StartCoroutine(DropBooksSequentially());
    }

    IEnumerator DropBooksSequentially()
    {
        float zPosition = zStartPosition;
        float zStep = (zStartPosition - zEndPosition) / numberOfBooks;

        for (int i = 0; i < numberOfBooks; i++)
        {
            // �H���ͦ��ѥ���X�y��
            float randomX = Random.Range(xRangeMin, xRangeMax);

            // �]�w�ѥ���������m
            Vector3 dropPosition = new Vector3(randomX, dropHeight, zPosition);

            // �H���ͦ��ѥ������ਤ��
            Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

            // �H���ͦ��ѥ����j�p
            float randomScale = Random.Range(0.8f, 1.2f);

            // �ͦ��ѥ�
            GameObject book = Instantiate(bookPrefab, dropPosition, randomRotation);
            book.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

            // ���񱼸��n���]�i��^
            AudioSource.PlayClipAtPoint(dropSound, dropPosition);

            // ��sZ�y�СA�̦����t��V����
            zPosition -= zStep;

            // ���ݫ��w�����j�ɶ���A�����U�@����
            yield return new WaitForSeconds(dropInterval);
        }
    }
}
