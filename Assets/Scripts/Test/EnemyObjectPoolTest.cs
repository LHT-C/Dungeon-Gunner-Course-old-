using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class EnemyObjectPoolTest : MonoBehaviour
{
    [SerializeField] private EnemyAnimationDetails[] enemyAnimationDetailsArray;
    [SerializeField] GameObject enemyExamplePrefab;
    private float timer = 1f;

    [System.Serializable]
    public struct EnemyAnimationDetails
    {
        public RuntimeAnimatorController animatorController;
        public Color spriteColor;
    }

    // Update is called once per frame

    void Update()
    {
        // Spawn random enemy sprite every second ÿ����������з���ͼ
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            GetEnemyExample();
            timer = 1f;
        }
    }

    private void GetEnemyExample()
    {
        // Current room
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        // random spawn position within room bounds �ڷ����ڿ�����ҵĵ��������λ��ˢ�������
        Vector3 spawnPositon = new Vector3(Random.Range(currentRoom.lowerBounds.x, currentRoom.upperBounds.x), Random.Range(currentRoom.lowerBounds.y, currentRoom.upperBounds.y), 0f);

        EnemyAnimation enemyAnimation = (EnemyAnimation)PoolManager.Instance.ReuseComponent(enemyExamplePrefab,
            HelperUtilities.GetSpawnPositionNearestToPlayer(spawnPositon), Quaternion.identity);

        int randomIndex = Random.Range(0, enemyAnimationDetailsArray.Length);

        enemyAnimation.gameObject.SetActive(true);

        enemyAnimation.SetAnimation(enemyAnimationDetailsArray[randomIndex].animatorController, enemyAnimationDetailsArray[randomIndex].spriteColor);
    }
}
