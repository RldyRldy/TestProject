using System.Collections.Generic;
using UnityEngine;
using System.IO;
using JGame.Pool;
using JGame.Data;

namespace JGame
{
    public class Map : MonoBehaviour
    {
        protected Vector3 topPos = new Vector3();

        // current hero position arlam
        GameObject heroArlramObj = null;

        // x * y
        public IntRect tileNum;
                
        // x spacing
        public Vector3 tileSpacingX = new Vector3(0, 0, 0);
        // y spacing
        public Vector3 tileSpacingY = new Vector3(0, 0, 0);

        // tile prefab
        public List<GameObject> tilePrefabs = new List<GameObject>();
        public GameObject spawnPointPrefab = null;

        [HideInInspector]
        // created tiles
        public List<Tile> tiles = new List<Tile>();
        public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

        public GameObject arlramObj;
        public List<GameObject> arlramlist = new List<GameObject>();

        public GameObject dirObj;
        public List<GameObject> dirList = new List<GameObject>();

        // instance
        private static Map _instance = null;
        public static Map instance
        {
            get
            {
                return _instance;
            }
        }

        // Awake
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        // Use this for initialization
        void Start()
        {
            if(arlramObj == null)
            {
                arlramObj = Instantiate(Resources.Load<GameObject>("Prefab/Map/Arlram"));
                if( arlramObj != null )
                {
                    arlramObj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 0, 1);
                    arlramObj.GetComponent<SpriteRenderer>().sortingOrder -= 1;
                    arlramObj.SetActive(false);

                    arlramObj.transform.SetParent(transform);
                }
            }

            if( dirObj == null )
            {
                dirObj = Instantiate(Resources.Load<GameObject>("Prefab/Map/Dir"));

                if( dirObj != null )
                {
                    dirObj.SetActive(false);
                    dirObj.transform.SetParent(transform);
                }
            }

            // Create hero position arlam
            if (heroArlramObj == null)
            {
                heroArlramObj = ObjectPoolManager.instance.Pop(Resources.Load<GameObject>("Prefab/Map/Arlram"));

                if (heroArlramObj != null)
                {
                    var sr = heroArlramObj.GetComponent<SpriteRenderer>();

                    if (sr != null)
                    {
                        sr.color = new Color(0, 1, 0);
                    }
                    heroArlramObj.SetActive(false);
                    heroArlramObj.transform.SetParent(transform);
                }
            }
        }

        public void SetCurrentHeroGuide(bool bShow, Vector3 pos)
        {
            if (heroArlramObj != null)
            {
                heroArlramObj.SetActive(bShow);
                heroArlramObj.transform.position = pos;
            }
        }

        void CreateDirGuide(int x, int y, float angle)
        {
            var newObj = ObjectPoolManager.instance.Pop(dirObj);
            newObj.transform.SetPositionAndRotation(GetTilePosition(x,y), Quaternion.identity);
            newObj.transform.Rotate(0, 0, angle);
            newObj.transform.SetParent(transform);
            newObj.GetComponent<Tile>().position.x = x;
            newObj.GetComponent<Tile>().position.y = y;
            dirList.Add(newObj);
        }

        void CreateArlram( Vector3 position )
        {
            var newObj = ObjectPoolManager.instance.Pop(arlramObj);
            newObj.transform.position = position;
            newObj.transform.SetParent(transform);
            arlramlist.Add(newObj);
        }

        void ClearArlramList()
        {
            foreach( GameObject obj in arlramlist )
            {
                ObjectPoolManager.instance.Push(obj);
            }
            arlramlist.Clear();
        }

        // initialize
        public void Initialize(string dataPath)
        {
            MapData data = DataController.LoadJson<MapData>(dataPath); 

            Initialize(data);
        }

        public void Initialize(MapData newData)
        {
            tiles.Clear();

            tileNum.x = newData.tileX;
            tileNum.y = newData.tileY;

            topPos = Vector3.zero;
            topPos += tileSpacingX * tileNum.x * -0.5f;
            topPos += tileSpacingY * tileNum.y * -0.5f;

            for (int y = 0; y < tileNum.y; ++y)
            {
                for (int x = 0; x < tileNum.x; ++x)
                {
                    var i = newData.datas[tileNum.x * y + x];

#if UNITY_EDITOR
                    GameObject obj = null;
                    if ( Application.isEditor )
                    {
                        obj = Instantiate(tilePrefabs[i]);
                    }
                    else
                    {
                        obj = ObjectPoolManager.instance.Pop(tilePrefabs[i]);
                    }
#else
                    GameObject obj = ObjectPoolManager.instance.Pop(tilePrefabs[i]);
#endif

                    if (obj == null)
                    {
                        Debug.LogError("obj == null.");
                    }

                    // set game object properties
                    obj.name = "tile_" + (tileNum.x * y + x ) + "_" + y + "," + x; // name
                    obj.transform.SetParent(transform); // parent
                    obj.transform.position = topPos + (tileSpacingX * x) + (tileSpacingY * y); // position

                    var tile = obj.GetComponent<Tile>();
                    tile.position.x = x;
                    tile.position.y = y;

                    var sr = obj.GetComponent<SpriteRenderer>();
                    sr.sortingOrder = tileNum.x * y + x;

                    tiles.Add(tile);
                }
            }

            // add spawn point
            for(int i=0; i< newData.SpawnPoints.Length; ++i)
            {
#if UNITY_EDITOR
                GameObject obj = null;
                if (Application.isEditor)
                {
                    obj = Instantiate(spawnPointPrefab);
                }
                else
                {
                    obj = ObjectPoolManager.instance.Pop(spawnPointPrefab);
                }
#else
                    GameObject obj = ObjectPoolManager.instance.Pop(spawnPointPrefab);
#endif
                if (obj == null)
                {
                    Debug.LogError("obj == null.");
                }

                // set game object properties
                obj.name = "SpawnPoint_" + i + "_team"+ newData.SpawnPoints[i].teamIndex; // name
                obj.transform.SetParent(transform); // parent
                obj.transform.position = topPos + (tileSpacingX * newData.SpawnPoints[i].tileX) + (tileSpacingY * newData.SpawnPoints[i].tileY); // position

                var spawnPoint = obj.GetComponent<SpawnPoint>();
                spawnPoint.position.x = newData.SpawnPoints[i].tileX;
                spawnPoint.position.y = newData.SpawnPoints[i].tileY;
                spawnPoint.SetTeamIndex(newData.SpawnPoints[i].teamIndex);

                spawnPoints.Add(spawnPoint);
            }
        }

        // get tile
        public Tile GetTile(IntRect pos)
        {
            return GetTile(pos.x, pos.y);
        }

        public Tile GetTile(int x, int y)
        {
            if( x < 0 || y < 0 || x >= tileNum.x || y >= tileNum.y )
            {
                return null;
            }

            return tiles[y * tileNum.x + x];
        }

        public Vector3 GetTilePosition(IntRect pos)
        {
            return GetTilePosition(pos.x, pos.y);
        }

        public Vector3 GetTilePosition(int x, int y)
        {
            return (topPos + (tileSpacingX * x) + (tileSpacingY * y));
        }

        public void LeaveFrom( IntRect pos, GameObject obj )
        {
            LeaveFrom(pos.x, pos.y, obj);
        }

        public void LeaveFrom( int x, int y, GameObject obj )
        {
            Tile t = GetTile(x,y);

            if (t == null) return;

            if (t.actor == obj)
            {
                t.actor = null;
            }
        }

        public void MoveTo( IntRect pos, GameObject obj )
        {
            MoveTo(pos.x, pos.y, obj);
        }

        public void MoveTo( int x, int y, GameObject obj )
        {
            Tile t = GetTile(x, y);

            if (t == null) return;

            if (t.actor == null)
            {
                t.actor = obj;
            }
        }

        public void DisableAllTiles()
        {
            ClearArlramList();
            for ( int i = 0; i < tiles.Count; ++i )
            {
                if( tiles[i] != null )
                {
                    tiles[i].SetSelectable(false);
                }
            }
        }

        public void EnableAllTiles()
        {
            ClearArlramList();
            for (int i = 0; i < tiles.Count; ++i)
            {
                if (tiles[i] != null)
                {
                    tiles[i].SetSelectable(true);
                }
            }
        }

        public void EnableRangeTiles( IntRect position, int range )
        {
            for (int y = (position.y - range); y <= (position.y + range); ++y)
            {
                var gab = range - Mathf.Abs(y - position.y);
                for ( int x = position.x - gab; x <= position.x + gab;++x )
                {
                    var tile = GetTile(x, y);

                    if( tile != null )
                    {
                        CreateArlram(tile.transform.position);
                        tile.SetSelectable(true);
                    }
                }
            }
        }

        public void EnableCanMoveRangeTiles(IntRect position, int range, Hero hero)
        {
            var tile = GetTile(position.x, position.y);

            if (tile != null)
            {
                tile.SetSelectable(true);
            }

            EnableCanMoveRangeTiles(position.x - 1, position.y, position.x, position.y, range, hero);
            EnableCanMoveRangeTiles(position.x + 1, position.y, position.x, position.y, range, hero);
            EnableCanMoveRangeTiles(position.x, position.y - 1, position.x, position.y, range, hero);
            EnableCanMoveRangeTiles(position.x, position.y + 1, position.x, position.y, range, hero);
        }

        protected void EnableCanMoveRangeTiles(int x, int y, int destX, int destY, int range, Hero hero)
        {
            if (range <= 0) return;

            var tile = GetTile(x, y);

            if (tile == null) return;

            if ( false == hero.CanMoveToTile(tile) )
            {
                return;
            }

            CreateArlram(tile.transform.position);
            tile.SetSelectable(true);            

            if ( false == (x - 1 == destX && y == destY) )
            {
                EnableCanMoveRangeTiles(x - 1, y, x, y, range - 1, hero);
            }

            if (false == (x + 1 == destX && y == destY))
            {
                EnableCanMoveRangeTiles(x + 1, y, x, y, range - 1, hero);
            }

            if (false == (x == destX && y - 1 == destY))
            {
                EnableCanMoveRangeTiles(x, y - 1, x, y, range - 1, hero);
            }

            if (false == (x == destX && y + 1 == destY))
            {
                EnableCanMoveRangeTiles(x, y + 1, x, y, range - 1, hero);
            }
        }

        public void ShowDirGuide(IntRect pivot)
        {
            CreateDirGuide(pivot.x - 1, pivot.y, 45.0f);
            CreateDirGuide(pivot.x + 1, pivot.y, -135.0f);
            CreateDirGuide(pivot.x , pivot.y + 1, 135.0f);
            CreateDirGuide(pivot.x , pivot.y - 1, -45.0f);
        }

        public void HideDirGuide()
        {
            foreach (GameObject obj in dirList)
            {
                ObjectPoolManager.instance.Push(obj);
            }
            dirList.Clear();
        }
    }
}
