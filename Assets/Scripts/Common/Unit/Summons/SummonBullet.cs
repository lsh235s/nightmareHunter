using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonBullet : MonoBehaviour
{
    public float range; // 사거리
    public Vector3 initialPosition; // 초기 위치
    private float delayTime = 0.0f;
    public float _bulletSpeed; // 총알 속도
    public float _bulletDamage; // 총알 데미지
    public string attackType; //발사체 유형
    public Transform targetMonster; // 타겟

    private bool activeflag = false;

    Rigidbody2D rigidbody2D;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if("FSP".Equals(attackType))
        {
              // 일정한 속도로 위쪽으로 이동
            transform.Translate(Vector3.forward  * _bulletSpeed * Time.deltaTime);
            Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        
            if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1)
            {
                Destroy(gameObject);
            }
            return;
        }
      
           
    }
}
