using UnityEngine;

namespace UnityUITest.Example
{
    public class SecondScreen : MonoBehaviour
    {
        public void Close()
        {
            Destroy(gameObject);
        }
    }
}