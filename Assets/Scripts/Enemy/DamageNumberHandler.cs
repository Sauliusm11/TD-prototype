using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public class DamageNumberHandler : MonoBehaviour
    {
        ObjectPooling damageNumberPooler;
        GameObject worldSpaceUICanvas;
        // Start is called before the first frame update
        void Start()
        {
            damageNumberPooler = GameObject.Find("DamageNumberPooler").GetComponent<ObjectPooling>();
            worldSpaceUICanvas = GameObject.Find("UIWorldSpaceCanvas");
        }

        public void ShowDamageNumber(int damage, float time, Transform transform)
        {
            StartCoroutine(UpdateNumber(damage, time, transform));
        }

        IEnumerator UpdateNumber(int damage, float time, Transform transform)
        {
            Vector3 offset = new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f));
            GameObject damageNumberObject = damageNumberPooler.ActivateObjectWithParent(worldSpaceUICanvas.transform);
            TMP_Text textObject = damageNumberObject.GetComponent<TMP_Text>();
            damageNumberObject.transform.position = transform.position + offset;
            textObject.text = damage.ToString();
            textObject.fontSize = 24 + Mathf.RoundToInt(damage / 50) * 4;

            float timeElapsed = Time.deltaTime;
            while (timeElapsed < time)
            {
                damageNumberObject.transform.position = transform.position + offset;
                yield return null;
                timeElapsed += Time.deltaTime;
            }
            damageNumberPooler.DeactivateObject(damageNumberObject);
        }
    }
}
