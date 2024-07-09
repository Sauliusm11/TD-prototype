using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Tilemaps;
using System;

public class TileSelectionHandler : MonoBehaviour
{
    GameManager manager;
    [SerializeField]
    List<GameObject> SelectorButtons;
    [SerializeField]
    List<Tile> Tiles;
    TileContainer tileContainer;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void SelectSquare()
    {
        if (tileContainer == null)
        {
            tileContainer = TileContainer.getInstance();
        }
        string name = EventSystem.current.currentSelectedGameObject.name;
        name = name.Substring(6);
        foreach (TileContainer.Tile tile in tileContainer.tiles)
        {
            if (name.Equals(tile.name))
            {
                manager.SetSelectedTile(tile);
            }
        }
    }

    public Tile GetTileFromSelection(TileContainer.Tile selection)
    {
        foreach (Tile tile in Tiles)
        {
            if (tile.name.Equals(selection.name))
            {
                Debug.Log(tile.name);
                return tile;
            }
        }
        //Removal
        return null;
    }
    //public List<Tile> GetTileList()
    //{
    //    return Tiles;
    //}
}
