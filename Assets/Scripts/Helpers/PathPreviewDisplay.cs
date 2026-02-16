using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Helpers
{
    public class PathPreviewDisplay : MonoBehaviour
    {
        [SerializeField]
        List<EnemyPathFinding> enemyPathFindings;
        [SerializeField]
        List<Sprite> sprites;
        [SerializeField]
        ObjectPooling previewImagePooler;
        public void ShowPath()
        {
            foreach (EnemyPathFinding pathFinding in enemyPathFindings)
            {
                Sprite sprite = SpriteFromPathfinder(pathFinding);
                Stack<WorldNode> path = pathFinding.GetPath();
                WorldNode[] pathArr = path.ToArray();
                WorldNode node = pathArr[0];
                Vector3 position = node.GetVector3();
                for (int i = 1; i < pathArr.Length; i++)
                {
                    WorldNode newNode = pathArr[i];
                    Vector3 newPosition = newNode.GetVector3();
                    DetermineDiretion(position, newPosition, sprite);
                    node = newNode;
                    position = newPosition;
                }
            }
        }
        public void ClearOutPreview()
        {
            previewImagePooler.DeactivateAll();
        }
        private void DetermineDiretion(Vector3 start, Vector3 end, Sprite sprite)
        {
            GameObject startObject = previewImagePooler.ActivateObjectWithParent(transform);
            GameObject endObject = previewImagePooler.ActivateObjectWithParent(transform);
            startObject.GetComponent<Image>().sprite = sprite;
            endObject.GetComponent<Image>().sprite = sprite;
            if (start.y < end.y)
            {
                //Up
                startObject.transform.position = start + new Vector3(0f, 0.25f, 0f);
                endObject.transform.position = end + new Vector3(0f, -0.25f, 0f);
            }
            if (start.y > end.y)
            {
                //down
                startObject.transform.position = start + new Vector3(0f, -0.25f, 0f);
                endObject.transform.position = end + new Vector3(0f, 0.25f, 0f);
            }
            if (start.x < end.x)
            {
                //right
                startObject.transform.position = start + new Vector3(0.25f, 0f, 0f);
                endObject.transform.position = end + new Vector3(-0.25f, 0f, 0f);
            }
            if (start.x > end.x)
            {
                //left
                startObject.transform.position = start + new Vector3(-0.25f, 0f, 0f);
                endObject.transform.position = end + new Vector3(0.25f, 0f, 0f);
            }

        }
        private Sprite SpriteFromPathfinder(EnemyPathFinding pathFinding)
        {
            if (pathFinding is BasicPathfinding)
            {
                return sprites[0];
            }
            if (pathFinding is NoTerrainPathfinding)
            {
                return sprites[1];
            }
            return sprites[0];
        }
    }
}
