using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("WallTiles").GetComponent<MapManager>();
                if (_instance == null)
                {
                    Debug.LogError("There are no tilemap");
                }
            }
            return _instance;
        }
    }

    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private LayerMask _whatIsObstacle;
    [SerializeField] private Tilemap _safezoneMap;

    public List<Vector3> GetAvailablePositionList(Vector3 center, float radius)
    {
        List<Vector3> pointList = new List<Vector3>();
        int radiusInt = Mathf.CeilToInt(radius);
        Vector3Int tileCenter = _tilemap.WorldToCell(center);

        for (int i = -radiusInt; i <= radiusInt; i++)
        {
            for (int j = -radiusInt; j <= radiusInt; j++)
            {
                if (Mathf.Abs(i) + Mathf.Abs(j) > radiusInt) continue;
                Vector3Int cellPoint = tileCenter + new Vector3Int(j, i);

                var tile = _tilemap.GetTile(cellPoint); // 만약 거기 타일이 있다면 타일베이스 나오고 아니면 null

                if (tile != null) continue;

                Vector3 worldPos = _tilemap.GetCellCenterWorld(cellPoint);
                var col = Physics2D.OverlapCircle(worldPos, 0.5f, _whatIsObstacle);

                if (col != null) continue;

                // 여기까지 도달했다면 아무것도 겹치는게 없으니 리스트에 넣는다.
                pointList.Add(worldPos);
            }
        }
        return pointList;
    }

    public bool InSafeZone(Vector3 worldPos)
    {
        Vector3Int pos = _safezoneMap.WorldToCell(worldPos);
        TileBase tile = _safezoneMap.GetTile(pos);
        return tile != null;
    }
}
