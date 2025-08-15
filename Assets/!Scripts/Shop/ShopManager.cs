using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ShopArea { Shop = 0, CraftingStation = 1, Shelves = 2, Portal = 3 }
public class ShopManager : PersistentSingleton<ShopManager>
{
    private struct LoadedArea
    {
        public ShopArea Area;
        public Scene SceneReference;
    }

    public ShopArea CurrentArea = ShopArea.Shop;

    public readonly Dictionary<ShopArea, string> _sceneAreaLookup = new();

    protected override void Awake()
    {
        base.Awake();

        _sceneAreaLookup.Add(ShopArea.Shop, "Shop");
        _sceneAreaLookup.Add(ShopArea.CraftingStation, "Crafting");
        _sceneAreaLookup.Add(ShopArea.Shelves, "Infinite Shelves");
        _sceneAreaLookup.Add(ShopArea.Portal, "Witching Zone");
    }

    public bool LoadArea(ShopArea area)
    {
        if (_sceneAreaLookup.TryGetValue(area, out string sceneName))
        {
            Scene targetScene = SceneManager.GetSceneByName(sceneName);
            if (targetScene.isLoaded)
            {
                Debug.LogWarning("WARN: attempted to load " + sceneName + " while it was already loaded!");
                return false;
            }

            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            SceneManager.sceneLoaded += SetActiveScene;
            CurrentArea = area;
            ToggleShopArea(false);

            return true;
        }

        return false;
    }

    public async void UnloadArea()
    {
        if (_sceneAreaLookup.TryGetValue(CurrentArea, out string name))
        {
            Scene toRemove = SceneManager.GetSceneByName(name);
            if (toRemove.isLoaded)
            {
                await SceneManager.UnloadSceneAsync(toRemove.buildIndex);

                ToggleShopArea(true);
                CurrentArea = ShopArea.Shop;
                SceneManager.SetActiveScene(SceneManager.GetSceneByName("Shop"));
            }
        }
    }

    private void SetActiveScene(Scene loadedScene, LoadSceneMode loadMode)
    {
        SceneManager.SetActiveScene(loadedScene);
        SceneManager.sceneLoaded -= SetActiveScene;
    }

    public void ToggleShopArea(bool toggle)
    {
        Scene shopScene = SceneManager.GetSceneByName("Shop");
        if (!shopScene.IsValid())
        {
            throw new System.InvalidOperationException("Shop scene is not loaded!");
        }

        GameObject[] shopGOs = shopScene.GetRootGameObjects();
        foreach (GameObject shopGO in shopGOs)
        {
            if (shopGO == gameObject) continue;

            shopGO.SetActive(toggle);
        }
    }
}
