using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Store : MonoBehaviour
{
    public float interactionRange = 2f;
    public GameObject StoreWindow;

    private void Update()
    {
        // 플레이어가 상호작용 범위에 있을 때 Space 바를 눌렀는지 확인
        if (GameManager.Instance.IsPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            OpenStore();
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("포탈들어옴");
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.IsPlayerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        // 플레이어가 포탈 범위에서 나갔을 때
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.IsPlayerInRange = false;
        }
    }
    private void OpenStore()
    {
        Debug.Log("상점 열림");
        Time.timeScale = 0;
        StoreWindow.SetActive(true);
    }
    public void CloseStore(){
        Time.timeScale=1;
        StoreWindow.SetActive(false);
    }

}
