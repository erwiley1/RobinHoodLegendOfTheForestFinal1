using UnityEngine;

//CREDIT: https://www.youtube.com/watch?v=mKLp-2iseDc

public class rotateTowardsMouse : MonoBehaviour
{
    public float speed = 5f;

    // Update is called once per frame
    void Update()
    {
        // direction between position and target position, use camera to create screen to world point.
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, speed * Time.deltaTime);
    }
}
