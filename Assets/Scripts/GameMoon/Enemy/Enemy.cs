using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private Rigidbody2D _target;
    [SerializeField]
    private GameObject _waypoints;

    List<Transform> _waypointList = new List<Transform>();
    bool isLive;
    int waypointIndex = 0;

    [SerializeField]
    private Animator _animator;

    private Rigidbody2D _rigidbody;

    private void Awake() {
        isLive = true;
        _rigidbody = GetComponent<Rigidbody2D>();
        Transform parentTransform = _waypoints.transform;

        foreach (Transform childTransform in parentTransform)
        {
            _waypointList.Add(childTransform);
        }
    }

    private void FixedUpdate() {
        if(!isLive)
            return;

        _animator.SetFloat("move", 1f);
    //     //(방향을 알수 있음)위치 = 타켓 위치 - 나의위치
    //    Vector2 dirVec = _target.position - _rigidbody.position;
    //    //다음에 가야할 위치 = 방향벡터 * 속도 * 시간
    //    Vector2 nextVec = dirVec.normalized  * _speed * Time.fixedDeltaTime;
    //    _rigidbody.MovePosition(_rigidbody.position + nextVec);
    //    _rigidbody.velocity = Vector2.zero;

        if(waypointIndex < _waypointList.Count) {
            transform.position = Vector2.MoveTowards (transform.position, _waypointList[waypointIndex].transform.position, _speed * Time.fixedDeltaTime);
        
            if(transform.position == _waypointList[waypointIndex].transform.position) {
                waypointIndex += 1;
            }
        }
    }

    private void LateUpdate() {
        if(!isLive)
            return;

        if(_target.position.x < _rigidbody.position.x) {
            transform.rotation = Quaternion.Euler(0, 180f, 0);
        } else {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
     
    }
}
