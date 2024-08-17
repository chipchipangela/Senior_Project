using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookDrop : MonoBehaviour
{
    public GameObject bookPrefab; // 書本的Prefab
    public float firstDropDistance = 0.6f; // 第一本書掉落的距離（60公分）
    public float secondDropDistance = 1.5f; // 第二本書掉落的距離（150公分）
    public float dropHeight = 4.0f; // 書本的初始高度
    public float groundHeight = 0.0f; // 地面的高度
    public AudioClip dropSound; // 書本掉落的聲音

    private Transform headsetTransform; // 頭顯的位置

    public float zStartPosition = 9.0f; // 掉落的起始Z座標
    public float zEndPosition = -9.0f; // 掉落的終點Z座標
    public float xRangeMin = -12.0f; // 掉落範圍的X最小值
    public float xRangeMax = 12.0f; // 掉落範圍的X最大值
    public float yMaxHeight = 2.0f; // 堆疊書本的最大高度
    public int numberOfBooks = 200; // 掉落書本的總數量
    public float dropInterval = 0.05f; // 書本掉落的間隔時間

    void Start()
    {
        // 獲取OVRCameraRig中的頭顯Transform
        headsetTransform = GameObject.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor").transform;
    }

    public void DropFirstBook()
    {
        // 在控制台輸出日誌確認方法是否被觸發
        Debug.Log("Dropping first book");

        // 計算第一本書掉落的位置
        Vector3 dropPosition = headsetTransform.position - headsetTransform.forward * firstDropDistance;
        dropPosition.y = dropHeight; // 書本生成在高度4

        // 生成第一本書
        GameObject firstBook = Instantiate(bookPrefab, dropPosition, Quaternion.identity);

        // 播放掉落聲音
        AudioSource.PlayClipAtPoint(dropSound, dropPosition);

        // 再次等待5秒，然後掉落第二本書
        Invoke("DropSecondBook", 5.0f);
    }

    void DropSecondBook()
    {
        // 重新抓取頭顯的位置
        headsetTransform = GameObject.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor").transform;

        // 計算第二本書掉落的位置
        Vector3 dropPosition = headsetTransform.position - headsetTransform.forward * secondDropDistance;
        dropPosition.y = dropHeight; // 書本生成在高度4

        // 生成第二本書
        GameObject secondBook = Instantiate(bookPrefab, dropPosition, Quaternion.identity);

        // 播放掉落聲音
        AudioSource.PlayClipAtPoint(dropSound, dropPosition);

        // 再過3秒後開始大量書本掉落
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
            // 隨機生成書本的X座標
            float randomX = Random.Range(xRangeMin, xRangeMax);

            // 設定書本掉落的位置
            Vector3 dropPosition = new Vector3(randomX, dropHeight, zPosition);

            // 隨機生成書本的旋轉角度
            Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

            // 隨機生成書本的大小
            float randomScale = Random.Range(0.8f, 1.2f);

            // 生成書本
            GameObject book = Instantiate(bookPrefab, dropPosition, randomRotation);
            book.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

            // 播放掉落聲音（可選）
            AudioSource.PlayClipAtPoint(dropSound, dropPosition);

            // 更新Z座標，依次往負方向移動
            zPosition -= zStep;

            // 等待指定的間隔時間後再掉落下一本書
            yield return new WaitForSeconds(dropInterval);
        }
    }
}
