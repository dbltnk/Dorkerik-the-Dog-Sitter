using UnityEngine;

namespace YuetilitySoftbody
{
    public class MoveAwayController : MonoBehaviour
    {
        public float MoveSpeed = 5f;

        private GameObject dorkerik;

        void Start()
        {
            dorkerik = GameObject.Find("Dorkerik");
            if (dorkerik == null)
            {
                Debug.LogError("No object named 'Dorkerik' found in the scene.");
            }
        }

        void Update()
        {
            if (dorkerik != null)
            {
                Vector3 directionToMove = transform.position - dorkerik.transform.position;
                float angle = Mathf.Atan2(directionToMove.y, directionToMove.x) * Mathf.Rad2Deg;
                Rigidbody rigidbody = GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    rigidbody.AddTorque(Vector3.forward * MoveSpeed * -angle);
                }
            }
        }
    }
}