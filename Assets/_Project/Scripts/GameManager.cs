using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts
{
    public class GameManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void RestartScene()
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().path);
        }
    }
}
