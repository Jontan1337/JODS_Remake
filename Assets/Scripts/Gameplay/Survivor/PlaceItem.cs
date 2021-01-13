using UnityEngine;
using Mirror;

public class PlaceItem : NetworkBehaviour
{
    public GameObject item;
    public GameObject placeholderPrefab;
    private GameObject placeholder;
    private Shoot shoot;
    private Camera cam;

    private void Start()
    {
        shoot = GetComponent<Shoot>();
        cam = shoot.camera;
    }

    /// <summary>
    /// Show the placeholder?
    /// </summary>
    /// <param name="placehold"></param>
    /// <returns></returns>
    public bool Place(bool placehold)
    {
        RaycastHit hit;
        if (placehold)
        {
            if (!placeholder)
            {
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 3f, shoot.raycastLayerIgnore))
                {
                    placeholder = Instantiate(placeholderPrefab, hit.point, transform.rotation, transform);
                    if (item.GetComponent<ItemMesh>().mesh.Length != 0)
                    {
                        foreach (var mesh in item.GetComponent<ItemMesh>().mesh)
                        {
                            placeholder.GetComponent<MeshFilter>().mesh = mesh;
                        }
                    }
                    else if (item.GetComponent<ItemMesh>().model)
                    {
                        Instantiate(item.GetComponent<ItemMesh>().model, placeholder.transform.position, Quaternion.identity, placeholder.transform);
                    }
                    return false;
                }
                return false;
            }
            if (placeholder != null)
            {
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 3f, shoot.raycastLayerIgnore))
                {
                    placeholder.SetActive(true);
                    placeholder.transform.position = new Vector3(hit.point.x, hit.point.y + 0.1F, hit.point.z);
                }
                else
                {
                    //out of range
                    placeholder.SetActive(false);
                    placeholder.GetComponent<ItemPlaceholder>().ChangeMaterials(false);
                    placeholder.GetComponent<ItemPlaceholder>().obstructed = false;
                }
                return false;
            }
            return false;
        }
        else
        {
            if (placeholder && placeholder.activeSelf && !placeholder.GetComponent<ItemPlaceholder>().obstructed)
            {
                //Create the new item
                Debug.Log(item);
                CmdPlaceItem(item.name, placeholder.transform.position, placeholder.transform.rotation);
                Destroy(placeholder);
                return true;
            }
            else
            {
                GameObject infoMessages = GameObject.Find("GlobalMessages");
                if (infoMessages)
                {
                    infoMessages.GetComponent<InfoMessage>().NewMessage("Could not place item");
                }
                print("Could not place item");
                Destroy(placeholder);
                return false;
            }
        }
    }
    [Command]
    void CmdPlaceItem(string prefabName, Vector3 position, Quaternion rotation)
    {
        GameObject newItem = Instantiate(Resources.Load<GameObject>($"Spawnables/{prefabName}"), position, rotation);
        print($"Placing item {newItem}");
        NetworkServer.Spawn(newItem);
    }
}
