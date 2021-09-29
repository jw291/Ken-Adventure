using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Define.CameraMode _mode = Define.CameraMode.ShoulderView;

    [SerializeField]
    Vector3 _delta = new Vector3(0.0f, 2.46f, -4.43f);

    [SerializeField]
    GameObject _player = null;

    private int rightFingerId;
    private float halfScreenWidth;  //화면 절반만 터치하면 카메라 회전
    private Vector2 prevPoint;

    private Vector3 originalPos;
    private Vector2 lookInput;
    private float cameraPitch; //pitch 시점
    private float cameraSpeed = 10.0f;


    public void SetPlayer(GameObject player) { _player = player; }

    void Start()
    {
        this.rightFingerId = -1;    //-1은 추적중이 아닌 손가락
        this.halfScreenWidth = Screen.width / 2;
    }
    void Update()
    {
        GetTouchInput();
    }
 
    void LateUpdate()
    {
        if (_mode == Define.CameraMode.ShoulderView)
        {
            RaycastHit hit;
            if (Physics.Raycast(_player.transform.position, _delta, out hit, _delta.magnitude))
            {
                if (!hit.collider.gameObject.CompareTag("Monster")) //몬스터를 제외한 충돌시 카메라 위치 조정
                {
                    float dist = (hit.point - _player.transform.position).magnitude * 0.8f;
                    transform.position = _player.transform.position + _delta.normalized * dist;
                }
                else
                {
                    transform.position = _player.transform.position + _delta;
                }
            }
            else
            {
                transform.position = _player.transform.position + _delta;
            }
        }
    }

    public void SetQuarterView(Vector3 delta)
    {
        _mode = Define.CameraMode.QuarterView;
        _delta = delta;
    }

    private void GetTouchInput()
    {
        //몇개의 터치가 입력되는가
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);

            switch (t.phase)
            {
                case TouchPhase.Began:

                    if (t.position.x > this.halfScreenWidth && this.rightFingerId == -1)
                    {
                        this.rightFingerId = t.fingerId;
                        Debug.Log("오른쪽 손가락 입력");
                    }
                    break;

                case TouchPhase.Moved:

                    //이것을 추가하면 시점 원상태 버튼을 누를 때 화면이 돌아가지 않는다
                    if (!EventSystem.current.IsPointerOverGameObject(i))
                    {
                        if (t.fingerId == this.rightFingerId)
                        {
                            if (EventSystem.current.IsPointerOverGameObject() == false)
                            {//수평
                                this.prevPoint = t.position - t.deltaPosition;//현재터치 위치 - 터치의 이동(터치가 빠르면 값이 높아짐)
                                //this.transform.RotateAround(this._player.transform.position, Vector3.up, -(t.position.x - this.prevPoint.x) * 0.2f);

                                _delta = Quaternion.AngleAxis((t.position.x - this.prevPoint.x) * 0.2f, Vector3.up) * _delta;
                                transform.position = _player.transform.position + _delta;
                                transform.LookAt(new Vector3(_player.transform.position.x, _player.transform.position.y + _delta.y, _player.transform.position.z));

                                this.prevPoint = t.position;
                            }
                        }
                    }
                    break;
                    
                case TouchPhase.Stationary:

                    if (t.fingerId == this.rightFingerId)
                    {
                        this.lookInput = Vector2.zero;

                    }
                    break;

                case TouchPhase.Ended:

                    if (t.fingerId == this.rightFingerId)
                    {
                        this.rightFingerId = -1;
                        Debug.Log("오른쪽 손가락 끝");

                    }
                    break;

                case TouchPhase.Canceled:

                    if (t.fingerId == this.rightFingerId)
                    {
                        this.rightFingerId = -1;
                        Debug.Log("오른쪽 손가락 끝");

                    }
                    break;
            }
        }
    }
}