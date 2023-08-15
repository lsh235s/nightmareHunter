using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonBullet : MonoBehaviour
{
    public float range; // 사거리
    public Vector3 initialPosition; // 초기 위치
    private float delayTime = 0.0f;
    public float _bulletSpeed; // 총알 속도

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
       
                // 일정한 속도로 위쪽으로 이동
                transform.Translate(Vector2.right * _bulletSpeed * Time.deltaTime);
           
    }
}
