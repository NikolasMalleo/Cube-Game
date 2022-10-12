using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    private CubePos nowCube = new CubePos(0, 1, 0);
    public float cubeChangePlaceSpeed = 0.5f, camSpeed = 1.5f;
    public Transform cubeToPlace;
    public GameObject cubeToCreate, allCubes;
    private Rigidbody AllCubesRb;
    private bool IsLose = false, firstCube = false;
    private Coroutine showCubePlace;
    public GameObject[] canvasStartPage;
    private Transform mainCam;

    private List<Vector3> allCubesPositions = new List<Vector3>
    {
        new Vector3(0, 1, 0),
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0, -1),
        new Vector3(1, 0, 1),
        new Vector3(-1, 0, 1),
        new Vector3(1, 0, -1),
        new Vector3(-1, 0, -1)
    };

    private void Start()
    {
        mainCam = Camera.main.transform;
        AllCubesRb = allCubes.GetComponent<Rigidbody>(); 
        showCubePlace = StartCoroutine(ShowCubePlace());
    }

    private void Update()
    {
        if(cubeToPlace != null && allCubes != null && (Input.GetMouseButtonDown(0) || Input.touchCount > 0) && !EventSystem.current.IsPointerOverGameObject())
        {
#if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began)
                return;
#endif

            if (!firstCube)
            {
                firstCube = true;
                foreach (GameObject obj in canvasStartPage)
                    Destroy(obj);
            }

            GameObject newCube = Instantiate(cubeToCreate,
                cubeToPlace.position,
                Quaternion.identity) as GameObject;
            newCube.transform.SetParent(allCubes.transform);
            nowCube.setVector(cubeToPlace.position);
            allCubesPositions.Add(nowCube.GetVector());

            AllCubesRb.isKinematic = true;
            AllCubesRb.isKinematic = false;

            SpawnPositions();
            MoveCameraChangeBg();
        }

        if(!IsLose && AllCubesRb.velocity.magnitude >= 0.1f)
        {
            Destroy(cubeToPlace.gameObject);
            IsLose = true;
            StopCoroutine(showCubePlace);
        }

        mainCam.localPosition = Vector3.MoveTowards(mainCam.localPosition, new Vector3(mainCam.localPosition.x, nowCube.y + 3f, mainCam.localPosition.z), camSpeed * Time.deltaTime);

    }

    IEnumerator ShowCubePlace()
    {
        while (true)
        {
            SpawnPositions();

            yield return new WaitForSeconds(cubeChangePlaceSpeed);
        }
    }

    private void SpawnPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        if (isPositionEmpty(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z)) && nowCube.x + 1 != cubeToPlace.position.x)
        {
            positions.Add(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z));
        }
        if (isPositionEmpty(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z)) && nowCube.x - 1 != cubeToPlace.position.x)
        {
            positions.Add(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z));
        }
        if (isPositionEmpty(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z)) && nowCube.y + 1 != cubeToPlace.position.y)
        {
            positions.Add(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z));
        }
        if (isPositionEmpty(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z)) && nowCube.y - 1 != cubeToPlace.position.y)
        {
            positions.Add(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z));
        }
        if (isPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1)) && nowCube.z + 1 != cubeToPlace.position.z)
        {
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1));
        }
        if (isPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1)) && nowCube.z - 1 != cubeToPlace.position.z)
        {
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1));
        }

        cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)];
    }


    private bool isPositionEmpty(Vector3 targetPos)
    {
        if(targetPos.y == 0)
        {
            return false;
        }

        foreach(Vector3 pos in allCubesPositions)
        {
            if (pos.x == targetPos.x && pos.y == targetPos.y && pos.z == targetPos.z)
            {
                return false;
            }
        }
        return true;
    }

    private void MoveCameraChangeBg()
    {
        int maxX = 0, maxY = 0, maxZ = 0;

        foreach (Vector3 pos in allCubesPositions)
        {
            if (Mathf.Abs(Convert.ToInt32(pos.x)) > maxX)
                maxX = Convert.ToInt32(pos.x);
            if (Convert.ToInt32(pos.y) > maxY)
                maxY = Convert.ToInt32(pos.y);
            if (Mathf.Abs(Convert.ToInt32(pos.z)) > maxZ)
                maxZ = Convert.ToInt32(pos.z);
        }
    }

}

struct CubePos
{
    public int x, y, z;

    public CubePos(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 GetVector()
    {
        return new Vector3(x, y, z);
    }

    public void setVector(Vector3 pos)
    {
        x = Convert.ToInt32(pos.x);
        y = Convert.ToInt32(pos.y);
        z = Convert.ToInt32(pos.z);
    }
}