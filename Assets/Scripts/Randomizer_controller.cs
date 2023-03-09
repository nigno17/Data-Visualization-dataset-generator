using System.IO;
using System;
using System.Collections.Generic;
//using System.Web.Script.Serialization;

namespace UnityEngine
{
    public class Randomizer_controller : MonoBehaviour
    {
        private GameObject loadedModel;
        private GameObject dirLight;
        private Object[] modelList;
        private Object[] skyMaterialList;
        // private Object[] floorMaterialList;
        private Object[] modelMaterialList;
        private int modelCounter;
        private int skyMaterialCounter;
        // private int floorMaterialCounter;
        private int modelMaterialCounter;
        private RandCameraController mainCameraController;
        // private MeshRenderer floorMeshRenderer;
        private string tempFolderName;
        private bool isRecording;
        
        // [SerializeField] GameObject mainMenu;
        // [SerializeField] GameObject ingameMenu;
        // [SerializeField] GameObject settingsMenu;
        // [SerializeField] GameObject animLoaderMenu;
        // [Space(10)]

        [Header("Folder where the dataset will be saved")]
        [Space(5)]
        [SerializeField] string screensFolderName = "";
        [Space(10)]

        // [Header("Add mirrored animations (flag)")]
        // [Space(5)]
        // [SerializeField] bool addMirrors = false;
        // [Space(10)]

        [Header("Directional light")]
        [Space(5)]
        [Tooltip("x: min, y: max")]
        [SerializeField] Vector2 intensityLev = new Vector2 (0.5f, 1.5f);
        
        [Space(5)]
        [Tooltip("flag to decide if the directional light color is going to be randomized")]
        [SerializeField] bool isColorRandomized = true;
        [Space(5)]
        [Tooltip("flag to decide if the skybox is going to be randomized")]
        [SerializeField] bool useSkyBoxRandomization = false;
        
        

        // Start is called before the first frame updateprivate static Random rng = new Random();  
        void Start()
        {
            Time.fixedDeltaTime = 0.01f;
            Time.maximumDeltaTime = 0.01f;

            isRecording = true;

            if (screensFolderName == "")
                screensFolderName = Application.dataPath + "/Screens/";

            modelCounter = -1;
            skyMaterialCounter = -1;
            modelMaterialCounter = -1;
            // floorMaterialCounter = -1;
            modelMaterialCounter = -1;
            tempFolderName = "";

            modelList = Resources.LoadAll("Models", typeof(GameObject));
            skyMaterialList = Resources.LoadAll("HDRIHaven/Materials", typeof(Material));
            // floorMaterialList = Resources.LoadAll("floor/Materials", typeof(Material));
            modelMaterialList = Resources.LoadAll("Materials", typeof(Material));

            dirLight = GameObject.Find("Directional Light");

            GameObject temp_randomizer = GameObject.Find("Main Camera");
            mainCameraController = temp_randomizer.GetComponent<RandCameraController>();
            
            // GameObject temp_floor = GameObject.Find("Floor");
            // floorMeshRenderer = temp_floor.GetComponent<MeshRenderer>();

            Debug.Log("Randomizer started...");
        }

        void Update()
        {
            if (isRecording)
                randomizerUpdate();
        }
        void LateUpdate()
        {
            if (isRecording)
                saveOnDisk();
        }

        void randomizerUpdate()
        {
            Destroy(loadedModel);
            loadedModel = spawnNextModel();
            MeshRenderer modelMeshRenderer = loadedModel.GetComponent<MeshRenderer>();
            modelMeshRenderer.material = spawnNextModelMaterial();

            if (useSkyBoxRandomization)
            {
                RenderSettings.skybox = spawnNextSkyMaterial();
                DynamicGI.UpdateEnvironment();
            }

            dirLight.transform.rotation = Random.rotation;
            Light tempLight = dirLight.GetComponent<Light>();
            tempLight.intensity = Random.Range(intensityLev[0], intensityLev[1]);
            if (isColorRandomized)
                tempLight.color = Random.ColorHSV();

            // floorMeshRenderer.material = spawnNextFloorMaterial();

            mainCameraController.setRandFlag(true);
        }

        // public void startRecording()
        // {
        //     isRecording = true;
        //     mainMenu.SetActive(false);
        //     ingameMenu.SetActive(true);
        //     settingsMenu.SetActive(false);
        //     animLoaderMenu.SetActive(false);
        // }

        // public void goToMain()
        // {
        //     isRecording = false;
        //     mainMenu.SetActive(true);
        //     ingameMenu.SetActive(false);
        //     settingsMenu.SetActive(false);
        //     animLoaderMenu.SetActive(false);
        // }

        // public void goToSettings()
        // {
        //     isRecording = false;
        //     mainMenu.SetActive(false);
        //     ingameMenu.SetActive(false);
        //     settingsMenu.SetActive(true);
        //     animLoaderMenu.SetActive(false);
        // }

        // public void goToAnimLoader()
        // {
        //     isRecording = false;
        //     mainMenu.SetActive(false);
        //     ingameMenu.SetActive(false);
        //     settingsMenu.SetActive(false);
        //     animLoaderMenu.SetActive(true);
        // }

        void saveOnDisk()
        {
             // Debug.Log(Directory.Exists("Assets"));
            string folderName = screensFolderName + "/" + modelList[modelCounter].name + "/";
            //string folderName = "Screens/" + modelList[modelCounter].name + "/";
            if(Directory.Exists(folderName) == false)
            {
                Directory.CreateDirectory(folderName);
            }

            // int dirCounter = 0;
            // tempFolderName = folderName + dirCounter++.ToString() + "/";
            // while(Directory.Exists(tempFolderName))
            //     tempFolderName = folderName + dirCounter++.ToString() + "/";
            // Directory.CreateDirectory(tempFolderName);

            // File.WriteAllText(tempPath + ".json", skeletonData[frameCounter]);

            int dirCounter = 0;
            string tempPath = folderName + dirCounter++.ToString() + ".png";
            while(File.Exists(tempPath))
                tempPath = folderName + dirCounter++.ToString() + ".png";

            byte[] Bytes = mainCameraController.CamCapture();
            File.WriteAllBytes(tempPath, Bytes);
        }

        GameObject spawnNextModel(bool shuffle = true)
        {
            GameObject tempObject;

            if(shuffle)
                modelCounter = Random.Range(0, modelList.Length);
            else
                modelCounter++;

            if(modelCounter >= modelList.Length)
            {
                modelCounter = 0;
            }
            
            tempObject = Instantiate((GameObject)modelList[modelCounter]);      

            return tempObject;
        }
        Material spawnNextSkyMaterial(bool shuffle = true)
        {
            Material skyMat;

            if(shuffle)
                skyMaterialCounter = Random.Range(0, skyMaterialList.Length);
            else
                skyMaterialCounter++;

            if(skyMaterialCounter >= skyMaterialList.Length)
            {
                skyMaterialCounter = 0;
            }

            skyMat = (Material)skyMaterialList[skyMaterialCounter];        

            return skyMat;
        }

        // Material spawnNextFloorMaterial(bool shuffle = true)
        // {
        //     Material floorMat;

        //     if(shuffle)
        //         floorMaterialCounter = Random.Range(0, floorMaterialList.Length);
        //     else
        //         floorMaterialCounter++;

        //     if(floorMaterialCounter >= floorMaterialList.Length)
        //     {
        //         floorMaterialCounter = 0;
        //     }

        //     floorMat = (Material)floorMaterialList[floorMaterialCounter];    

        //     return floorMat;
        // }

        Material spawnNextModelMaterial(bool shuffle = true)
        {
            Material modelMat;

            if(shuffle)
                modelMaterialCounter = Random.Range(0, modelMaterialList.Length);
            else
                modelMaterialCounter++;

            if(modelMaterialCounter >= modelMaterialList.Length)
            {
                modelMaterialCounter = 0;
            }

            modelMat = (Material)modelMaterialList[modelMaterialCounter];    

            return modelMat;
        }

        public GameObject getModel()
        {
            return loadedModel;
        }

        public string getSaveDir()
        {
            if (screensFolderName == "")
                screensFolderName = Application.dataPath + "/Screens/";

            return screensFolderName;
        }

        public void setSaveDir(string newSaveDir)
        {
            screensFolderName = newSaveDir;
        }

        // public bool getAddMirrors()
        // {
        //     return addMirrors;
        // }

        // public void setAddMirrors(bool newAddMirrors)
        // {
        //     addMirrors = newAddMirrors;
        // }
    }
}
